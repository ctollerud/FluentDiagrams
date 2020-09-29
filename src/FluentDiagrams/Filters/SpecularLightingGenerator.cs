using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FluentDiagrams.Core.Filters
{
	public class SpecularLightingGenerator : IFilterComponent
	{
		public Color LightColor { get; }
		public decimal SpecularConstant { get; }
		public decimal SurfaceScale { get; }
		public decimal BrightnessScale { get; }
		public IFilterLight Light { get; }
		public decimal SpecularExponent { get; }

		public SpecularLightingGenerator( 
			Color lightColor, 
			decimal specularConstant, 
			decimal surfaceScale, 
			decimal brightnessScale,
			decimal specularExponent,
			IFilterLight light )
		{
			LightColor = lightColor;
			SpecularConstant = specularConstant;
			SurfaceScale = surfaceScale;
			BrightnessScale = brightnessScale;
			Light = light;
			SpecularExponent = specularExponent;
		}
	}
}
