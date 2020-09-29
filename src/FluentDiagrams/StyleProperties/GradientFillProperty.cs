using FluentDiagrams.Gradients;

namespace FluentDiagrams.StyleProperties
{
	public class GradientFillProperty : IStyleProperty
	{
		public GradientFillProperty( IGradient gradient )
		{
			Gradient = gradient;
		}

		public IGradient Gradient { get; }
	}
}
