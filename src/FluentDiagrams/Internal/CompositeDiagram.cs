using LinqGarden;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using LinqGarden.Functions;
using FluentDiagrams.Primitives;

namespace FluentDiagrams.Internal
{
	public class CompositeDiagram : IDiagram, IRotatable
	{
		public ImmutableList<IDiagram> Diagrams { get; }
		public BoundingBox Bounds { get; }

		private CompositeDiagram( ImmutableList<IDiagram> diagrams, BoundingBox bounds )
		{
			Diagrams = diagrams;
			Bounds = bounds;
		}

		public static CompositeDiagram SingleItem( IDiagram diagram )
		{
			if( diagram is CompositeDiagram compositeDiagram )
			{
				return compositeDiagram;
			}


			var list = new[] { diagram }.ToImmutableList();
			return new CompositeDiagram( list, diagram.Bounds );
		}

		internal CompositeDiagram FollowedBy( IDiagram next )
		{
			if( next is CompositeDiagram composite )
			{
				var combinedList = this.Diagrams.AddRange( composite.Diagrams );
				var bounds =
					new[] { this.Bounds, next.Bounds }
					.Pipe( BoundingBox.Compose );
				return new CompositeDiagram( combinedList, bounds );
			}
			return this.FollowedBy( CompositeDiagram.SingleItem( next ) );
		}


		internal static CompositeDiagram Compose( IEnumerable<IDiagram> diagrams ) =>
			diagrams
			.Select( CompositeDiagram.SingleItem )
			.Aggregate( ( x, y ) => x.FollowedBy( y ) );

		IDiagram IRotatable.PerformRotate( Angle angle ) =>
			Diagrams.Select( diagram => diagram.RotateAbout( Bounds.Center(), angle ) )
			.ToImmutableList()
			.Pipe( x => new CompositeDiagram( x, BoundingBox.Compose( x.Select( y => y.Bounds ) ) ) );
	}
}
