using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentDiagrams.Internal.Transformations
{
	public class RotatedDiagram : IDiagram
	{
		public IDiagram Diagram { get; }
		public Angle Angle { get; }
		public Coordinate RotationOrigin { get; }
		public BoundingBox Bounds { get; }

		internal RotatedDiagram( IDiagram innerDiagram, Angle angle, Coordinate rotationOrigin )
		{
			Diagram = innerDiagram;
			Angle = angle;
			RotationOrigin = rotationOrigin;
			Bounds = Diagram.Bounds.RotateAbout( rotationOrigin, angle );
		}

		/// <summary>
		/// cat's out of the bag. Just tack the next rotate on top
		/// TODO: we can optimize this!
		///		1. We need to determine where the new origin is
		///		2. We need to perform some amount of rotation to this object.
		/// </summary>
		/// <param name="coordinate"></param>
		/// <param name="angle"></param>
		/// <returns></returns>
		public IDiagram DeepRotate( Coordinate coordinate, Angle angle ) =>
			new RotatedDiagram( this, angle, coordinate );
	}
}
