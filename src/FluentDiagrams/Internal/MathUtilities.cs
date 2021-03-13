using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FluentDiagrams.Primitives;
using LinqGarden.Functions;

namespace FluentDiagrams.Internal
{
	public static class MathUtilities
	{
		public static decimal Rescale(
			decimal input,
			decimal inMin,
			decimal inMax,
			decimal outMin,
			decimal outMax ) =>
				input
				.Pipe( x => x - inMin )
				.Pipe( x => x / ( inMax - inMin ) )
				.Pipe( x => x * ( outMax - outMin ) )
				.Pipe( x => x + outMin );

		public static double Rescale(
					double input,
					double inMin,
					double inMax,
					double outMin,
					double outMax,
					double curve = 1 ) =>
				input
				.Pipe( x => x - inMin )
				.Pipe( x => x / ( inMax - inMin ) )

				.Pipe( x => Math.Pow( x, curve ) )

				.Pipe( x => x * ( outMax - outMin ) )
				.Pipe( x => x + outMin );


		/// <summary>
		/// Create a number of evenly distributed values between (and including) the start and end values.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static IEnumerable<decimal> Interpolate( decimal start, decimal end, int count )
		{
			if( count <= 0 )
			{
				return Enumerable.Empty<decimal>();
			}

			if( count == 1 )
			{
				return new[] { start };
			}
			var distancebetweenValues = 1M / ( count - 1 );

			return Enumerable.Range( 0, count )
				.Select( x => distancebetweenValues * x )
				.Select( x => x * ( end - start ) )
				.Select( x => x + start );
		}

		public static IEnumerable<decimal> Interpolate( int start, int end, int count ) =>
			Interpolate( Convert.ToDecimal( start ), Convert.ToDecimal( end ), count );

		/// <summary>
		/// Create a number of evenly distributed values between (and including) the start and end values.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static IEnumerable<double> Interpolate( double start, double end, int count )
		{
			if( count <= 0 )
			{
				return Enumerable.Empty<double>();
			}

			if( count == 1 )
			{
				return new[] { start };
			}
			var distancebetweenValues = 1.0 / ( count - 1 );

			return Enumerable.Range( 0, count )
				.Select( x => distancebetweenValues * x )
				.Select( x => x * ( end - start ) )
				.Select( x => x + start );
		}

		/// <summary>
		/// Create a specific number of values evenly distributed between min and max, but excluding a value for the max
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static IEnumerable<decimal> LeftInterpolate( decimal min, decimal max, int count ) =>
			Interpolate( min, max, count + 1 ).Take( count );


		/// <summary>
		/// create an even distribution of angles between 0 and 1 rotations.
		/// </summary>
		public static IEnumerable<Angle> InterpolateAngles( int count ) =>
			from rotationAmount in LeftInterpolate( 0M, 1M, count )
			select Angle.FromRotations( rotationAmount );

		public static IEnumerable<Coordinate> InterpolateCoordinates( Coordinate start, Coordinate end, int count ) =>
			Interpolate( start.X, end.X, count )
			.Zip( Interpolate( start.Y, end.Y, count ), Coordinate.Cartesian );
	}
}
