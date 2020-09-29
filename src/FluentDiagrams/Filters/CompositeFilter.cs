namespace FluentDiagrams.Core.Filters
{
	public enum CompositeOperator
	{
		Over,
		In,
		Out,
		Atop,
		Xor,
		Lighter,
		Arithmetic
	}

	public class CompositeFilter : IFilterComponent
	{
		public IFilterComponent SecondInput { get; }
		public CompositeOperator Operation { get; }
		public decimal K1 { get; }
		public decimal K2 { get; }
		public decimal K3 { get; }
		public decimal K4 { get; }

		public CompositeFilter( IFilterComponent secondInput, CompositeOperator operation, decimal k1, decimal k2, decimal k3, decimal k4 )
		{
			SecondInput = secondInput;
			Operation = operation;
			K1 = k1;
			K2 = k2;
			K3 = k3;
			K4 = k4;
		}
	}
}