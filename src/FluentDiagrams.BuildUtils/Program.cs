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
	internal class Program
	{
		private class RenderingResult
		{
			public RenderingResult( GalleryItem item, Maybe<string> issue )
			{
				this.Item = item;
				this.Issue = issue;
			}

			public GalleryItem Item { get; }
			public Maybe<string> Issue { get; }
		}

		private static Maybe<string> GetFlagValue( string[] args, string flagString ) =>
			args.SkipWhile( x => !x.Equals( flagString, StringComparison.OrdinalIgnoreCase ) )
			.Skip( 1 )
			.FirstOrNone();

		private static Fallible<string, string> GetRequiredFlagValue( string[] args, string flagString ) =>
			GetFlagValue( args, flagString )
			.IfNoneFail( $"failed to locate value for required flag '{flagString}'" );

		/// <summary>
		/// For now this app will just be used for generating the gallery.
		/// Later on, we'll have it be able to handle more tasks.
		/// </summary>
		/// <param name="args"></param>
		private static int Main( string[] args )
		{
			return GenerateGallery( args )
				.IfFailureDo( failure =>
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Out.WriteLine( failure );
				} )
				.To<int>(
					failure => 1,
					success => 0 );
		}

		private static Fallible<string, Unit> GenerateGallery( string[] args ) =>
			from galleryOutputDir in GetRequiredFlagValue( args, "--GalleryOutputDir" )
			from regressionDir in GetRequiredFlagValue( args, "--RegressionPathDir" )
			from outcome in FunctionExtensions.Function( () =>
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
					from errorLine in new[]
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


		private static RenderingResult ProcessGalleryItem( GalleryItem item, string outputDirPath, string regressionDirPath ) =>
			(
				from svgElement in
					Svg.Renderer.RenderSvgElement( item.Diagram.Compile().Invoke() )
					.IfSuccessDo( svgElement => WriteOutToGallery( svgElement, item, outputDirPath ) )
				let regressionFilePath = System.IO.Path.Combine( regressionDirPath, $"{item.Title}.svg" )
				from _regressionTest in CheckForRegressions( svgElement, regressionFilePath )
				select _regressionTest
			)
			.FailureValue
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
			  select FunctionExtensions.Function( () =>
			  {
				  Console.Out.WriteLine( $"writing file '{fullDestinationPath}'..." );
				  File.WriteAllText( fullDestinationPath, pair.Text );
			  } )
			.Invoke() )
			.ForEach();
		}


		private static Fallible<string, Unit> CheckForRegressions( XElement svgElement, string regressionFilePath ) =>
			from expectedElement in
				FallibleFunction.Build( () => XElement.Load( regressionFilePath ) )
				.Catch<XmlException>()
					.SelectFailure( x =>
						new[]
						{
							$"XML Parse Failed:",
							x.Message,
							$"Line:{x.LineNumber}, Position:{x.LinePosition}"
						}
						.JoinStrings( Environment.NewLine ) )
				.Catch<FileNotFoundException>( x => x.Message )
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
						new XElement( "h3", expression.ToString().Substring( 5 ) )
					),
					new XElement( "div",
						svgElement
					)
				)
			);
	}
}
