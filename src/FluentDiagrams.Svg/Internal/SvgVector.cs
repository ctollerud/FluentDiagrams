namespace FluentDiagrams.Svg.Internal
{
	internal class SvgVector
	{
		public int Dx { get; }
		public int Dy { get; }

		public SvgVector( int dx, int dy)
		{
			Dx = dx;
			Dy = dy;
		}
	}
}