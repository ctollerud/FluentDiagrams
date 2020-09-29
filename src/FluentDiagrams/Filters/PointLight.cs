using FluentDiagrams.Primitives;

namespace FluentDiagrams.Core.Filters
{
	public class PointLight : IFilterLight
	{
		public Coordinate3d Coordinate { get; }

		public decimal BrightnessScale { get; }

		public PointLight( Coordinate3d coordinate3d, decimal brightnessScale = 1M )
		{
			Coordinate = coordinate3d;
			BrightnessScale = brightnessScale;
		}
	}
}