using System.Drawing;

namespace FluentDiagrams.StyleProperties
{
	public class StrokeColorProperty : IStyleProperty
	{
		public StrokeColorProperty( Color color )
		{
			Color = color;
		}

		public Color Color { get; }
	}
}
