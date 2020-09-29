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

		public IDiagram DeepRotate( Coordinate coordinate, Angle angle ) =>
			new CircleDiagram( Radius, Origin.RotateAbout( coordinate, angle ) );
	}
}
