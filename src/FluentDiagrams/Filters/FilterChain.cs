using System.Collections.Generic;

namespace FluentDiagrams.Core.Filters
{
	public class FilterChain : IFilterComponent
	{
		public FilterChain( ICollection<IFilterComponent> components )
		{
			Components = components;
		}

		public ICollection<IFilterComponent> Components { get; }
	}
}
