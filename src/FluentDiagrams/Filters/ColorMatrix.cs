using FluentDiagrams.Core.Filters;
using System.Collections.Generic;
using System.Linq;

namespace FluentDiagrams.Core
{
	public class ColorMatrix : IFilterComponent
	{
		public ComponentTransformation RedChannel { get; }
		public ComponentTransformation GreenChannel { get; }
		public ComponentTransformation BlueChannel { get; }

		internal static IFilterComponent Noop() =>
			new ColorMatrix(
				redChannel: new ComponentTransformation( 1, 0, 0, 0, 0 ),
				greenChannel: new ComponentTransformation( 0, 1, 0, 0, 0 ),
				blueChannel: new ComponentTransformation( 0, 0, 1, 0, 0 ),
				alphaChannel: new ComponentTransformation( 0, 0, 0, 1, 0 ) );

		/// <summary>
		/// Create a mask out of the alpha channel (white lets stuff through, black doesn't)
		/// </summary>
		/// <returns></returns>
		internal static IFilterComponent AlphaAsMask() =>
			new ColorMatrix(
				redChannel: new ComponentTransformation( 1, 0, 0, 0.333333333333M, 0 ),
				greenChannel: new ComponentTransformation( 0, 1, 0, 0.333333333333M, 0 ),
				blueChannel: new ComponentTransformation( 0, 0, 1, 0.333333333333M, 0 ),
				alphaChannel: new ComponentTransformation( 0, 0, 0, 0, 1 ) );

		public ComponentTransformation AlphaChannel { get; }

		public class ComponentTransformation
		{
			public ComponentTransformation( decimal redScale, decimal greenScale, decimal blueScale, decimal alphaScale, decimal offset )
			{
				RedScale = redScale;
				GreenScale = greenScale;
				BlueScale = blueScale;
				AlphaScale = alphaScale;
				Offset = offset;
			}

			public decimal RedScale { get; }
			public decimal GreenScale { get; }
			public decimal BlueScale { get; }
			public decimal AlphaScale { get; }
			public decimal Offset { get; }

			public IEnumerable<decimal> GetMatrixValues() => new[] { RedScale, GreenScale, BlueScale, AlphaScale, Offset };
		}

		public ColorMatrix(
			ComponentTransformation redChannel,
			ComponentTransformation greenChannel,
			ComponentTransformation blueChannel,
			ComponentTransformation alphaChannel )
		{
			RedChannel = redChannel;
			GreenChannel = greenChannel;
			BlueChannel = blueChannel;
			AlphaChannel = alphaChannel;
		}

		public IEnumerable<decimal> GetMatrixValues() =>
			new[]
			{
				RedChannel,
				GreenChannel,
				BlueChannel,
				AlphaChannel
			}.SelectMany( component => component.GetMatrixValues() );

		internal static IFilterComponent ScaleComponents(
			decimal scaleRed,
			decimal offsetRed,
			decimal scaleGreen,
			decimal offsetGreen,
			decimal scaleBlue,
			decimal offsetBlue,
			decimal scaleAlpha,
			decimal offsetAlpha ) =>
			new ColorMatrix(
				redChannel: new ComponentTransformation( scaleRed, 0, 0, 0, offsetRed ),
				greenChannel: new ComponentTransformation( 0, scaleGreen, 0, 0, offsetGreen ),
				blueChannel: new ComponentTransformation( 0, 0, scaleBlue, 0, offsetBlue ),
				alphaChannel: new ComponentTransformation( 0, 0, 0, scaleAlpha, offsetAlpha ) // alpha channel is left unaffected.
				);

		public static ColorMatrix Photonegative() =>
			new ColorMatrix(
				redChannel: new ComponentTransformation( -1, 0, 0, 0, 1 ),
				greenChannel: new ComponentTransformation( 0, -1, 0, 0, 1 ),
				blueChannel: new ComponentTransformation( 0, 0, -1, 0, 1 ),
				alphaChannel: new ComponentTransformation( 0, 0, 0, 1, 0 ) // alpha channel is left unaffected.
				);
	}
}
