using System.Drawing;
using System.Linq;
using LinqGarden.Enumerables;
using LinqGarden.Functions;

namespace FluentDiagrams.Svg.Internal
{
	public static class SvgColorFactory
	{
		public static string SvgColor( Color color )
		{
			return new[]
			{
				color.R,
				color.G,
				color.B
			}
			.Select( x => x.ToString( "X2" ) )
			.StartWith( "#" )
			.Pipe( string.Concat );
		}

		public static string SvgOpacity( Color fillColor ) =>
			fillColor
			.Pipe( x => (decimal)x.A )
			.Pipe( x => x / 255 )
			.Pipe( x => x.ToString() );
	}
}
