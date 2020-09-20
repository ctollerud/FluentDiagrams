using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Primitives
{
	public class Coordinate3d
	{
		public decimal X { get; }
		public decimal Y { get; }
		public decimal Z { get; }
		public static Coordinate3d Origin { get; } = Coordinate3d.Cartesian( 0, 0, 0 );

		private Coordinate3d( decimal x, decimal y, decimal z )
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static Coordinate3d Cartesian( decimal x, decimal y, decimal z ) =>
			new Coordinate3d( x, y, z );
	}

	public static class Coordinate3dExt
	{
		public static Coordinate3d To3d( this Coordinate input, decimal z ) =>
			Coordinate3d.Cartesian( input.X, input.Y, z );
	}


}
