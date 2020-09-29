using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Paths
{
	public static class PathInstruction
	{
		public static CubicPathInstruction Cubic( Coordinate endPosition, Coordinate controlPoint1, Coordinate controlPoint2 ) =>
			new CubicPathInstruction( endPosition, controlPoint1, controlPoint2 );

		internal static IPathInstruction MoveTo( Coordinate coord2 ) =>
			new MoveInstruction( coord2 );

	}
}
