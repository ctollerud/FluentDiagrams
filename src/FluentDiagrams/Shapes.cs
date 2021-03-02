using FluentDiagrams.Internal.Shapes;
using FluentDiagrams.Primitives;

namespace FluentDiagrams
{
	public static class Shapes
	{
		/// <summary>
		/// A circle with a radius of 0.5
		/// </summary>
		/// <returns></returns>
		public static IDiagram Circle() => CircleDiagram.Default;

		/// <summary>
		/// A rectangle with sides of length 1
		/// </summary>
		/// <returns></returns>
		public static IDiagram Square() =>
			new RectangleDiagram( 1, 1, Coordinate.Origin() );
	}
}
