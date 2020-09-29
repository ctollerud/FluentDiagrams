using System.Drawing;

namespace FluentDiagrams.Core.Filters
{
	public class DiffuseLightingGenerator : IFilterComponent
	{
		public Color LightColor { get; }
		public IFilterLight Light { get; }
		public decimal DiffuseConstant { get; }
		public decimal SurfaceScale { get; }

		public DiffuseLightingGenerator( Color lightColor, IFilterLight light, decimal diffuseConstant, decimal surfaceScale )
		{
			this.LightColor = lightColor;
			this.Light = light;
			this.DiffuseConstant = diffuseConstant;
			this.SurfaceScale = surfaceScale;
		}
	}
}