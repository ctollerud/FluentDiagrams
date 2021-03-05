using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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

		public static IDiagram WithBackground( this IDiagram mask, Func<IDiagram, IDiagram> rectBgStyle )
		{
			var background = Shapes.Square().ScaleTo( mask ).OffsetTo( mask ).Pipe( rectBgStyle );
			return background.Then( mask );
		}

		public static IDiagram ClipToSizeOf( this IDiagram diagram, IDiagram sizeProvider )
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
				return new BoundingBoxOverridingDiagram( diagram, newBoundingBox );
			}


		}

		/// <summary>
		/// Scale down the size of the diagram to something else, by cutting off edges
		/// </summary>
		/// <param name="input"></param>
		/// <param name="scaleX"></param>
		/// <param name="scaleY"></param>
		/// <returns></returns>
		public static IDiagram Trim( this IDiagram input, decimal scaleX, decimal scaleY )
		{
			var square = Shapes.Square().WithFill( Color.White ).ScaleTo( input ).OffsetTo( input ).Scale( scaleX, scaleY );

			return square.AsMaskOver( input );
		}

		/// <summary>
		/// expand the diagrams margins so that it fits the specified width/height
		/// </summary>
		/// <param name="diagram"></param>
		/// <param name="widthOverHeight"></param>
		/// <returns></returns>
		public static IDiagram MarginToAspectRatio( this IDiagram diagram, decimal widthOverHeight )
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
