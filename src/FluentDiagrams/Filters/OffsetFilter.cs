
using FluentDiagrams.Primitives;

namespace FluentDiagrams.Core.Filters
{
	public class OffsetFilter : IFilterComponent
	{
		public Vector Vector { get; }

		public OffsetFilter( Vector vector )
		{
			Vector = vector;
		}
	}
}