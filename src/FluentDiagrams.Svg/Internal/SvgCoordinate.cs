using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Svg.Internal
{
	internal class SvgCoordinate
	{
		public SvgCoordinate( int x, int y )
		{
			X = x;
			Y = y;
		}

		public int X { get; }
		public int Y { get; }
	}
}
