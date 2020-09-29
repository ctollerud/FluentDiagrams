using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace FluentDiagrams.Core.Filters
{
	public class MergeFilter : IFilterComponent
	{
		public MergeFilter( ImmutableList<IFilterComponent> filterComponents )
		{
			FilterComponents = filterComponents;
		}

		public ImmutableList<IFilterComponent> FilterComponents { get; }
	}
}
