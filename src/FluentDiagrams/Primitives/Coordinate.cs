using System;
using System.Collections.Generic;
using System.Linq;
using LinqGarden.Functions;

namespace FluentDiagrams.Primitives
{
	public class Coordinate
	{
		public Coordinate( decimal x, decimal y )
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Get the cartesian coordinate X position
		/// </summary>
		public decimal X { get; }

		/// <summary>
		/// Get the cartesian coordinate Y position
		/// </summary>
		public decimal Y { get; }

		/// <summary>
		/// Get the cartesian coordinate angle
		/// </summary>
		public Angle Angle =>
			Math.Atan2( (double)Y, (double)X )
			.Pipe( x => (decimal)x )
			.Pipe( Angle.FromRadians );


		/// <summary>
		/// Get the cartesian coordinate radius
		/// </summary>
		public decimal Radius =>
			( Math.Pow( (double)X, 2 ) + Math.Pow( (double)Y, 2 ) )
			.Pipe( Math.Sqrt )
			.Pipe( x => (decimal)x );

		public Coordinate Translate( Vector vector ) =>
			new Coordinate( X + vector.Dx, Y + vector.Dy );

		public Coordinate Translate( decimal dx, decimal dy ) =>
			new Coordinate( X + dx, Y + dy );

		public Coordinate RotateAbout( Coordinate rotationOrigin, Angle angle )
		{
			//convert to polar coordinates

			var preRotateTranslate = Vector.FromCoordinates( rotationOrigin, Coordinate.Origin() );
			var postRotateTranslate = preRotateTranslate.Reverse();

			return
			this
				.Translate( preRotateTranslate )
				.Pipe( x => x.Rotate( angle ) )
				.Translate( postRotateTranslate );
		}

		public static Coordinate Cartesian( decimal x, decimal y ) =>
			new Coordinate( x, y );

		public Coordinate Rotate( Angle angle ) =>
			Coordinate.Polar( Angle.Plus( angle ), Radius );

		public static Coordinate Origin() =>
			new Coordinate( 0M, 0M );

		public static Coordinate Polar( Angle angle, decimal radius )
		{
			decimal x = radius * (decimal)Math.Cos( (double)angle.Radians );
			decimal y = radius * (decimal)Math.Sin( (double)angle.Radians );

			return new Coordinate( x, y );
		}

		public static Coordinate Average( params Coordinate[] coordinates ) =>
			Coordinate.Cartesian( coordinates.Select( x => x.X ).Average(), coordinates.Select( x => x.Y ).Average() );

		public static IEnumerable<Coordinate> InterpolateBetween( Coordinate start, Coordinate end, int totalValues )
		{
			var xCoords = Maths.Interpolate( start.X, end.X, totalValues );
			var yCoords = Maths.Interpolate( start.Y, end.Y, totalValues );

			return xCoords.Zip( yCoords, ( x, y ) => Coordinate.Cartesian( x, y ) );
		}

		public static Coordinate Bisect( Coordinate start, Coordinate end ) =>
			 Average( start, end );

		/// <summary>
		/// Scale the coordinate about the origin
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Coordinate Scale( decimal x = 1, decimal y = 1 ) =>
			Coordinate.Cartesian( this.X * x, this.Y * y );

	}
}
