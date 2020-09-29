using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Core.Filters
{
	public class Spotlight : IFilterLight
	{
		public Coordinate3d LightSource { get; }
		public Coordinate3d LightDestination { get; }
		public Angle ConeAngle { get; }
		public decimal SpecularExponent { get; }
		public decimal BrightnessScale { get; }

		public Spotlight( Coordinate3d lightSource, Coordinate3d lightDestination, Angle coneAngle, decimal specularExponent = 1M, decimal brightnessScale = 1M )
		{
			LightSource = lightSource;
			LightDestination = lightDestination;
			ConeAngle = coneAngle;
			SpecularExponent = specularExponent;
			BrightnessScale = brightnessScale;
		}
	}
}
