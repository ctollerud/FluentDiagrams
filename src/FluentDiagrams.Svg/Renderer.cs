using FluentDiagrams.Primitives;
using FluentDiagrams.Svg.Internal;
using LinqGarden;
using LinqGarden.Functions;
using System;
using System.Linq;
using System.Xml.Linq;

namespace FluentDiagrams.Svg
{
	public class Renderer
	{
		/// <summary>
		/// Generates an SVG element containing the diagram
		/// </summary>
		/// <param name="diagram">the diagram to render</param>
		/// <param name="internalToSvgScaling">the "internal" to SVG scaling ratio.</param>
		/// <returns></returns>
		public static Fallible<string, XElement> RenderSvgElement(
			IDiagram diagram,
			int internalToSvgScaling = 10000 )
		{
			return
				from boundingBox in diagram.Bounds.IfNoneFail( "Diagram has no dimensions, and is therefore impossible to render." )
				let converter = new CoordinatesConverter( boundingBox, internalToSvgScaling )
				let drawState = new SvgDrawState( converter )
				let renderingResult = RenderSvg( diagram ).Func( drawState )
				let mainElement = renderingResult.Value
				let finalDrawState = renderingResult.State
				let svgElements = finalDrawState.BuildDeclarations()
					.Concat( new[] { mainElement.Build() } )
				select new XElement( "svg",
						new XAttribute( "stroke", "black" ), // provide a reasonable default stroke
						new XAttribute( "stroke-width", "0" ), // no stroke width.  Those that need it will set it explicitly.
						new XAttribute( "viewbox", $"0 0 {converter.SvgWidth} {converter.SvgHeight}" ),
						svgElements );
		}
		internal static State<SvgDrawState, XElementBuilder> RenderSvg( IDiagram diagram )
		{
			return diagram switch
			{
				CircleDiagram x => RenderCircle( x ),

				//we should probably gravitate away from this exception in the nearish future.
				_ => throw new DiagramNotRenderableException( diagram )
			};
		}

		private static State<SvgDrawState, XElementBuilder> RenderCircle( CircleDiagram circle ) =>
			from state in State.Get<SvgDrawState>()
			let svgUnitsRadius = state.Converter.ScaleDistance( circle.Radius )
			let origin = circle.Origin.Pipe( state.Converter.ToSvgCoord )
			select
				XElementBuilder.WithName( "circle" )
				.Add(
					new XAttribute( "cx", origin.X ),
					new XAttribute( "cy", origin.Y ),
					new XAttribute( "r", svgUnitsRadius ) );
	}
}
