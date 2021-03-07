using FluentDiagrams.Primitives;
using LinqGarden;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams
{
	public static class RandomUtilities
	{
		public static Random<decimal> RandomDecimal( decimal min, decimal max ) =>
			MakeRandom.NextDouble()
			.Select( x => x * Convert.ToDouble( max - min ) )
			.Select( Convert.ToDecimal )
			.Select( x => x + min );

		//TODO: create method to generate ranges that include curvature!

		public static Random<Angle> RandomAngle() =>
			MakeRandom.NextDouble().Select( Convert.ToDecimal ).Select( Angle.FromRotations );


	}
}
