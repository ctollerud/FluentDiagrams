using System.Collections.Generic;

namespace FluentDiagrams.Core.Filters
{
	public class BlendFilter : IFilterComponent
	{
		private static readonly IReadOnlyDictionary<BlendMode, string> EnumToSvgName =
		new Dictionary<BlendMode, string>
		{
			[BlendMode.Normal] = "normal",
			[BlendMode.Multiply] = "multiply",
			[BlendMode.Screen] = "screen",
			[BlendMode.Overlay] = "overlay",
			[BlendMode.Darken] = "darken",
			[BlendMode.Lighten] = "lighten",
			[BlendMode.ColorDodge] = "color-dodge",
			[BlendMode.ColorBurn] = "color-burn",
			[BlendMode.HardLight] = "hard-light",
			[BlendMode.SoftLight] = "soft-light",
			[BlendMode.Difference] = "difference",
			[BlendMode.Exclusion] = "exclusion",
			[BlendMode.Hue] = "hue",
			[BlendMode.Saturation] = "saturation",
			[BlendMode.Color] = "color",
			[BlendMode.Luminosity] = "luminosity",
		};

		public enum BlendMode
		{
			/// <summary>
			/// The final color is the top color, regardless of what the bottom color is.
			/// The effect is like two opaque pieces of paper overlapping.
			/// </summary>
			Normal,

			/// <summary>
			/// The final color is the result of multiplying the top and bottom colors.
			/// A black layer leads to a black final layer, and a white layer leads to no change.
			/// The effect is like two images printed on transparent film overlapping.
			/// </summary>
			Multiply,

			/// <summary>
			/// The final color is the result of inverting the colors, multiplying them, and inverting that value.
			/// A black layer leads to no change, and a white layer leads to a white final layer.
			/// The effect is like two images shone onto a projection screen.
			/// </summary>
			Screen,

			/// <summary>
			/// The final color is the result of multiply if the bottom color is darker, or screen if the bottom color is lighter.
			/// This blend mode is equivalent to hard-light but with the layers swapped.
			/// </summary>
			Overlay,

			/// <summary>
			/// The final color is composed of the darkest values of each color channel.
			/// </summary>
			Darken,

			/// <summary>
			/// The final color is composed of the lightest values of each color channel.
			/// </summary>
			Lighten,


			/// <summary>
			/// The final color is the result of dividing the bottom color by the inverse of the top color.
			/// A black foreground leads to no change. A foreground with the inverse color of the backdrop leads to a fully lit color.
			/// This blend mode is similar to screen, but the foreground need only be as light as the inverse of the backdrop to create a fully lit color.
			/// </summary>
			ColorDodge,


			/// <summary>
			/// The final color is the result of inverting the bottom color, dividing the value by the top color, and inverting that value.
			/// A white foreground leads to no change. A foreground with the inverse color of the backdrop leads to a black final image.
			/// This blend mode is similar to multiply, but the foreground need only be as dark as the inverse of the backdrop to make the final image black.
			/// </summary>
			ColorBurn,

			/// <summary>
			/// The final color is the result of multiply if the top color is darker, or screen if the top color is lighter.
			/// This blend mode is equivalent to overlay but with the layers swapped.
			/// The effect is similar to shining a harsh spotlight on the backdrop.
			/// </summary>
			HardLight,

			/// <summary>
			/// The final color is similar to hard-light, but softer.
			/// This blend mode behaves similar to hard-light.
			/// The effect is similar to shining a diffused spotlight on the backdrop.
			/// </summary>
			SoftLight,

			/// <summary>
			/// The final color is the result of subtracting the darker of the two colors from the lighter one.
			/// A black layer has no effect, while a white layer inverts the other layer's color.
			/// </summary>
			Difference,


			/// <summary>
			/// The final color is similar to difference, but with less contrast.
			/// As with difference, a black layer has no effect, while a white layer inverts the other layer's color.
			/// </summary>
			Exclusion,

			/// <summary>
			/// The final color has the hue of the top color, while using the saturation and luminosity of the bottom color.
			/// </summary>
			Hue,

			/// <summary>
			/// The final color has the saturation of the top color, while using the hue and luminosity of the bottom color.
			/// A pure gray backdrop, having no saturation, will have no effect.
			/// </summary>
			Saturation,

			/// <summary>
			/// The final color has the hue and saturation of the top color, while using the luminosity of the bottom color.
			/// The effect preserves gray levels and can be used to colorize the foreground.
			/// </summary>
			Color,

			/// <summary>
			/// The final color has the luminosity of the top color, while using the hue and saturation of the bottom color.
			/// This blend mode is equivalent to color, but with the layers swapped.
			/// </summary>
			Luminosity


		}

		public BlendFilter( IFilterComponent source2, BlendMode blendMode )
		{
			Source2 = source2;
			BlendModeType = blendMode;
		}

		public IFilterComponent Source2 { get; }
		public BlendMode BlendModeType { get; }

		public string BlendModeSvgName => EnumToSvgName[BlendModeType];
	}
}
