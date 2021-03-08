using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FluentDiagrams.Colors
{
	public class HslColor
	{
		/// <summary>
		/// 0 to 1
		/// </summary>
		public double Hue { get; }

		/// <summary>
		/// 0 to 1
		/// </summary>
		public double Saturation { get; }

		/// <summary>
		/// 0 to 1
		/// </summary>
		public double Luminosity { get; }

		/// <summary>
		/// We store Saturation and Luminosity between 0 and 1. 0 and 240 is what .NET uses.
		/// </summary>
		//const double ComponentScale = 240.0;

		internal HslColor( double hue, double saturation, double luminosity )
		{
			Hue = hue;
			Saturation = saturation;
			Luminosity = luminosity;
		}

		public Color ToRgb( int alpha = 255 )
		{
			double r = 0, g = 0, b = 0;
			if( Luminosity != 0 )
			{
				if( Saturation == 0 )
				{
					r = g = b = Luminosity;
				}
				else
				{
					double temp2 = GetTemp2();
					double temp1 = ( 2.0 * Luminosity ) - temp2;

					r = GetColorComponent( temp1, temp2, Hue + ( 1.0 / 3.0 ) );
					g = GetColorComponent( temp1, temp2, Hue );
					b = GetColorComponent( temp1, temp2, Hue - ( 1.0 / 3.0 ) );
				}
			}
			return Color.FromArgb( alpha, (int)( 255 * r ), (int)( 255 * g ), (int)( 255 * b ) );
		}

		private static double GetColorComponent( double temp1, double temp2, double temp3 )
		{
			temp3 = MoveIntoRange( temp3 );
			if( temp3 < 1.0 / 6.0 )
			{
				return temp1 + ( ( temp2 - temp1 ) * 6.0 * temp3 );
			}
			else if( temp3 < 0.5 )
			{
				return temp2;
			}
			else if( temp3 < 2.0 / 3.0 )
			{
				return temp1 + ( ( temp2 - temp1 ) * ( ( 2.0 / 3.0 ) - temp3 ) * 6.0 );
			}
			else
			{
				return temp1;
			}
		}
		private static double MoveIntoRange( double value )
		{
			if( value < 0.0 )
			{
				value += 1.0;
			}
			else if( value > 1.0 )
			{
				value -= 1.0;
			}

			return value;
		}

		private double GetTemp2()
		{
			double temp2 = Luminosity < 0.5
				? Luminosity * ( 1.0 + Saturation )
				: Luminosity + Saturation - ( Luminosity * Saturation );
			return temp2;
		}


		public static HslColor FromRgb( Color color ) =>
			new HslColor(
				color.GetHue() / 360.0,// we store hue as 0-1 as opposed to 0-360 
				color.GetSaturation(),
				color.GetBrightness() );
	}
}
