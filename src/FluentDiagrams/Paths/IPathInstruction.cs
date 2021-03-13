using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Paths
{
	public interface IPathInstruction
	{
		Coordinate EndPosition { get; }
		BoundingBox GetBoundingBox( Coordinate startPosition );
		IPathInstruction RotateAbout( Coordinate rotationOrigin, Angle angle );
	}
}
