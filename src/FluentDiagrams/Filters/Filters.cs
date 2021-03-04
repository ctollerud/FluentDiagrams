using FluentDiagrams.Primitives;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using LinqGarden;
using LinqGarden.Enumerables;
using LinqGarden.Functions;
using static FluentDiagrams.Core.Filters.BlendFilter;

namespace FluentDiagrams.Core.Filters
{
	public static class Filters
	{
		public static IFilterComponent Photonegative { get; } = ColorMatrix.Photonegative();
		public static IFilterComponent Noop { get; } = ColorMatrix.Noop();

		/// <summary>
		/// Use a value of 0 or greater for the deviations
		/// </summary>
		/// <param name="standardDeviationX"></param>
		/// <param name="standardDeviationY"></param>
		/// <returns></returns>
		public static IFilterComponent GaussianBlur( decimal standardDeviationX, decimal standardDeviationY ) =>
			new GaussianBlurFilter( standardDeviationX, standardDeviationY );

		/// <summary>
		/// Use a value of 0 or greater for the deviations
		/// </summary>
		/// <param name="standardDeviation"></param>
		/// <returns></returns>
		public static IFilterComponent GaussianBlur( decimal standardDeviation ) =>
			GaussianBlur( standardDeviation, standardDeviation );

		public static IFilterComponent DropShadow( Vector vector, decimal stdDeviation ) =>
			new DropShadowFilter( vector, stdDeviation );

		public static IFilterComponent Chain( params IFilterComponent[] components ) =>
			new FilterChain( components );

		public static IFilterComponent Chain( this IEnumerable<IFilterComponent> components ) =>
			new FilterChain( components.AsCollection() );

		public static IFilterComponent TurbulentDisplacement(
			decimal displacementScale,
			decimal frequencyX = 0.5M,
			decimal frequencyY = 1M,
			int numberOfOctaves = 2,
			int seed = 42,
			bool stitchTiles = true ) =>
			new DisplacementFilter(
				displacementProvider:
				new TurbulenceGenerator(
					frequencyX,
					frequencyY,
					numberOfOctaves,
					seed,
					stitchTiles ), displacementScale );

		public static IFilterComponent Multiply( IFilterComponent filterComponent ) =>
			new CompositeFilter( filterComponent, CompositeOperator.Arithmetic, 1, 0, 0, 0 );

		public static IFilterComponent BrightnessToAlpha()
		{
			var fullBrightness = new ColorMatrix.ComponentTransformation( 0, 0, 0, 0, 1 );

			return
				new ColorMatrix(
					redChannel: fullBrightness,
					greenChannel: fullBrightness,
					blueChannel: fullBrightness,
					alphaChannel: new ColorMatrix.ComponentTransformation( 1, 0, 0, 0, 0 ) );
		}

		public static IFilterComponent Offset( Vector vector ) =>
			new OffsetFilter( vector );

		public static IFilterComponent Blend( IFilterComponent secondInput, BlendMode blendMode ) =>
			new BlendFilter( secondInput, blendMode );

		public static IFilterComponent ScaleComponents(
			decimal scaleRed = 1,
			decimal offsetRed = 0,
			decimal scaleGreen = 1,
			decimal offsetGreen = 0,
			decimal scaleBlue = 1,
			decimal offsetBlue = 0,
			decimal scaleAlpha = 1,
			decimal offsetAlpha = 0
			)
		{
			return ColorMatrix.ScaleComponents( scaleRed, offsetRed, scaleGreen, offsetGreen, scaleBlue, offsetBlue, scaleAlpha, offsetAlpha );
		}

		public static IFilterComponent ScaleBrightness( decimal brightness ) =>
			ScaleComponents( scaleRed: brightness, scaleGreen: brightness, scaleBlue: brightness );

		public static IFilterComponent FloodPreserveAlpha( Color fillColor )
		{
			static decimal CreateOffset( byte colorComponent )
			{
				return ( (decimal)colorComponent ) / 255;
			}

			return ScaleComponents(
				scaleRed: 0,
				offsetRed: CreateOffset( fillColor.R ),
				scaleGreen: 0,
				offsetGreen: CreateOffset( fillColor.G ),
				scaleBlue: 0,
				offsetBlue: CreateOffset( fillColor.B ),
				scaleAlpha: CreateOffset( fillColor.A ) );
		}

