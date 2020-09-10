using LinqGarden.Functions;
using System;

namespace FluentDiagrams.Primitives
{
	public class Angle
	{
		private static readonly decimal TAU = (decimal)( 2 * Math.PI );

		public decimal Rotations { get; }
		public decimal Radians => Rotations * TAU;
		public decimal Degrees => Rotations * 360;

		public static Angle Zero { get; } = Angle.FromRotations( 0 );

		private Angle( decimal rotations )
		{
			Rotations = rotations;
		}

		public static Angle FromRotations( decimal rotations ) =>
			new Angle( rotations );

		public static Angle FromRadians( decimal radians ) =>
			radians
			.Pipe( x => (double)x )
			.Pipe(x => ( x / ( 2 * Math.PI ) ) )
			.Pipe( x => (decimal)x )
			.Pipe( FromRotations );

		public Angle Plus( Angle angle ) =>
			Angle.FromRotations( Rotations + angle.Rotations );

		public Angle Reverse() =>
			new Angle( rotations: 1M - Rotations );

		public Angle Scale( decimal scale ) =>
			Angle.FromRotations( Rotations * scale );

		public static Angle FromDegrees( decimal v ) =>
			v
			.Pipe( x => (double)x )
			.Pipe( x => ( x / 360 ) )
			.Pipe( x => (decimal)x )
			.Pipe( Angle.FromRotations );
	}
}