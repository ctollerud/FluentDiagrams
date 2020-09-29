using FluentDiagrams.Primitives;
using System.Collections.Generic;

namespace FluentDiagrams.Paths
{
	public class CubicPathInstruction : IPathInstruction
	{
		public Coordinate EndPosition { get; }
		public Coordinate ControlPoint1 { get; }
		public Coordinate ControlPoint2 { get; }

		public CubicPathInstruction( Coordinate endPosition, Coordinate controlPoint1, Coordinate controlPoint2 )
		{
			EndPosition = endPosition;
			ControlPoint1 = controlPoint1;
			ControlPoint2 = controlPoint2;
		}

		public IEnumerable<Coordinate> GetBoundingCoordinates()
		{
			yield return EndPosition;
		}

		public IPathInstruction RotateAbout( Coordinate rotationOrigin, Angle angle ) =>
			new CubicPathInstruction(
				EndPosition.RotateAbout( rotationOrigin, angle ),
				ControlPoint1.RotateAbout( rotationOrigin, angle ),
				ControlPoint2.RotateAbout( rotationOrigin, angle ) );
	}
}