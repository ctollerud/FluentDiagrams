using FluentDiagrams.Internal;
using FluentDiagrams.Primitives;
using LinqGarden;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FluentDiagrams
{
	public static class RandomUtilities
	{
		public static Random<decimal> RandomDecimal( decimal min, decimal max, decimal curvature = 1 ) =>
			MakeRandom.NextDouble()
			.Select( x => Math.Pow( x, Convert.ToDouble( curvature ) ) )
			.Select( x => x * Convert.ToDouble( max - min ) )
			.Select( Convert.ToDecimal )
			.Select( x => x + min );

		public static Random<Angle> RandomAngle() =>
			MakeRandom.NextDouble().Select( Convert.ToDecimal ).Select( Angle.FromRotations );

		private static readonly Random<int> s_RandomArgbComponent =
			MakeRandom.Next( 0, 256 );

		public static Random<Color> RandomRgbColor() =>
			from r in s_RandomArgbComponent
			from g in s_RandomArgbComponent
			from b in s_RandomArgbComponent
			select Color.FromArgb( r, g, b );

		public static Random<Color> RandomRgbColor( Color color1, Color color2 ) =>
			from blendAmount in RandomUtilities.RandomDecimal( 0, 1 )
			select Colors.ColorExtensions.Blend( color1, color2, blendAmount );

		public static Random<T> OneOf<T>( params T[] items )
		{
			if( items.Length == 0 )
			{
				throw new InvalidOperationException( "Unable to select a value from an empty array" );
			}

			return
				from index in MakeRandom.Next( 0, items.Length )
				select items[index];

		}



	}
}
