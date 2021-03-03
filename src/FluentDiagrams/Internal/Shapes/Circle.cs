using FluentDiagrams.Primitives;
using LinqGarden.Functions;

namespace FluentDiagrams.Internal.Shapes
{
	public class CircleDiagram : IDiagram, IRotatable, ITranslatable, IScalable
	{
		public BoundingBox Bounds { get; }

		public static CircleDiagram Default =>
			new CircleDiagram(
				radius: 0.5M,
				origin: Coordinate.Origin() );

		internal CircleDiagram( decimal radius, Coordinate origin )
		{
			Radius = radius;
			Origin = origin;

			Bounds =
				( radius * 2 )
				.Pipe( diameter => BoundingBox.Create( diameter, diameter, origin ) );
		}
		public decimal Radius { get; }
		public Coordinate Origin { get; }

		/// <summary>
		/// Rotating a circle about its origin is a noop.
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		IDiagram IRotatable.PerformRotate( Angle angle ) =>
			this;

		IDiagram ITranslatable.PerformTranslate( decimal x, decimal y ) =>
			new CircleDiagram( Radius, Origin.Translate( x, y ) );

		IDiagram IScalable.PerformScaling( decimal x, decimal y )
		{
			return
				(x, y) switch
				{
					(1, 1 ) => this,
					(decimal scaleX, decimal scaleY ) when scaleX == scaleY => new CircleDiagram( Radius * scaleX, this.Origin ),

					//Replace this with ellipse once we've implemented it.
					_ => this.WithScale( x, y )
				};
		}
	}
}
