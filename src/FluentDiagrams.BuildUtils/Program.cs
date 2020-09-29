using LinqGarden;
using LinqGarden.Enumerables;
using System;
using System.Linq;

namespace FluentDiagrams.BuildUtils
{
	internal class Program
	{

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
			from _result in GalleryGeneration.GenerateGallery( galleryOutputDir, default )
			select _result;
	}
}
