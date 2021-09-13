using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using FluentDiagrams.Internal;
using FluentDiagrams.Internal.Transformations;
using LinqGarden.Functions;

namespace FluentDiagrams
{
	/// <summary>
	/// Houses higher-level composition methods
	/// </summary>
	public static class PresentationExtensions
	{
		public static IDiagram ScaleUpToAspectRatio( this IDiagram diagram, decimal widthOverHeight )
		{
			var currentBoundingBox = diagram.Bounds;
			BoundingBox newBoundingBox = ScaleUpToAspectRatio( diagram.Bounds, widthOverHeight );


			if( (currentBoundingBox.Width, currentBoundingBox.Height) == (newBoundingBox.Width, newBoundingBox.Height) )
			{
				return diagram;
			}
			else
			{
				return diagram.Scale( x: newBoundingBox.Width / currentBoundingBox.Width, y: newBoundingBox.Height / currentBoundingBox.Height );
			}
		}

		private static BoundingBox ScaleUpToAspectRatio( BoundingBox boundingBox, decimal widthOverHeight )
		{
			decimal currentAspectRatio = boundingBox.Width / boundingBox.Height;
			if( widthOverHeight == currentAspectRatio )
			{
				return boundingBox;
			}
			else if( widthOverHeight > currentAspectRatio )
			{
				//the target aspect ratio is wider than the current, so we need to expand the diagram horizontally.
				var widthExpansionAmount = widthOverHeight / currentAspectRatio;
				return BoundingBox.Create( widthExpansionAmount * boundingBox.Width, boundingBox.Height, boundingBox.Center() );
			}
			else// widthOverHeight < currentAspectRatio
			{
				// the target aspect ratio is narrower than the current, So we need to expand the diagram vertically
				var heightExpansionAmount = currentAspectRatio / widthOverHeight;
				return BoundingBox.Create( boundingBox.Width, boundingBox.Height * heightExpansionAmount, boundingBox.Center() );
			}
		}

		private static BoundingBox ScaleDownToAspectRatio(
			BoundingBox boundingBox,
			decimal widthOverHeight,
			HorizontalAlignment horizontalAlignment,
			VerticalAlignment verticalAlignment )
		{
			decimal oldWidthOverHeight = boundingBox.Width / boundingBox.Height;
			if( widthOverHeight == oldWidthOverHeight )
			{
				return boundingBox;
			}

			BoundingBox newBoundingBox;

			if( widthOverHeight > oldWidthOverHeight )
			{
				//the target aspect ratio is wider than the current, so we need to contract vertically
				var heightContractionAmount = widthOverHeight / oldWidthOverHeight;
				newBoundingBox = BoundingBox.Create( boundingBox.Width, boundingBox.Height / heightContractionAmount, boundingBox.Center() );
			}
			else// widthOverHeight < oldWidthOverHeight
			{
				// the target aspect ratio is narrower than the current, So we need to contract the diagram horizontally
				var widthContractionAmount = oldWidthOverHeight / widthOverHeight;
				newBoundingBox = BoundingBox.Create( boundingBox.Width / widthContractionAmount, boundingBox.Height, boundingBox.Center() );
			}

			return newBoundingBox.Offset( LayoutUtilities.ComputeAlignment( newBoundingBox, boundingBox, verticalAlignment, horizontalAlignment ) );
		}

		/// <summary>
		/// Scale the diagram to the dimensions of another.
		/// </summary>
		/// <param name="diagram"></param>
		/// <param name="scaleProvider"></param>
		/// <returns></returns>
		public static IDiagram ScaleTo( this IDiagram diagram, IDiagram scaleProvider )
		{
			if( (diagram.Bounds.Width, diagram.Bounds.Height) == (scaleProvider.Bounds.Width, scaleProvider.Bounds.Height) )
			{
				return diagram;
			}
			else
			{
				return diagram.Scale( x: scaleProvider.Bounds.Width / diagram.Bounds.Width, y: scaleProvider.Bounds.Height / diagram.Bounds.Height );
			}
		}

		public static IDiagram WithBackground( this IDiagram foreground, Func<IDiagram, IDiagram> rectBgStyle )
		{
			var background = Shapes.Square().ScaleTo( foreground ).OffsetTo( foreground ).Pipe( rectBgStyle );
			return background.Then( foreground );
		}

		public static IDiagram WithBackground( this IDiagram mask, Color backgroundColor ) =>
			mask.WithBackground( x => x.WithFill( backgroundColor ) );

		public static IDiagram CropToAspectRatio(
			this IDiagram diagram,
			decimal aspectRatio,
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment verticalAlignment = VerticalAlignment.Center
			)
		{
			var newBounds = ScaleDownToAspectRatio( diagram.Bounds, aspectRatio, horizontalAlignment, verticalAlignment );

			return Crop( diagram, newBounds );
		}

