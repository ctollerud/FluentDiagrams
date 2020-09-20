using System;
using System.Linq.Expressions;

namespace FluentDiagrams.Gallery
{
	public class GalleryItem
	{
		public GalleryItem( string title, string description, Expression<Func<IDiagram>> diagram )
		{
			Title = title;
			Description = description;
			Diagram = diagram;
		}

		public string Title { get; }
		public string Description { get; }
		public Expression<Func<IDiagram>> Diagram { get; }
	}
}
