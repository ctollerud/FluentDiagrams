using FluentDiagrams.Internal.Transformations;
using FluentDiagrams.Primitives;

namespace FluentDiagrams
{
	public static class LayoutExtensions
	{
		public static IDiagram Scale( this IDiagram diagram, decimal x = 1, decimal y = 1 ) =>
			new ScaledDiagram( diagram, x, y );


		/// <summary>
		/// Reorients the diagram to the bottom of the other diagram and combines them into a single diagram.
		/// </summary>
		public static IDiagram ToTheTopOf( this IDiagram topDiagram, IDiagram bottomDiagram, HorizontalAlignment? alignment = null )
		{
			return
				Composition.ComposedOf(
					topDiagram.OrientToTheTopOf( bottomDiagram, alignment ),
					bottomDiagram
				);
		}

		/// <summary>
		/// Reorients the diagram to the bottom of the other diagram and combines them into a single diagram.
		/// </summary>
		public static IDiagram ToTheBottomOf( this IDiagram bottomDiagram, IDiagram topDiagram, HorizontalAlignment? alignment = null )
		{
			return
				Composition.ComposedOf(
					bottomDiagram.OrientToTheBottomOf( topDiagram, alignment ),
					topDiagram
				);
		}


		/// <summary>
		/// Reorients the diagram to the left of the other diagram and combines them into a single diagram.
		/// </summary>
		/// <param name="rightDiagram"></param>
		/// <param name="leftDiagram"></param>
		/// <param name="alignment"></param>
		/// <returns></returns>
		public static IDiagram ToTheLeftOf( this IDiagram leftDiagram, IDiagram rightDiagram, VerticalAlignment? alignment = null )
		{
			return
				Composition.ComposedOf(
					leftDiagram.OrientToTheLeftOf( rightDiagram, alignment ),
					rightDiagram
				);
		}

		/// <summary>
		/// Reorients the diagram to the right of the other diagram and combines them into a single diagram.
		/// </summary>
		/// <param name="rightDiagram"></param>
		/// <param name="leftDiagram"></param>
		/// <param name="alignment"></param>
		/// <returns></returns>
		public static IDiagram ToTheRightOf( this IDiagram rightDiagram, IDiagram leftDiagram, VerticalAlignment? alignment = null )
		{
			return
				Composition.ComposedOf(
					rightDiagram.OrientToTheRightOf( leftDiagram, alignment ),
					leftDiagram
				);
		}


		/// <summary>
		/// Re-orient the diagram so that it's positioned to the left of the provided diagram
		/// </summary>
		/// <param name="leftDiagram">the diagram to move</param>
		/// <param name="rightDiagram">the diagram to compare to</param>
		/// <param name="alignment">how the diagram should get realigned vertically, if at all.</param>
		/// <returns></returns>
		public static IDiagram OrientToTheLeftOf( this IDiagram leftDiagram, IDiagram rightDiagram, VerticalAlignment? alignment = null )
		{
			var leftDiagramRightEdge = leftDiagram.Bounds.XMax;
			var rightDiagramLeftEdge = rightDiagram.Bounds.XMin;

			var vector = new Vector( rightDiagramLeftEdge - leftDiagramRightEdge, 0 )
				.Add( ComputeAlignment( leftDiagram, rightDiagram, alignment, null ) );

			return leftDiagram.Offset( vector.Dx, vector.Dy );
		}

