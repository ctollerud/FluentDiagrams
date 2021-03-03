using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Internal
{
	internal static class LayoutUtilities
	{
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
			HorizontalAlignment? horizontalAlignment )
		{
			var verticalOffset =
				verticalAlignment switch
				{
					VerticalAlignment.Bottom => diagramToCompare.Bounds.YMin - diagramToMove.Bounds.YMin,
					VerticalAlignment.Top => diagramToCompare.Bounds.YMax - diagramToMove.Bounds.YMax,
					VerticalAlignment.Center => diagramToCompare.Bounds.Center().Y - diagramToMove.Bounds.Center().Y,
					_ => 0M
				};

			var horizontalOffset =
				horizontalAlignment switch
				{
					HorizontalAlignment.Left => diagramToCompare.Bounds.XMin - diagramToMove.Bounds.XMin,
					HorizontalAlignment.Right => diagramToCompare.Bounds.XMax - diagramToMove.Bounds.XMax,
					HorizontalAlignment.Center => diagramToCompare.Bounds.Center().X - diagramToMove.Bounds.Center().X,
					_ => 0M
				};
			return new Vector( horizontalOffset, verticalOffset );
		}
	}
}
