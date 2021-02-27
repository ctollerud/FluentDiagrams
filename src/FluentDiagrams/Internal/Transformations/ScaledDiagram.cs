using FluentDiagrams.Primitives;

namespace FluentDiagrams.Internal.Transformations
{
	public class ScaledDiagram : IDiagram
	{
		internal ScaledDiagram( IDiagram innerDiagram, decimal x = 1, decimal y = 1 )
		{
			X = x;
			Y = y;
			Diagram = innerDiagram;
			Bounds = innerDiagram.Bounds.Scale( x, y );
		}
		public BoundingBox Bounds { get; }
		public decimal X { get; }
		public decimal Y { get; }
		public IDiagram Diagram { get; }

		/// <summary>
		/// TODO: we can we improve this implementation?
		/// </summary>
		/// <param name="coordinate"></param>
		/// <param name="angle"></param>
		/// <returns></returns>
		public IDiagram Rotate( Angle angle ) =>
			new RotatedDiagram( this, angle, Bounds.Center() );
	}
}
