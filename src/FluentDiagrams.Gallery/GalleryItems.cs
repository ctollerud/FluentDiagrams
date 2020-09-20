using System.Collections.Generic;

namespace FluentDiagrams.Gallery
{
	public static class GalleryItems
	{
		public static IEnumerable<GalleryItem> RegressionList() =>
			new[]
			{
				new GalleryItem( "Circle", "literally just a circle.  Most simple diagram conceivable.", () => Shapes.Circle() )
			};
	}
}