		public static IFilterComponent ScaleOpacity( decimal opacityScale ) =>
			ScaleComponents( scaleAlpha: opacityScale );

		public static IFilterComponent Flood( Color color )
		{
			return new FloodGenerator( color );
		}

		public static IFilterComponent Source( SourceType sourceType ) =>
			new FilterSource( sourceType );

		public static IFilterComponent Overlay( params IFilterComponent[] components ) =>
			components.AsEnumerable().Overlay();

		public static IFilterComponent Overlay( this IEnumerable<IFilterComponent> components ) =>
			new MergeFilter( components.ToImmutableList() );

		private static IFilterComponent BuildMorphology( Morphology.Type type, decimal radiusX, decimal radiusY ) =>
			new Morphology( type, radiusX, radiusY );

		public static IFilterComponent Erode( decimal radiusX, decimal? radiusY = default ) =>
			BuildMorphology( Morphology.Type.Erode, radiusX, radiusY ?? radiusX );

		public static IFilterComponent Dilate( decimal radiusX, decimal? radiusY = default ) =>
			BuildMorphology( Morphology.Type.Dilate, radiusX, radiusY ?? radiusX );

		public static IFilterComponent SpecularLighting(
			Color lightColor,
			decimal specularConstant = 1,
			decimal surfaceScale = 1,
			decimal brightnessScale = 1,
			decimal specularExponent = 1,
			bool preserveAlpha = false,
			params IFilterLight[] lights ) =>
			SpecularLightingGenerator( lightColor, specularConstant, surfaceScale, brightnessScale, specularExponent, lights )
			.Pipe( x =>
				!preserveAlpha
					? x
					: Chain(
						FloodPreserveAlpha( Color.White ),
						ArithmeticMultiply( x )
						) )
			.Pipe( ArithmeticAdd );

		public static IFilterComponent SpecularLightingGenerator(
			Color lightColor,
			decimal specularConstant = 1,
			decimal surfaceScale = 1,
			decimal brightnessScale = 1,
			decimal specularExponent = 1,
			params IFilterLight[] lights )
		{
			var lightingGenerators = lights.Select( x =>
				Chain
				(
					new SpecularLightingGenerator( lightColor, specularConstant, surfaceScale, brightnessScale, specularExponent, x ),
					ScaleBrightness( brightnessScale * x.BrightnessScale )
				)

				);

			IFilterComponent composedComponent =
				lightingGenerators
				.Cast<IFilterComponent>()
				.Aggregate( ( acc, next ) =>
				{
					return ArithmeticAdd(
						acc,
						next );
				}
				);

			return composedComponent;

		}

		public static IFilterComponent DiffuseLighting(
			Color lightColor,
			decimal diffuseConstant = 1,
			decimal surfaceScale = 1,
			decimal brightnessScale = 1,
			params IFilterLight[] lights ) =>
				DiffuseLightingGenerator( lightColor, diffuseConstant, surfaceScale, brightnessScale, lights )
				.Pipe( ArithmeticMultiply );

		private static IFilterComponent ArithmeticMultiply( IFilterComponent lightingGenerator ) =>
			new CompositeFilter( secondInput: lightingGenerator, operation: CompositeOperator.Arithmetic, k1: 1M, k2: 0M, k3: 0M, k4: 0M );

		public static IFilterComponent DiffuseLightingGenerator( Color lightColor, decimal diffuseConstant, decimal surfaceScale, decimal brightnessScale, params IFilterLight[] lights )
		{
			var lightingGenerators =
				lights.Select( x =>
					Chain
					(
						new DiffuseLightingGenerator( lightColor, x, diffuseConstant, surfaceScale ),
						ScaleBrightness( brightnessScale * x.BrightnessScale )
					)
				);

			IFilterComponent composedComponent =
				lightingGenerators
				.Cast<IFilterComponent>()
				.Aggregate( ( acc, next ) =>
				{
					return ArithmeticAdd(
						acc,
						next );
				}
				);

			return composedComponent;

		}

		private static IFilterComponent ArithmeticAdd( IFilterComponent in1, IFilterComponent in2 ) =>
			new FilterChain(
				new IFilterComponent[]
				{
					in1,
					ArithmeticAdd( in2 )
				} );
		private static IFilterComponent ArithmeticAdd( IFilterComponent in2 ) =>
				new CompositeFilter( in2, CompositeOperator.Arithmetic, k1: 0, k2: 1, k3: 1, k4: 0 );

	}
}
