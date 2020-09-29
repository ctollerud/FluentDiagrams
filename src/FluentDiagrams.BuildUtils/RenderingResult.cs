using FluentDiagrams.Gallery;
using LinqGarden;

namespace FluentDiagrams.BuildUtils
{
	public class RenderingResult
	{
		public RenderingResult( GalleryItem item, Maybe<string> issue )
		{
			this.Item = item;
			this.Issue = issue;
		}

		public GalleryItem Item { get; }
		public Maybe<string> Issue { get; }
	}
}
