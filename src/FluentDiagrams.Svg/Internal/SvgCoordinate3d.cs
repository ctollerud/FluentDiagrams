namespace FluentDiagrams.Svg.Internal
{
	internal class SvgCoordinate3d
	{
		public int X { get; }
		public int Y { get; }
		public int Z { get; }

		public SvgCoordinate3d( int x, int y, int z )
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}