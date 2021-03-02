using FluentDiagrams.Internal;
using System.Collections.Generic;

namespace FluentDiagrams
{
	public static class Composition
	{
		public static IDiagram Then( this IDiagram input, IDiagram next ) =>
			CompositeDiagram.SingleItem( input ).FollowedBy( next );

		public static IDiagram ComposedOf( params IDiagram[] diagrams ) =>
			CompositeDiagram.Compose( diagrams );

		public static IDiagram Compose( this IEnumerable<IDiagram> diagrams ) =>
			CompositeDiagram.Compose( diagrams );
	}
}
