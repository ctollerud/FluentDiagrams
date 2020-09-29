using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Core.Filters
{
	public class FilterSource : IFilterComponent
	{
		public FilterSource( SourceType sourceType )
		{
			SourceType = sourceType;
		}

		public SourceType SourceType { get; }
	}
}
