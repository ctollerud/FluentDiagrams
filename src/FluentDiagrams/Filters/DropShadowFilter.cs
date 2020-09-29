using FluentDiagrams.Primitives;

namespace FluentDiagrams.Core.Filters
{
	public class DropShadowFilter : IFilterComponent
	{
		public DropShadowFilter( Vector vector, decimal stdDeviation )
		{
			ShadowVector = vector;
			StdDeviation = stdDeviation;
		}

		public Vector ShadowVector { get; }
		public decimal StdDeviation { get; }
	}
}
