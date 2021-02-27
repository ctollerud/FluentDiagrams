using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqGarden.Enumerables;

namespace FluentDiagrams.Internal.Shapes
{
	public class PolygonDiagram : IDiagram
	{
		public ICollection<Coordinate> Coordinates { get; }

		public PolygonDiagram( IEnumerable<Coordinate> coordinates )
		{
			Coordinates = coordinates.AsCollection();
			Bounds = BoundingBox.Compose( Coordinates );
		}

		public BoundingBox Bounds { get; }

		public IDiagram Rotate( Angle angle ) =>
			new PolygonDiagram( Coordinates.Select( coord => coord.RotateAbout( Bounds.Center(), angle ) ) );
	}
}
