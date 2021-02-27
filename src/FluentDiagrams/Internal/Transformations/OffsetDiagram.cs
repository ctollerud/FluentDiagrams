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

		public IDiagram Rotate( Angle angle ) =>
			Coordinate.Origin().Translate( Vector )
			.RotateAbout( Bounds.Center(), angle )
			.Pipe( newOrigin => Vector.FromCoordinates( Coordinate.Origin(), newOrigin ) )
			.Pipe( newVector => new OffsetDiagram( InnerDiagram.Rotate( angle ), newVector ) );
	}
}
