using FluentDiagrams.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using LinqGarden.Functions;
using System.Linq;
using LinqGarden.Enumerables;

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

		private static int BlendColorComponent( int firstComponent, int secondComponent, decimal mix ) =>
			MathUtilities.Rescale( mix, 0M, 1M, firstComponent, secondComponent )
			.Pipe( Math.Round )
			.Pipe( Convert.ToInt32 );

		/// <summary>
		/// Blend the two colors into a single color
		/// </summary>
		/// <param name="firstColor"></param>
		/// <param name="secondColor"></param>
		/// <param name="mix">0 = completely the first color, 1 = completely the second color, 0.5 = even mix</param>
		/// <returns></returns>
		public static Color Blend( Color firstColor, Color secondColor, decimal mix ) =>
			Color.FromArgb(
				BlendColorComponent( firstColor.A, secondColor.A, mix ),
				BlendColorComponent( firstColor.R, secondColor.R, mix ),
				BlendColorComponent( firstColor.G, secondColor.G, mix ),
				BlendColorComponent( firstColor.B, secondColor.B, mix )
			);


		private static double MixHslComponent( double firstComponent, double secondComponent, decimal mix ) =>
			MathUtilities.Rescale( Convert.ToDouble( mix ), 0.0, 1.0, firstComponent, secondComponent );

		private static IEnumerable<double> InterpolateHslComponent( double firstComponent, double secondComponent, int count ) =>
			MathUtilities.Interpolate( firstComponent, secondComponent, count );


		/// <summary>
		/// Helps in finding the shortest path from one hue to another.
		/// Returns some new "denormalized" hue values that allow for better interpolation.
		/// By denormalized, we mean that they are no longer guaranteed to be between 0 and 1.
		/// </summary>
		/// <param name="firstHue"></param>
		/// <param name="secondHue"></param>
		/// <returns></returns>
		private static (double firstHue, double secondHue) PrepHuesForBlend( double firstHue, double secondHue )
		{
			if( firstHue - secondHue > 0.5 )
			{
				secondHue += 1.0;
			}
			else if( secondHue - firstHue > 0.5 )
			{
				firstHue += 1.0;
			}

			return (firstHue, secondHue);
		}

		private static HslColor BlendHsl( double h1, double h2, double s1, double s2, double l1, double l2, decimal mix ) =>
			new HslColor(
				MixHslComponent( h1, h2, mix ),
				MixHslComponent( s1, s2, mix ),
				MixHslComponent( l1, l2, mix ) );

		/// <summary>
		/// Blend the two colors into a single color
		/// </summary>
		/// <param name="firstColor"></param>
		/// <param name="secondColor"></param>
		/// <param name="mix">0 = completely the first color, 1 = completely the second color, 0.5 = even mix</param>
		/// <returns></returns>
		public static Color BlendHsl( Color firstColor, Color secondColor, decimal mix )
		{
			(var firstHsl, var secondHsl) = (HslColor.FromRgb( firstColor ), HslColor.FromRgb( secondColor ));

			(var firstHue, var secondHue) = PrepHuesForBlend( firstHsl.Hue, secondHsl.Hue );

			var newOpacity = BlendColorComponent( firstColor.A, secondColor.A, mix );

			return BlendHsl(
				firstHue,
				secondHue,
				firstHsl.Saturation,
				secondHsl.Saturation,
				firstHsl.Luminosity,
				secondHsl.Luminosity,
				mix ).ToRgb( newOpacity );
		}


		public static IEnumerable<int> InterpolateArgbComponent( int firstComponent, int secondComponent, int count ) =>
			MathUtilities.Interpolate( firstComponent, secondComponent, count )
			.Select( Convert.ToInt32 );

		public static IEnumerable<Color> InterpolateRgb( Color firstColor, Color secondColor, int count )
		{
			return
			 Sequence.Zip(
				 InterpolateArgbComponent( firstColor.A, secondColor.A, count ),
				 InterpolateArgbComponent( firstColor.R, secondColor.R, count ),
				 InterpolateArgbComponent( firstColor.G, secondColor.G, count ),
				 InterpolateArgbComponent( firstColor.B, secondColor.B, count ),
				 ( a, r, g, b ) => Color.FromArgb( a, r, g, b ) );
		}

		public static IEnumerable<Color> InterpolateHsl( Color firstColor, Color secondColor, int count )
		{
			(var firstHsl, var secondHsl) = (HslColor.FromRgb( firstColor ), HslColor.FromRgb( secondColor ));

			(var firstHue, var secondHue) = PrepHuesForBlend( firstHsl.Hue, secondHsl.Hue );

			return
			Sequence.Zip(
				InterpolateHslComponent( firstHue, secondHue, count ),
				InterpolateHslComponent( firstHsl.Saturation, secondHsl.Saturation, count ),
				InterpolateHslComponent( firstHsl.Luminosity, secondHsl.Luminosity, count ),
				InterpolateArgbComponent( firstColor.A, secondColor.A, count ),
				( h, s, l, a ) => new HslColor( h, s, l ).ToRgb( a ) );



		}

	}
}
