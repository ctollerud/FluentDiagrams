using FluentDiagrams.Internal;
using FluentDiagrams.StyleProperties;
using FluentDiagrams.Styling;
using System.Drawing;

namespace FluentDiagrams
{
	public static class StylingExtensions
	{
		public static IDiagram WithFill( this IDiagram diagram, Color color ) =>
			new StyleDecorator( diagram, new FillProperty( color ) );

		public static IDiagram WithFillPattern( this IDiagram diagram, IDiagram patternDiagram )
		{
			var fillWidthRatio = patternDiagram.Bounds.Width / diagram.Bounds.Width;
			var fillHeightRatio = patternDiagram.Bounds.Height / diagram.Bounds.Height;

			return new StyleDecorator( diagram, new FillPatternProperty( patternDiagram, fillWidthRatio, fillHeightRatio ) );
		}

		public static IDiagram WithSvgStroke( this IDiagram diagram, decimal strokeWidth ) =>
			new StyleDecorator( diagram, new StrokeWidthProperty( new SvgStrokeWidth( strokeWidth ) ) );

		public static IDiagram WithStrokeColor( this IDiagram diagram, Color strokeColor ) =>
			new StyleDecorator( diagram, new StrokeColorProperty( strokeColor ) );
	}
}
