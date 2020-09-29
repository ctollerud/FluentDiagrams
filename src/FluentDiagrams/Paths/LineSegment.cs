using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Paths
{
	public class LineSegment : IPathSegment
	{
		public Coordinate Coord1 { get; }
		public Coordinate Coord2 { get; }

		public LineSegment( Coordinate coord1, Coordinate coord2 )
		{
			Coord1 = coord1;
			Coord2 = coord2;
		}

		public static LineSegment Create( Coordinate coord1, Vector vector ) =>
			new LineSegment( coord1, coord1.Translate( vector ) );

		public IPathSegment RotateAbout( Coordinate rotationOrigin, Angle angle ) =>
			new LineSegment( Coord1.RotateAbout( rotationOrigin, angle ), Coord2.RotateAbout( rotationOrigin, angle ) );

		public BoundingBox GetBoundingBox( decimal strokeWidth )
		{
			var vector = Vector.FromCoordinates( Coord1, Coord2 );
			var center = Coordinate.Average( Coord1, Coord2 );
			return BoundingBox.Create( vector.Magnitude, strokeWidth, center ).RotateAbout( center, vector.Angle );
		}

		public PathInstructions ToPathInstructions() =>
			new PathInstructions( Coord1, PathInstruction.MoveTo( Coord2 ) );
	}
}
