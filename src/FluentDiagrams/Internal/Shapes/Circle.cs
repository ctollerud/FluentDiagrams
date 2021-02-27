using LinqGarden.Functions;

namespace FluentDiagrams.Primitives
{
	public class CircleDiagram : IDiagram
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
		public IDiagram Rotate( Angle angle ) =>
			this;

	}
}
