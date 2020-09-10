using System;
using System.Collections.Generic;
using System.Linq;
using LinqGarden.Functions;

namespace FluentDiagrams.Primitives
{
	public static class Maths
	{
		public static decimal Rescale( this decimal input, decimal inputMin, decimal inputMax, decimal outputMin, decimal outputMax, decimal exponent = 1 ) =>
			input
			.Pipe( x => x - inputMin )
			.Pipe( x => x / ( inputMax - inputMin ) )
			.Pipe( x => Math.Pow( (double)x, (double)exponent ) )
			.Pipe( x => (decimal)x )
			.Pipe( x => x * ( outputMax - outputMin ) )
			.Pipe( x => x + outputMin );

		public static IEnumerable<decimal> Interpolate( decimal start, decimal end, int count )
		{
			if( count <= 0 )
			{
				return new decimal[] { };
			}

			if( count == 1 )
			{
				return new[] { start };
			}
			var distancebetweenValues = ( 1M / (count - 1 ) );

			return Enumerable.Range( 0, count )
				.Select( x => distancebetweenValues * x )
				.Select( x => x * ( end - start ) )
				.Select( x => x + start );
		}

		public static IEnumerable<decimal> LeftDistribute( decimal min, decimal max, int count ) =>
			Interpolate( min, max, count + 1 ).Take( count );
	}
}
