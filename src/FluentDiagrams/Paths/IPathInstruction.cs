using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Paths
{
	public interface IPathInstruction
	{
		IEnumerable<Coordinate> GetBoundingCoordinates();

		IPathInstruction RotateAbout( Coordinate rotationOrigin, Angle angle );
	}
}
