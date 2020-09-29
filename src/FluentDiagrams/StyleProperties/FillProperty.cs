using System.Drawing;

namespace FluentDiagrams.StyleProperties
{
	public class FillProperty : IStyleProperty
	{
		public FillProperty( Color fillColor )
		{
			FillColor = fillColor;
		}

		public Color FillColor { get; }
	}
}