		/// <summary>
		/// Compute a vector that would move a diagram into alignment with the other diagram.
		/// </summary>
		/// <param name="diagramToMove"></param>
		/// <param name="diagramToCompare"></param>
		/// <param name="verticalAlignment"></param>
		/// <param name="horizontalAlignment"></param>
		/// <returns></returns>
		private static Vector ComputeAlignment(
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

		/// <summary>
		/// Re-orient the diagram so that it's positioned to the right of the provided diagram
		/// </summary>
		/// <param name="rightDiagram">the diagram to move</param>
		/// <param name="leftDiagram">the diagram to compare to</param>
		/// <param name="alignment">how the diagram should get realigned vertically, if at all.</param>
		/// <returns></returns>
		public static IDiagram OrientToTheRightOf( this IDiagram rightDiagram, IDiagram leftDiagram, VerticalAlignment? alignment = null )
		{
			//find the distance between the ride side of the left diagram and the left side of the right diagram
			var rightDiagramLeftEdge = rightDiagram.Bounds.XMin;
			var leftDiagramRightEdge = leftDiagram.Bounds.XMax;

			var vector = new Vector( leftDiagramRightEdge - rightDiagramLeftEdge, 0 )
				.Add( ComputeAlignment( rightDiagram, leftDiagram, alignment, null ) );

			return rightDiagram.Offset( vector.Dx, vector.Dy );
		}


		/// <summary>
		/// Re-orient the diagram so that it's positioned to the top of the provided diagram (y axis )
		/// </summary>
		/// <param name="topDiagram">the diagram to move</param>
		/// <param name="bottomDiagram">the diagram to compare to</param>
		/// <param name="alignment">how the diagram should get realigned horizontally, if at all.</param>
		/// <returns></returns>
		public static IDiagram OrientToTheTopOf( this IDiagram topDiagram, IDiagram bottomDiagram, HorizontalAlignment? alignment = null )
		{
			var topDiagramBottomEdge = topDiagram.Bounds.YMin;

			var bottomDiagramTopEdge = bottomDiagram.Bounds.YMax;

			var vector = new Vector( 0, bottomDiagramTopEdge - topDiagramBottomEdge )
				.Add(
					ComputeAlignment(
						topDiagram,
						bottomDiagram,
						null,
						alignment ) );

			return topDiagram.Offset( vector.Dx, vector.Dy );
		}

		/// <summary>
		/// Re-orient the diagram so that it's positioned to the top of the provided diagram  (y axis )
		/// </summary>
		/// <param name="bottomDiagram">the diagram to move</param>
		/// <param name="topDiagram">the diagram to compare to</param>
		/// <param name="alignment">how the diagram should get realigned horizontally, if at all.</param>
		/// <returns></returns>
		public static IDiagram OrientToTheBottomOf( this IDiagram bottomDiagram, IDiagram topDiagram, HorizontalAlignment? alignment = null )
		{
			var bottomDiagramTopEdge = bottomDiagram.Bounds.YMax;
			var topDiagramBottomEdge = topDiagram.Bounds.YMin;

			var vector = new Vector( 0, topDiagramBottomEdge - bottomDiagramTopEdge )
				.Add(
					ComputeAlignment(
						bottomDiagram,
						topDiagram,
						null,
						alignment ) );

			return bottomDiagram.Offset( vector.Dx, vector.Dy );
		}

		/// <summary>
		/// Just a temporary placeholder.  In the future, this will call into a method 
		/// on the interface
		/// </summary>
		/// <param name="diagram"></param>
		/// <param name="translateX"></param>
		/// <param name="translateY"></param>
		/// <returns></returns>
		public static IDiagram Offset( this IDiagram diagram, decimal translateX, decimal translateY ) =>
			WithOffset( diagram, new Vector( translateX, translateY ) );

		public static IDiagram Offset( this IDiagram diagram, Vector vector ) =>
			Offset( diagram, vector.Dx, vector.Dy );

		public static IDiagram WithOffset( this IDiagram diagram, decimal translateX, decimal translateY ) =>
			WithOffset( diagram, new Vector( translateX, translateY ) );

		public static IDiagram WithOffset( this IDiagram diagram, Vector vector ) =>
			new OffsetDiagram( diagram, vector );




		public static IDiagram RotateAbout( this IDiagram diagram, Coordinate coordinate, Angle angle )
		{
			var newOrigin = diagram.Bounds.Center().RotateAbout( coordinate, angle );

			var offsetVector = Vector.FromCoordinates( coordinate, newOrigin );

			return diagram.WithOffset( offsetVector );
		}
	}
}
