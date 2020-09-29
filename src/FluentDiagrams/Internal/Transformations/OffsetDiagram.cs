using FluentDiagrams.Primitives;
using LinqGarden.Functions;

namespace FluentDiagrams.Internal.Transformations
{
	public class OffsetDiagram : IDiagram
	{

		public IDiagram InnerDiagram { get; }
		public Vector Vector { get; }
		public BoundingBox Bounds { get; }

		internal OffsetDiagram( IDiagram innerDiagram, Vector vector )
		{
			InnerDiagram = innerDiagram;
			Vector = vector;
			Bounds = innerDiagram.Bounds.Offset( vector );
		}

		public IDiagram DeepRotate( Coordinate coordinate, Angle angle ) =>
			Coordinate.Origin().Translate( Vector )
			.RotateAbout( coordinate, angle )
			.Pipe( x => Vector.FromCoordinates( Coordinate.Origin(), x ) )
			.Pipe( x => new OffsetDiagram( InnerDiagram.DeepRotate( InnerDiagram.Bounds.Center(), angle ), x ) );
	}
}
