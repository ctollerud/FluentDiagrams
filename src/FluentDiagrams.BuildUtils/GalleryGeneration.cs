using FluentDiagrams.Gallery;
using LinqGarden;
using LinqGarden.Enumerables;
using LinqGarden.Functions;
using LinqGarden.Strings;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using static System.Console;

namespace FluentDiagrams.BuildUtils
{
	public static class GalleryGeneration
	{
		public static Fallible<string, Unit> GenerateGallery( string galleryOutputDir, Maybe<string> regressionDir ) =>
		from outcome in Function.From( () =>
		{
			Out.WriteLine( $"Generating gallery to output dir '{galleryOutputDir}'" );

			Directory.CreateDirectory( galleryOutputDir );

			Directory.GetFiles( galleryOutputDir )
			.Do( File.Delete )
			.ForEach();

			var results =
				Gallery.GalleryItems.RegressionList()
				.Select( x => ProcessGalleryItem( x, galleryOutputDir, regressionDir ) )
				.ToList();

			var errorMessages =
				from result in results
				from issue in result.Issue
				from errorLine in
					new[]
					{
						$"item '{result.Item.Title}' failed:",
						issue,
						string.Empty
					}
				select errorLine;

			return
				errorMessages.JoinStrings( Environment.NewLine )
				.NoneIfEmpty()
				.Select( errorMessages => $"The following errors were encountered:{Environment.NewLine}{errorMessages}" )
				.IfSomeFail();

		} ).Invoke()
		select outcome;


		private static RenderingResult ProcessGalleryItem( GalleryItem item, string outputDirPath, Maybe<string> regressionDirPath ) =>
			(
				from svgElement in
					Svg.Renderer.RenderSvgElement( item.Diagram.Compile().Invoke() )
					.IfSuccessDo( svgElement => WriteOutToGallery( svgElement, item, outputDirPath ) )
				from _regressionTest in
					regressionDirPath.To<Fallible<string, Unit>>(
						dir =>
							CheckForRegressions(
								svgElement,
								Path.Combine( dir, $"{item.Title}.svg" ) ),
						() => Fallible.Success<string, Unit>( Unit.Instance ) )
				select _regressionTest
			)
			.GetFailure()
			.Pipe( maybeFailure => new RenderingResult( item, maybeFailure ) );

		private static void WriteOutToGallery( XElement svgElement, GalleryItem item, string outputDirPath )
		{
			string html =
				NestInHtml( item.Title, item.Description, svgElement, item.Diagram ).ToString();

			string svg =
				svgElement.ToString();

			( from pair in
				new[]
				{
					(Filename: $"{item.Title}.html", Text: html),
					(Filename: $"{item.Title}.svg", Text: svg)
				}
			  let fullDestinationPath = Path.Combine( outputDirPath, pair.Filename )
			  select Function.From( () =>
			  {
				  Console.Out.WriteLine( $"writing file '{fullDestinationPath}'..." );
				  File.WriteAllText( fullDestinationPath, pair.Text );
			  } )
			.Invoke() )
			.ForEach();
		}


		private static Fallible<string, Unit> CheckForRegressions( XElement svgElement, string regressionFilePath ) =>
			from expectedElement in
				Function.From( () => XElement.Load( regressionFilePath ) )
				.CatchAsFailure(
					( XmlException x ) =>
						new[]
						{
							$"XML Parse Failed:",
							x.Message,
							$"Line:{x.LineNumber}, Position:{x.LinePosition}"
						}
						.JoinStrings( Environment.NewLine ) )
				.CatchAsFailure( ( FileNotFoundException x ) => x.Message )
				.Invoke()
			from _performComparison in XmlComparison.CompareElements( svgElement, expectedElement )
			select _performComparison;

		private static XElement NestInHtml( string title, string description, XElement svgElement, Expression<Func<IDiagram>> expression ) =>
			new XElement( "html",
				new XElement( "head",
					new XElement( "title", title
					)
				),
				new XElement( "body",
					new XElement( "div",
						new XElement( "h2", description ),
						new XElement( "h3", expression.ToString()[5..] )
					),
					new XElement( "div",
						svgElement
					)
				)
			);

	}
}
