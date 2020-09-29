using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Core.Filters
{
	public class GaussianBlurFilter : IFilterComponent
	{
		public GaussianBlurFilter( decimal standardDeviationX, decimal standardDeviationY )
		{
			StandardDeviationX = standardDeviationX;
			StandardDeviationY = standardDeviationY;
		}

		public decimal StandardDeviationX { get; }
		public decimal StandardDeviationY { get; }
	}
}
