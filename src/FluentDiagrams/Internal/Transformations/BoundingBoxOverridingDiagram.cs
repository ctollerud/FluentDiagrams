using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Internal.Transformations
{
	public class BoundingBoxOverridingDiagram : IDiagram, IRotatable
	{
		internal BoundingBoxOverridingDiagram( IDiagram diagram, BoundingBox newBoundingBox )
		{
			Diagram = diagram;
			Bounds = newBoundingBox;
		}

		public BoundingBox Bounds { get; }

		public IDiagram Diagram { get; }

		IDiagram IRotatable.PerformRotate( Angle angle ) =>
			new BoundingBoxOverridingDiagram(
				Diagram.RotateAbout( Bounds.Center(), angle ),
				Bounds.RotateAbout( Bounds.Center(), angle ) );
	}
}