		public static IDiagram CropToSizeOf(
			this IDiagram diagram,
			IDiagram sizeProvider,
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment verticalAlignment = VerticalAlignment.Center )
		{
			var boundingBoxes = new[] { diagram.Bounds, sizeProvider.Bounds };
			var newBoundingBox =
				BoundingBox.Create(
					boundingBoxes.Min( x => x.Width ),
					boundingBoxes.Min( x => x.Height ),
					diagram.Bounds.Center() );


			if( (diagram.Bounds.Width, diagram.Bounds.Height) == (newBoundingBox.Width, newBoundingBox.Height) )
			{
				return diagram;
			}
			else
			{
				return Crop( diagram, newBoundingBox.Offset( LayoutUtilities.ComputeAlignment(
					newBoundingBox,
					diagram.Bounds,
					verticalAlignment,
					horizontalAlignment ) ) );
			}


		}

		/// <summary>
		/// Scale up/down the size of the diagram to something else, by either clipping or
		/// adding margins.
		/// 
		/// The resulting diagram will have a rectangular envelope.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="scaleX"></param>
		/// <param name="scaleY"></param>
		/// <param name="horizontalAlignment">How the cropping should align with the original horizontally</param>
		/// <param name="verticalAlignment">How the cropping should align with the original vertically</param>
		/// <returns></returns>
		public static IDiagram CropScale( this IDiagram input, decimal scaleX, decimal scaleY, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment )
		{

			var oldBounds = input.Bounds;
			var scaledBounds = BoundingBox.Create( oldBounds.Width * scaleX, oldBounds.Height * scaleY );

			var translation = LayoutUtilities.ComputeAlignment( scaledBounds, oldBounds, verticalAlignment, horizontalAlignment );

			var newBounds = scaledBounds.Offset( translation );
			return Crop( input, newBounds );

		}

		private static IDiagram Crop( this IDiagram input, BoundingBox newBounds )
		{
			return new ViewboxDiagram(
				input,
				newBounds,
				xMin: newBounds.XMin,
				yMin: newBounds.YMin );
		}

		/// <summary>
		/// Scale up/down the size of the diagram to something else, by either clipping or
		/// adding margins.
		/// 
		/// The resulting diagram will have a rectangular envelope.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="scaleX"></param>
		/// <param name="scaleY"></param>
		/// <param name="horizontalAlignment">How the cropping should align with the original horizontally</param>
		/// <returns></returns>
		public static IDiagram CropScale( this IDiagram input, decimal scaleX, decimal scaleY, HorizontalAlignment horizontalAlignment ) =>
			CropScale( input, scaleX, scaleY, horizontalAlignment, VerticalAlignment.Center );

		/// <summary>
		/// Scale up/down the size of the diagram to something else, by either clipping or
		/// adding margins.
		/// 
		/// The resulting diagram will have a rectangular envelope.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="scaleX"></param>
		/// <param name="scaleY"></param>
		/// <param name="verticalAlignment">How the cropping should align with the original vertically</param>
		/// <returns></returns>
		public static IDiagram CropScale( this IDiagram input, decimal scaleX, decimal scaleY, VerticalAlignment verticalAlignment ) =>
			CropScale( input, scaleX, scaleY, HorizontalAlignment.Center, verticalAlignment );

		/// <summary>
		/// Scale up/down the size of the diagram to something else, by either clipping or
		/// adding margins.
		/// 
		/// The resulting diagram will have a rectangular envelope with the same center as the original.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="scaleX"></param>
		/// <param name="scaleY"></param>
		/// <returns></returns>
		public static IDiagram CropScale( this IDiagram input, decimal scaleX, decimal scaleY ) =>
			CropScale( input, scaleX, scaleY, HorizontalAlignment.Center, VerticalAlignment.Center );

		/// <summary>
		/// expand the diagram's bounding box so that it fits the specified width/height
		/// </summary>
		/// <param name="diagram"></param>
		/// <param name="widthOverHeight"></param>
		/// <returns></returns>
		public static IDiagram ExpandToAspectRatio( this IDiagram diagram, decimal widthOverHeight )
		{
			var currentBoundingBox = diagram.Bounds;
			BoundingBox newBoundingBox = ScaleUpToAspectRatio( diagram.Bounds, widthOverHeight );


			if( (currentBoundingBox.Width, currentBoundingBox.Height) == (newBoundingBox.Width, newBoundingBox.Height) )
			{
				return diagram;
			}
			else
			{
				return new BoundingBoxOverridingDiagram( diagram, newBoundingBox );
			}
		}
	}
}
