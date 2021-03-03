using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentDiagrams.Internal.Transformations
{
	public class RotatedDiagram : IDiagram, IRotatable
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

		IDiagram IRotatable.PerformRotate( Angle angle ) =>
			new RotatedDiagram( this, Angle.Plus( angle ), Bounds.Center() );
	}
}
