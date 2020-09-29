using FluentDiagrams.Internal.Transformations;
using FluentDiagrams.Primitives;

namespace FluentDiagrams
{
	public static class LayoutExtensions
	{
		public static IDiagram Scale( this IDiagram diagram, decimal x = 1, decimal y = 1 ) =>
			new ScaledDiagram( diagram, x, y );

		public static IDiagram WithOffset( this IDiagram diagram, decimal translateX, decimal translateY ) =>
			WithOffset( diagram, new Vector( translateX, translateY ) );

		public static IDiagram WithOffset( this IDiagram diagram, Vector vector ) =>
			new OffsetDiagram( diagram, vector );
	}
}
