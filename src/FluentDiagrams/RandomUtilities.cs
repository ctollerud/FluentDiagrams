using FluentDiagrams.Primitives;
using LinqGarden;
using System;
using System.Collections.Generic;
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
