using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FluentDiagrams.Core.Filters
{
	public class FloodGenerator : IFilterComponent
	{
		public FloodGenerator( Color color )
		{
			Color = color;
		}

		public Color Color { get; }
	}
}
