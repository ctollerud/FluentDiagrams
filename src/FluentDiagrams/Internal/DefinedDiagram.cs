using FluentDiagrams.Internal.Transformations;
using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Internal
{
	public class DefinedDiagram : IDiagram, IRotatable
	{
		public DefinedDiagram( IDiagram diagram )
		{
			Diagram = diagram;
		}

		public BoundingBox Bounds => Diagram.Bounds;

		public IDiagram Diagram { get; }

		IDiagram IRotatable.PerformRotate( Angle angle ) =>
			new RotatedDiagram( this, angle, this.Diagram.Bounds.Center() );
	}
}
