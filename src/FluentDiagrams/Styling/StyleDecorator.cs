using FluentDiagrams.Primitives;
using FluentDiagrams.StyleProperties;

namespace FluentDiagrams.Styling
{
	/// <summary>
	/// Represents a diagram with some styling applied.
	/// </summary>
	public class StyleDecorator : IDiagram
	{
		public StyleDecorator( IDiagram diagram, IStyleProperty property )
		{
			Diagram = diagram;
			Property = property;
		}

		public BoundingBox Bounds => Diagram.Bounds;

		public IDiagram Diagram { get; }
		public IStyleProperty Property { get; }

		public IDiagram DeepRotate( Coordinate coordinate, Angle angle ) =>
			new StyleDecorator( Diagram.DeepRotate( coordinate, angle ), Property );
	}
}
