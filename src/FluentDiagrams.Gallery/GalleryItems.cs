using FluentDiagrams.Primitives;
using System.Collections.Generic;
using System.Drawing;

namespace FluentDiagrams.Gallery
{
	public static class GalleryItems
	{
		public static IEnumerable<GalleryItem> RegressionList() =>
			new[]
			{
				new GalleryItem( "Circle", "literally just a circle.  Most simple diagram conceivable.", () => Shapes.Circle(0.5M, Coordinate.Origin() ) ),
				new GalleryItem( "Circle", "Testing a ranging of styling", () => Shapes.Circle(0.5M, Coordinate.Origin() ).WithFill( Color.Red ) )
			};
	}
}
