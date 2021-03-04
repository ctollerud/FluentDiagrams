using FluentDiagrams.Internal;
using FluentDiagrams.Internal.Shapes;
using FluentDiagrams.Internal.Transformations;
using FluentDiagrams.Paths;
using FluentDiagrams.Primitives;
using FluentDiagrams.Svg.Internal;
using LinqGarden;
using LinqGarden.Functions;
using LinqGarden.Enumerables;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

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
				from inputDiagram in Fallible.Success<string, IDiagram>( diagram )
				let boundingBox = inputDiagram.Bounds
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
				CompositeDiagram x => RenderCompositeDiagram( x ),
				ScaledDiagram x => RenderScaled( x ),
				OffsetDiagram x => RenderOffsetDiagram( x ),
				StyleDecorator x => RenderStyleDecorator( x ),
				BoundingBoxOverridingDiagram x => RenderSvg( x.Diagram ),
				RectangleDiagram x => RenderRectangle( x ),
				PolygonDiagram x => RenderPolygon( x ),
				DefinedDiagram x => RenderDefined( x ),
				WhitelistMask x => RenderWhitelistMask( x ),
				RotatedDiagram x => RenderRotated( x ),
				PathDiagram x => RenderPathDiagram( x ),
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

		private static XElementBuilder AddSvgTransform( XElementBuilder builder, string transformInstruction ) =>
			builder.AddOrModifyAttribute( "transform",
				 () => transformInstruction,
				 oldValue => $"{transformInstruction} {oldValue}" );


		/// <summary>
		/// The complexity of this method has to do with 
		/// how svg handles scaling, compared to how we want to handle it.
		/// SVG scales the diagram with respect to the origin (affecting its offset in the process).
		/// </summary>
		/// <param name="decorator"></param>
		/// <returns></returns>
		private static State<SvgDrawState, XElementBuilder> RenderScaled( ScaledDiagram decorator ) =>
			from drawState in State.Get<SvgDrawState>()
			let xSign = decorator.X < 0 ? -1 : 1
			let ySign = decorator.Y < 0 ? -1 : 1
			let converter = drawState.Converter
			let preScaleTransform =
				new Vector(
					converter.BoundingBox.XMin - decorator.Diagram.Bounds.XMin,
					converter.BoundingBox.YMax - decorator.Diagram.Bounds.YMax )
			let offsetDiagram = decorator.Diagram.WithOffset( preScaleTransform )
			let postScaleTransformCenter =
				new BoundingBox(
					xMin: offsetDiagram.Bounds.XMin * xSign,
					xMax: ( offsetDiagram.Bounds.XMin * xSign ) + decorator.Bounds.Width,
					yMin: offsetDiagram.Bounds.YMax - decorator.Bounds.Height,
					yMax: offsetDiagram.Bounds.YMax )

				//if the Y-scale is negative, then we need to "flip" over its axis
				.Pipe(
					box => ySign == 1
					? box
					: BoundingBox.Create( box.Width, box.Height, box.Center()
					  .Translate(
						new Vector(
							0,
							( converter.BoundingBox.YMax - box.Center().Y ) * 2 )
						)
					  )
				)
				.Center()
				.Pipe( coord => Coordinate.Cartesian( coord.X * xSign, coord.Y ) )
			let postScaleSvgVector =
				Vector.FromCoordinates(
					postScaleTransformCenter,
					decorator.Bounds.Center() )
				.Pipe( converter.ToSvgVector )

			//Render the input diagram, only offset to the origin.
			from subElement in RenderSvg( offsetDiagram )

				//Once the scaling has been applied, we can offset it back to its desired position again.
			select AddSvgTransform( subElement, $"translate({postScaleSvgVector.Dx},{postScaleSvgVector.Dy}) scale({decorator.X},{decorator.Y})" );

		private static State<SvgDrawState, XElementBuilder> RenderOffsetDiagram( OffsetDiagram offset ) =>
			from drawState in State.Get<SvgDrawState>()
			let vector = offset.Vector.Pipe( drawState.Converter.ToSvgVector )
			from subElement in RenderSvg( offset.InnerDiagram )
			select AddSvgTransform( subElement, $"translate({vector.Dx},{vector.Dy})" );

		private static State<SvgDrawState, XElementBuilder> RenderCompositeDiagram( CompositeDiagram composite )
		{
			IEnumerable<State<SvgDrawState, XElementBuilder>> composedDiagrams = composite.Diagrams.Select( RenderSvg );

			State<SvgDrawState, ICollection<XElementBuilder>> concatenated = State.Concat( composedDiagrams );

			return
				from protoElements in concatenated
				select
					XElementBuilder.WithName( "g" )
					.Add( protoElements.Select( x => x.Build() ) );
		}

		private static State<SvgDrawState, XElementBuilder> RenderStyleDecorator( StyleDecorator decorator ) =>
			from svg in RenderSvg( decorator.Diagram )
			from styleAttributes in SvgStyleFactory.GenerateStyleAttributes( decorator.Property )
			select
				XElementBuilder.WithName( "g" )
				.Add( svg.Build() )
				.Add( styleAttributes );

		private static State<SvgDrawState, XElementBuilder> RenderRectangle( RectangleDiagram rectangle ) =>
			from drawState in State.Get<SvgDrawState>()
			let converter = drawState.Converter
			let svgCoord =
				rectangle.Bounds.TopLeft
				.Pipe( converter.ToSvgCoord )
			let width = converter.ScaleDistance( rectangle.Width )
			let height = converter.ScaleDistance( rectangle.Height )
			select
				XElementBuilder.WithName( "rect" ).Add(
					new XAttribute( "x", svgCoord.X ),
					new XAttribute( "y", svgCoord.Y ),
					new XAttribute( "width", width ),
					new XAttribute( "height", height )
					);

		private static State<SvgDrawState, XElementBuilder> RenderPolygon( PolygonDiagram polygon ) =>
			from drawState in State.Get<SvgDrawState>()
			select
				XElementBuilder.WithName( "polygon" ).Add(
					new XAttribute( "points",
						polygon.Coordinates
						.Select( drawState.Converter.ToSvgCoord )
						.Select( coord => $"{coord.X},{coord.Y}" )
						.Pipe( x => string.Join( " ", x ) )
						) );

		private static State<SvgDrawState, XElementBuilder> RenderDefined( DefinedDiagram defined ) =>
			from definitionId in IncludeDefinition( defined, RenderSvg( defined.Diagram ) )
			select XElementBuilder.WithName( "use" ).Add(
				new XAttribute( "href", $"#{definitionId}" ) );

		private static State<SvgDrawState, string> IncludeDefinition( object key, State<SvgDrawState, XElementBuilder> definitionRenderer ) =>
			from drawState in State.Get<SvgDrawState>()
			from definitionKey in drawState.IncludeDefinitionInternal( key, definitionRenderer )
			select definitionKey;

		private static State<SvgDrawState, XElementBuilder> RenderWhitelistMask( WhitelistMask whitelistMask ) =>
			from maskeeSvg in RenderSvg( whitelistMask.Maskee )
			let maskFactory = BuildMask( whitelistMask.Mask )
			from maskId in IncludeDefinition( whitelistMask, maskFactory )
			select
				XElementBuilder.WithName( "g" ).Add(
					new XAttribute( "mask", $"url(#{maskId})" ) )
					.Add( maskeeSvg.Build() );

		private static State<SvgDrawState, XElementBuilder> BuildMask( IDiagram mask ) =>
			from body in RenderSvg( mask )
			select XElementBuilder.WithName( "mask" ).Add( body.Build() );

		private static State<SvgDrawState, XElementBuilder> RenderRotated( RotatedDiagram rotatedDiagram ) =>
			from drawState in State.Get<SvgDrawState>()
			let rotationOriginSvg = rotatedDiagram.RotationOrigin.Pipe( drawState.Converter.ToSvgCoord )
			let degrees = rotatedDiagram.Angle.Reverse().Degrees
			from subElement in RenderSvg( rotatedDiagram.Diagram )
			select AddSvgTransform( subElement, $"rotate({degrees},{rotationOriginSvg.X},{rotationOriginSvg.Y})" );

		private static State<SvgDrawState, XElementBuilder> RenderPathDiagram( PathDiagram pathDiagram ) =>
			from state in State.Get<SvgDrawState>()
			let converter = state.Converter
			select
				XElementBuilder.WithName( "path" )
				.Add(
					new XAttribute( "stroke-width", state.Converter.ScaleDistance( pathDiagram.StrokeWidth ) ),
					new XAttribute( "d", GenerateSegment( pathDiagram.Instructions, converter ) ),
					new XAttribute( "stroke-linecap", "round" ),
					new XAttribute( "fill", "transparent" )
				);

		private static string GenerateSegment( PathInstructions instructions, CoordinatesConverter converter )
		{
			var startPosition = instructions.StartLocation.Pipe( converter.ToSvgCoord );

			return
				instructions.Instructions
				.Select( x => GeneratePathInstruction( x, converter ) )
				.StartWith( $"M {startPosition.X} {startPosition.Y}" )
				.Pipe( x => string.Join( " ", x ) );
		}

		private static string GeneratePathInstruction( IPathInstruction instruction, CoordinatesConverter converter )
		{
			if( instruction is MoveInstruction moveInstruction )
			{
				var coordinate = converter.ToSvgCoord( moveInstruction.MoveTo );

				return $"L {coordinate.X} {coordinate.Y}";
			}
			if( instruction is CubicPathInstruction cubicInstruction )
			{
				var svgEndPosition = cubicInstruction.EndPosition.Pipe( converter.ToSvgCoord );
				var svgControl1 = cubicInstruction.ControlPoint1.Pipe( converter.ToSvgCoord );
				var svgControl2 = cubicInstruction.ControlPoint2.Pipe( converter.ToSvgCoord );

				return $"C {svgControl1.X} {svgControl1.Y}, {svgControl2.X} {svgControl2.Y}, {svgEndPosition.X} {svgEndPosition.Y}";
			}
			throw new Exception( $" unable to handle path instruction {nameof( IPathInstruction )} of type {instruction.GetType().Name}" );
		}

	}
}
