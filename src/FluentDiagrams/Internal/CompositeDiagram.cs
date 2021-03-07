using LinqGarden;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using LinqGarden.Functions;
using FluentDiagrams.Primitives;

namespace FluentDiagrams.Internal
{
	public class CompositeDiagram : IDiagram, IRotatable, ITranslatable, IScalable
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

		IDiagram ITranslatable.PerformTranslate( decimal x, decimal y ) =>
			Diagrams.Select( diagram => diagram.Offset( x, y ) )
			.ToImmutableList()
			.Pipe( diagrams => new CompositeDiagram( diagrams, this.Bounds.Offset( x, y ) ) );


		/// <summary>
		/// Scale the diagram around a specific point.  This can be done by using an offset and a scale together
		/// </summary>
		private static IDiagram RescaleSubdiagram( IDiagram diagram, Coordinate scaleAround, decimal scaleX, decimal scaleY )
		{
			var oldCenter = diagram.Bounds.Center();

			var oldXDistance =
				oldCenter.X - scaleAround.X;

			var newXDistance = oldXDistance * scaleX;

			var xAdjustment = newXDistance - oldXDistance;

			var oldYDistance = oldCenter.Y - scaleAround.Y;
			var newYDistance = oldYDistance * scaleY;
			var yAdjustment = newYDistance - oldYDistance;

			return diagram
				.Offset( xAdjustment, yAdjustment )
				.Scale( scaleX, scaleY );
		}

		IDiagram IScalable.PerformScaling( decimal x, decimal y ) =>
			Diagrams.Select( diagram => RescaleSubdiagram( diagram, this.Bounds.Center(), x, y ) )
			.ToImmutableList()
			.Pipe( diagrams => new CompositeDiagram( diagrams, this.Bounds.Scale( x, y ) ) );
	}
}
