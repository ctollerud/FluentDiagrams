using FluentDiagrams.Gradients;
using FluentDiagrams.Internal;
using FluentDiagrams.Primitives;
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

		public static IDiagram WithLinearGradient( this IDiagram diagram, Color start, Color end ) =>
			WithLinearGradient( diagram, start, end, Angle.Zero );

		public static IDiagram WithLinearGradient( this IDiagram diagram, Color start, Color end, Angle angle ) =>
			new StyleDecorator( diagram, new GradientFillProperty( new LinearGradient( start, end, angle ) ) );

		public static IDiagram WithRadialGradient( this IDiagram diagram, Color centerColor, Color edgeColor ) =>
			new StyleDecorator( diagram, new GradientFillProperty( new RadialGradient( centerColor, edgeColor ) ) );

	}
}
