using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FluentDiagrams.Colors
{
	public static class ColorExtensions
	{
		public static Color WithOpacity( this Color color, int alpha ) =>
			Color.FromArgb( alpha, color );
		public static Color MapRed( this Color input, Func<int, int> mapper ) =>
			Color.FromArgb( input.A, BoundValue( mapper( input.R ) ), input.G, input.B );

		public static Color MapGreen( this Color input, Func<int, int> mapper ) =>
			Color.FromArgb( input.A, input.R, BoundValue( mapper( input.G ) ), input.B );

		public static Color MapBlue( this Color input, Func<int, int> mapper ) =>
			Color.FromArgb( input.A, input.R, input.G, BoundValue( mapper( input.B ) ) );


		private static int BoundValue( int value ) =>
			Math.Min( Math.Max( value, 0 ), 255 );
	}
}
