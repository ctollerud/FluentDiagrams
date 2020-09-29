using FluentDiagrams.Primitives;

namespace FluentDiagrams.Core.Filters
{
	public class DistantLight : IFilterLight
	{
		public Angle Azimuth { get; }
		public Angle Elevation { get; }
		public decimal BrightnessScale { get; }

		public DistantLight( Angle azimuth, Angle elevation, decimal brightnessScale = 1 )
		{
			Azimuth = azimuth.Reverse();
			Elevation = elevation;
			BrightnessScale = brightnessScale;
		}
	}
}