using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Core.Filters
{
	public class TurbulenceGenerator : IFilterComponent
	{
		public TurbulenceGenerator( decimal frequencyX, decimal frequencyY, int numberOfOctaves, int seed, bool stitchTiles )
		{
			FrequencyX = frequencyX;
			FrequencyY = frequencyY;
			NumberOfOctaves = numberOfOctaves;
			Seed = seed;
			StitchTiles = stitchTiles;
		}

		public decimal FrequencyX { get; }
		public decimal FrequencyY { get; }

		public int NumberOfOctaves { get; }
		public int Seed { get; }
		public bool StitchTiles { get; }
	}
}
