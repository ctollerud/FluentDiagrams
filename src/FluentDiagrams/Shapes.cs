using FluentDiagrams.Internal.Shapes;
using FluentDiagrams.Primitives;

namespace FluentDiagrams
{
	public static class Shapes
	{

		public static IDiagram Circle( decimal radius = 0.5M, Coordinate? center = null ) =>
			new CircleDiagram( radius, center ?? Coordinate.Origin() );

		public static IDiagram Ellipse( decimal xRadius, decimal yRadius, Coordinate? center = null )
		{
			if( center == null )
			{
				center = Coordinate.Origin();
			}

			if( xRadius == yRadius )
			{
				return Circle( xRadius, center );
			}

			return new EllipseDiagram( xRadius, yRadius, center );
		}

		/// <summary>
		/// A rectangle with sides of length 1
		/// </summary>
		/// <returns></returns>
		public static IDiagram Square() =>
			new RectangleDiagram( 1, 1, Coordinate.Origin() );
	}
}
