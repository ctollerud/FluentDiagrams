using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Paths
{
	public class MoveInstruction : IPathInstruction
	{
		public Coordinate MoveTo { get; }

		public MoveInstruction( Coordinate moveTo )
		{
			MoveTo = moveTo;
		}

		public IEnumerable<Coordinate> GetBoundingCoordinates()
		{
			yield return MoveTo;
		}

		public IPathInstruction RotateAbout( Coordinate rotationOrigin, Angle angle ) =>
			new MoveInstruction( MoveTo.RotateAbout( rotationOrigin, angle ) );
	}
}
