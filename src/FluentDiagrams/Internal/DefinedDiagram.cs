using FluentDiagrams.Internal.Transformations;
using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Internal
{
	public class DefinedDiagram : IDiagram
	{
		public DefinedDiagram( IDiagram diagram )
		{
			Diagram = diagram;
		}

		public BoundingBox Bounds => Diagram.Bounds;

		public IDiagram Diagram { get; }

		public IDiagram DeepRotate( Coordinate coordinate, Angle angle ) =>
			new RotatedDiagram( this, angle, coordinate );
	}
}
