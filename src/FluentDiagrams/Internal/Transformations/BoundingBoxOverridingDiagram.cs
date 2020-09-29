using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Internal.Transformations
{
	public class BoundingBoxOverridingDiagram : IDiagram
	{
		internal BoundingBoxOverridingDiagram( IDiagram diagram, BoundingBox newBoundingBox )
		{
			Diagram = diagram;
			Bounds = newBoundingBox;
		}

		public BoundingBox Bounds { get; }

		public IDiagram Diagram { get; }

		public IDiagram DeepRotate( Coordinate coordinate, Angle angle ) =>
			new BoundingBoxOverridingDiagram(
				Diagram.DeepRotate( coordinate, angle ),
				Bounds.RotateAbout( coordinate, angle ) );
	}
}
