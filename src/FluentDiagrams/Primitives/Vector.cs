using LinqGarden.Enumerables;
using LinqGarden.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentDiagrams.Primitives
{
	public class Vector
	{
		public Vector( decimal dx, decimal dy )
		{
			Dx = dx;
			Dy = dy;
		}

		public decimal Dx { get; }
		public decimal Dy { get; }
		public decimal Magnitude =>
			( Dx * Dx + Dy * Dy )
			.Pipe( x => (double)x )
			.Pipe( Math.Sqrt )
			.Pipe( x => (decimal)x );

		public Angle Angle => Coordinate.Cartesian( Dx, Dy ).Angle;

		public static Vector Zero { get; } = new Vector( 0, 0 );

		public static Vector FromCoordinates( Coordinate start, Coordinate end ) =>
			new Vector( end.X - start.X, end.Y - start.Y );

		public Vector Reverse() => new Vector( -Dx, -Dy );

		public Vector Rotate( Angle angle ) =>
			Coordinate.Cartesian( Dx, Dy ).RotateAbout( Coordinate.Origin(), angle ).Pipe( c => new Vector( c.X, c.Y ) );

		public static Vector Create( Angle angle, decimal v ) =>
			Coordinate.Polar( angle, v ).Pipe( x => new Vector( x.X, x.Y ) );

		public static Vector Create( decimal dx, decimal dy ) =>
			new Vector( dx, dy );

		public Vector Add( Vector b ) => Vector.Create( Dx + b.Dx, Dy + b.Dy );
	}

	public static class VectorExt
	{
		public static Vector Sum( this IEnumerable<Vector> vectors ) =>
			vectors.StartWith( Vector.Zero ).Aggregate( ( a, b ) => a.Add( b ) );

		public static Vector Scale( this Vector input, decimal scaleMagnitude ) =>
			Vector.Create( input.Angle, scaleMagnitude * input.Magnitude );
	}
}
