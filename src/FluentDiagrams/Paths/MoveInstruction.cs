using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Paths
{
	public class MoveInstruction : IPathInstruction
	{
		public Coordinate MoveTo { get; }

		public Coordinate EndPosition => MoveTo;

		public MoveInstruction( Coordinate moveTo )
		{
			MoveTo = moveTo;
		}

		public IPathInstruction RotateAbout( Coordinate rotationOrigin, Angle angle ) =>
			new MoveInstruction( MoveTo.RotateAbout( rotationOrigin, angle ) );

		public BoundingBox GetBoundingBox( Coordinate startPosition ) =>
			BoundingBox.Compose( startPosition, EndPosition );
	}
}
