namespace FluentDiagrams.Core.Filters
{
	public class DisplacementFilter : IFilterComponent
	{

		public DisplacementFilter( IFilterComponent displacementProvider, decimal scale )
		{
			DisplacementProvider = displacementProvider;
			Scale = scale;
		}

		public decimal Scale { get; }

		public IFilterComponent DisplacementProvider { get; }
	}
}
