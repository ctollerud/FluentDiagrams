using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Paths
{
	public class CubicSegment : IPathSegment
	{

		public CubicSegment( Coordinate startPosition, Coordinate controlPoint1, Coordinate controlPoint2, Coordinate endPoint )
		{
			StartPosition = startPosition;
			ControlPoint1 = controlPoint1;
			ControlPoint2 = controlPoint2;
			EndPoint = endPoint;
		}

		public Coordinate StartPosition { get; }
		public Coordinate ControlPoint1 { get; }
		public Coordinate ControlPoint2 { get; }
		public Coordinate EndPoint { get; }

		public PathInstructions ToPathInstructions() =>
			new PathInstructions( StartPosition, PathInstruction.Cubic( EndPoint, ControlPoint1, ControlPoint2 ) );
	}

	public static class CubicSegmentExtensions
	{
		public static CubicSegment ToCubicSegment( this LineSegment segment, Coordinate controlPoint1, Coordinate controlPoint2 ) =>
			new CubicSegment( segment.Coord1, controlPoint1, controlPoint2, segment.Coord2 );
	}
}
