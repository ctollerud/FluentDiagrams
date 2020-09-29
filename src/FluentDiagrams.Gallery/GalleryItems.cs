using System.Collections.Generic;
using System.Drawing;

namespace FluentDiagrams.Gallery
{
	public static class GalleryItems
	{
		public static IEnumerable<GalleryItem> RegressionList() =>
			new[]
			{
				new GalleryItem( "Circle", "literally just a circle.  Most simple diagram conceivable.", () => Shapes.Circle() ),
				new GalleryItem( "Circle", "Testing a ranging of styling", () => Shapes.Circle().WithFill( Color.Red ) )
			};
	}
}
