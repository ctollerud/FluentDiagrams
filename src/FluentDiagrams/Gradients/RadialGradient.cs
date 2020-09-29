using System.Drawing;

namespace FluentDiagrams.Gradients
{
	public class RadialGradient : IGradient
	{
		public RadialGradient( Color centerColor, Color edgeColor )
		{
			CenterColor = centerColor;
			EdgeColor = edgeColor;
		}

		public Color CenterColor { get; }
		public Color EdgeColor { get; }
	}
}
