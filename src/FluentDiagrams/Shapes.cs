using FluentDiagrams.Internal;
using FluentDiagrams.Internal.Shapes;
using FluentDiagrams.Paths;
using FluentDiagrams.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace FluentDiagrams
{
	public static class Shapes
	{

		public static IDiagram Circle( decimal radius = 0.5M, Coordinate? center = null ) =>
			new CircleDiagram( radius, center ?? Coordinate.Origin() );

		public static IDiagram Ellipse( decimal xRadius, decimal yRadius, Coordinate? center = null )
		{
			if( center == null )
			{
				center = Coordinate.Origin();
			}

			if( xRadius == yRadius )
			{
				return Circle( xRadius, center );
			}

			return new EllipseDiagram( xRadius, yRadius, center );
		}

		/// <summary>
		/// A rectangle with sides of length 1
		/// </summary>
		/// <returns></returns>
		public static IDiagram Square() =>
			new RectangleDiagram( 1, 1, Coordinate.Origin() );

		public static IDiagram Segment( IPathSegment segment, decimal strokeWidth, StrokeStyle strokeStyle ) =>
			new PathDiagram( segment.ToPathInstructions(), strokeWidth, strokeStyle );

		public static IDiagram LineSegment( Coordinate startCoordinate, Coordinate endCoordinate, decimal strokeWidth, StrokeStyle strokeStyle ) =>
			LineSegments( strokeWidth, strokeStyle, startCoordinate, endCoordinate );

		public static IDiagram LineSegments( decimal strokeWidth, StrokeStyle strokeStyle, params Coordinate[] coordinates ) =>
			LineSegments( strokeWidth, strokeStyle, coordinates.AsEnumerable() );

		public static IDiagram LineSegments( decimal strokeWidth, StrokeStyle strokeStyle, IEnumerable<Coordinate> coordinates ) =>
			new PathDiagram( PathInstructions.Segments( coordinates ), strokeWidth, strokeStyle );

		public static IDiagram LineSegment( LineSegment lineSegment, decimal strokeWidth, StrokeStyle strokeStyle ) =>
			Segment( lineSegment, strokeWidth, strokeStyle );

		public static IDiagram CubicSegment(
			Coordinate start,
			Coordinate end,
			Coordinate controlPoint1,
			Coordinate controlPoint2,
			decimal strokeWidth,
			StrokeStyle strokeStyle ) =>
			new PathDiagram( new CubicSegment( start, controlPoint1, controlPoint2, end ).ToPathInstructions(), strokeWidth, strokeStyle );
	}
}
