using FluentDiagrams.Gradients;
using FluentDiagrams.Primitives;
using FluentDiagrams.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LinqGarden.Functions;
using static FluentDiagrams.Svg.Internal.SvgColorFactory;
using FluentDiagrams.StyleProperties;
using LinqGarden;

namespace FluentDiagrams.Svg.Internal
{
	internal static class SvgStyleFactory
	{
		public static State<SvgDrawState, IEnumerable<XAttribute>> GenerateStyleAttributes( IStyleProperty styleProperty )
		{
			if( styleProperty is FillProperty fillProperty )
			{
				return new[]
				{
					new XAttribute( "fill", SvgColor( fillProperty.FillColor ) ),
					new XAttribute( "fill-opacity", SvgOpacity( fillProperty.FillColor ) ),
				}
				.Pipe( State.Return<SvgDrawState, IEnumerable<XAttribute>> );
			}
			if( styleProperty is GradientFillProperty gradientFillProperty )
			{
				var gradientState = State.Return<SvgDrawState, XElementBuilder>( () => BuildGradientElement( gradientFillProperty.Gradient ) );

				return
					from id in SvgDrawState.IncludeDefinition( gradientFillProperty.Gradient, gradientState )
					select new[] { new XAttribute( "fill", $"url(#{id})" ) }.AsEnumerable();
			}
			if( styleProperty is FillPatternProperty fillPatternProperty )
			{
				var patternFactory = RenderPattern( fillPatternProperty.PatternDiagram, fillPatternProperty.FillWidthRatio, fillPatternProperty.FillHeightRatio );

				return
					from id in SvgDrawState.IncludeDefinition( fillPatternProperty, patternFactory )
					select new[] { new XAttribute( "fill", $"url(#{id})" ) }.AsEnumerable();

			}
			if( styleProperty is FilterProperty filterProperty )
			{
				var filterFactory =
					from drawState in State.Get<SvgDrawState>()
					select SvgFilterFactory.BuildFilter( filterProperty.FilterComponent, drawState.Converter );

				return
					from id in SvgDrawState.IncludeDefinition( filterProperty, filterFactory )
					select new[] { new XAttribute( "filter", $"url(#{id})" ) }.AsEnumerable();
			}
			if( styleProperty is StrokeColorProperty strokeColorProperty )
			{
				return
					new[]
					{
						new XAttribute( "stroke", SvgColor( strokeColorProperty.Color ) ),
						new XAttribute( "stroke-opacity", SvgOpacity( strokeColorProperty.Color ) )
					}
					.AsEnumerable()
					.Pipe( State.Return<SvgDrawState, IEnumerable<XAttribute>> );
			}
			if( styleProperty is StrokeWidthProperty strokeWidthProperty )
			{
				return
					from state in State.Get<SvgDrawState>()
					let svgStrokeWidth = CalculateStrokeWidth( strokeWidthProperty.StrokeWidth, state.Converter )
					select new[]
					{
						new XAttribute( "stroke-width", svgStrokeWidth )
					}.AsEnumerable();
			}

			throw new Exception( $"unable to render fill type of style property {styleProperty.GetType()}" );
		}

		private static XElementBuilder BuildGradientElement( IGradient gradient )
		{
			if( gradient is LinearGradient linearGradient )
			{
				return BuildLinearGradientElement( linearGradient );
			}
			if( gradient is RadialGradient radialGradient )
			{
				return BuildRadialGradient( radialGradient );
			}

			throw new GradientNotSupportedException( gradient.GetType() );
		}

