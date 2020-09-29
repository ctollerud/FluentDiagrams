using FluentDiagrams.Core.Filters;

namespace FluentDiagrams.StyleProperties
{
	public class FilterProperty : IStyleProperty
	{
		public FilterProperty( IFilterComponent filterComponent )
		{
			FilterComponent = filterComponent;
		}

		public IFilterComponent FilterComponent { get; }
	}
}
