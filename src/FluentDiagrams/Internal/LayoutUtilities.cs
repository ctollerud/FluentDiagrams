using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Internal
{
	internal static class LayoutUtilities
	{
		/// <summary>
		/// Compute a vector that would move a bounding box into alignment with another bounding box.
		/// </summary>
		/// <param name="diagramToMove"></param>
		/// <param name="diagramToCompare"></param>
		/// <param name="verticalAlignment"></param>
		/// <param name="horizontalAlignment"></param>
		/// <returns></returns>
		public static Vector ComputeAlignment(
			BoundingBox boundsToMove,
			BoundingBox boundsToCompare,
			VerticalAlignment? verticalAlignment,
			HorizontalAlignment? horizontalAlignment )
		{
			var verticalOffset =
				verticalAlignment switch
				{
					VerticalAlignment.Bottom => boundsToCompare.YMin - boundsToMove.YMin,
					VerticalAlignment.Top => boundsToCompare.YMax - boundsToMove.YMax,
					VerticalAlignment.Center => boundsToCompare.Center().Y - boundsToMove.Center().Y,
					_ => 0M
				};

			var horizontalOffset =
				horizontalAlignment switch
				{
					HorizontalAlignment.Left => boundsToCompare.XMin - boundsToMove.XMin,
					HorizontalAlignment.Right => boundsToCompare.XMax - boundsToMove.XMax,
					HorizontalAlignment.Center => boundsToCompare.Center().X - boundsToMove.Center().X,
					_ => 0M
				};
			return new Vector( horizontalOffset, verticalOffset );
		}

		/// <summary>
		/// Compute a vector that would move a diagram into alignment with the other diagram.
		/// </summary>
		/// <param name="diagramToMove"></param>
		/// <param name="diagramToCompare"></param>
		/// <param name="verticalAlignment"></param>
		/// <param name="horizontalAlignment"></param>
		/// <returns></returns>
		public static Vector ComputeAlignment(
			IDiagram diagramToMove,
			IDiagram diagramToCompare,
			VerticalAlignment? verticalAlignment,
			HorizontalAlignment? horizontalAlignment ) =>
			ComputeAlignment(
				diagramToMove.Bounds,
				diagramToCompare.Bounds,
				verticalAlignment,
				horizontalAlignment );
	}
}
