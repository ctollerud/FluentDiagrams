using FluentDiagrams.Internal;

namespace FluentDiagrams
{
	public static class Composition
	{
		public static IDiagram Then( this IDiagram input, IDiagram next ) =>
			CompositeDiagram.SingleItem( input ).FollowedBy( next );
	}
}