		private static XElementBuilder BuildLinearGradientElement( LinearGradient gradient )
		{
			var startPositionPreTranslate = Coordinate.Cartesian( -50, 0 ).Rotate( gradient.Angle );
			var endPositionPreTranslate = Coordinate.Cartesian( -50, 0 ).Rotate( gradient.Angle.Plus( Angle.FromRotations( 0.5M ) ) );

			var startPosition = startPositionPreTranslate.Translate( new Vector( 50, 50 ) );
			var endPosition = endPositionPreTranslate.Translate( new Vector( 50, 50 ) );

			var startPercentX = startPosition.X.ToString() + "%";
			var startPercentY = 100 - startPosition.Y + "%";

			var endPercentX = endPosition.X.ToString() + "%";
			var endPercentY = 100 - endPosition.Y + "%";

			return
				XElementBuilder.WithName( "linearGradient" ).Add(
								new XAttribute( "x1", startPercentX ),
								new XAttribute( "y1", startPercentY ),
								new XAttribute( "x2", endPercentX ),
								new XAttribute( "y2", endPercentY )
								)
					.Add(
								new XElement( "stop",
									new XAttribute( "offset", "0%" ),
									new XAttribute( "style", $"stop-color:{SvgColor( gradient.Start )};stop-opacity:{SvgOpacity( gradient.Start )}" ) ),
								new XElement( "stop",
									new XAttribute( "offset", "100%" ),
									new XAttribute( "style", $"stop-color:{SvgColor( gradient.End )};stop-opacity:{SvgOpacity( gradient.End )}" ) ) );
		}

		private static XElementBuilder BuildRadialGradient( RadialGradient gradient )
		{
			return XElementBuilder.WithName( "radialGradient" ).Add(
				//TODO: incorporate, fx, fy to affect the focal point( intriguing )
				new XAttribute( "cx", "50%" ),
				new XAttribute( "cy", "50%" ) )
				.Add(
				new XElement( "stop",
					new XAttribute( "stop-color", SvgColor( gradient.CenterColor ) ),
					new XAttribute( "stop-opacity", SvgOpacity( gradient.CenterColor ) ),
					new XAttribute( "offset", "0%" )
				),
				new XElement( "stop",
					new XAttribute( "stop-color", SvgColor( gradient.EdgeColor ) ),
					new XAttribute( "stop-opacity", SvgOpacity( gradient.EdgeColor ) ),
					new XAttribute( "offset", "100%" )
				) );
		}

		private static State<SvgDrawState, XElementBuilder> RenderPattern( IDiagram fillPattern, decimal fillWidthRatio, decimal fillHeightRatio ) =>
			from drawState in State.Get<SvgDrawState>()
			let normalizedDiagram = NormalizeToTopLeftCorner( drawState, fillPattern )
			from diagram in Renderer.RenderSvg( normalizedDiagram )
			select XElementBuilder.WithName( "pattern" ).Add(
				new XAttribute( "x", 0 ),
				new XAttribute( "y", 0 ),

				//calculate this based off the diagram's proportions?
				new XAttribute( "width", fillWidthRatio ),
				new XAttribute( "height", fillHeightRatio ) )
			.Add( diagram.Build() );



		/// <summary>
		/// performs a translation transform on the diagram in order to push it to the "top-left" corner of the diagram.
		/// TODO: might make sense if the scale transform ends up using this as well.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="diagram"></param>
		/// <returns></returns>
		private static IDiagram NormalizeToTopLeftCorner( SvgDrawState state, IDiagram diagram )
		{
			var vector =
				Vector.FromCoordinates(
					diagram.Bounds.TopLeft,
					state.Converter.BoundingBox.TopLeft
					);

			//TODO: once implemented, should this use the more "programmatic" offset approach? (that doesn't rely on the SVG transform?)
			return diagram.WithOffset( vector );
		}

		private static decimal CalculateStrokeWidth( IStrokeWidth strokeWidth, CoordinatesConverter converter )
		{

			if( strokeWidth is SvgStrokeWidth svgStrokeWidth )
			{
				return
					converter.BoundingBox.Width.Pipe( x => x / 100 )
					.Pipe( x => x * svgStrokeWidth.StrokeWidth )
					.Pipe( converter.ScaleDistance );
			}
			throw new Exception( $"stroke width of type '{strokeWidth.GetType()}' can't be rendered" );
		}

	}
}
