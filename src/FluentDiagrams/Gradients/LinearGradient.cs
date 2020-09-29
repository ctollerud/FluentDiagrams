using FluentDiagrams.Primitives;
using System.Drawing;

namespace FluentDiagrams.Gradients
{
	public class LinearGradient : IGradient
	{
		public Color Start { get; }
		public Color End { get; }
		public Angle Angle { get; }

		public LinearGradient( Color start, Color end, Angle angle )
		{
			Start = start;
			End = end;
			Angle = angle;
		}
	}
}
