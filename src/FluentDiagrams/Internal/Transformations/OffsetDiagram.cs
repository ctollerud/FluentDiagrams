using FluentDiagrams.Primitives;
using LinqGarden.Functions;

namespace FluentDiagrams.Internal.Transformations
{
	public class OffsetDiagram : IDiagram, IRotatable
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

		IDiagram IRotatable.PerformRotate( Angle angle ) =>
			new OffsetDiagram( InnerDiagram.Rotate( angle ), Vector );
	}
}
