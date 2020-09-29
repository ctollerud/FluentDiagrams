using FluentDiagrams.Primitives;

namespace FluentDiagrams.Core.Filters
{
	public static class Lights
	{
		public static IFilterLight Spotlight( Coordinate3d lightSource, Coordinate3d lightDestination, Angle coneAngle, decimal specularExponent = 1M, decimal scaleBrightness = 1M ) =>
			new Spotlight( lightSource, lightDestination, coneAngle, specularExponent, scaleBrightness );

		public static IFilterLight DistantLight( Angle? azimuth = null, Angle? elevation = null, decimal brightnessScale = 1M ) =>
			new DistantLight( azimuth ?? Angle.Zero, elevation ?? Angle.Zero, brightnessScale );

		public static IFilterLight Pointlight( Coordinate3d coordinate3d, decimal scaleBrightness = 1M ) =>
			new PointLight( coordinate3d, scaleBrightness );
	}
}
