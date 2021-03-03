using FluentDiagrams.Internal;
using System.Collections.Generic;

namespace FluentDiagrams
{
	public static class Composition
	{
		public static IDiagram Then( this IDiagram input, IDiagram next ) =>
			CompositeDiagram.SingleItem( input ).FollowedBy( next );

		public static IDiagram Then( this IDiagram input, IDiagram next, HorizontalAlignment alignment, bool evaluateOffset = true ) =>
			ThenImplementation( input, next, alignment, null, evaluateOffset );

		public static IDiagram Then( this IDiagram input, IDiagram next, VerticalAlignment alignment, bool evaluateOffset = true ) =>
			ThenImplementation( input, next, null, alignment, evaluateOffset );

		public static IDiagram Then( this IDiagram input, IDiagram next, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, bool evaluateOffset = true ) =>
			ThenImplementation( input, next, horizontalAlignment, verticalAlignment, evaluateOffset );

		private static IDiagram ThenImplementation(
			IDiagram input,
			IDiagram next,
			HorizontalAlignment? horizontalAlignment,
			VerticalAlignment? verticalAlignment,
			bool evaluateOffset )
		{
			var alignmentVector = LayoutUtilities.ComputeAlignment( next, input, verticalAlignment, horizontalAlignment );
			var offsetDiagram = evaluateOffset ? next.Offset( alignmentVector ) : next.WithOffset( alignmentVector );
			return input.Then( offsetDiagram );
		}


		public static IDiagram ComposedOf( params IDiagram[] diagrams ) =>
			CompositeDiagram.Compose( diagrams );

		public static IDiagram Compose( this IEnumerable<IDiagram> diagrams ) =>
			CompositeDiagram.Compose( diagrams );
	}
}
