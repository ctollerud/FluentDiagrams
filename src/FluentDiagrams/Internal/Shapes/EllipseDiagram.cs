using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Internal.Shapes
{
	public class EllipseDiagram : IDiagram, IScalable
	{

		public EllipseDiagram( decimal rx, decimal ry, Coordinate center )
		{
			Rx = rx;
			Ry = ry;
			Center = center;

			Bounds = BoundingBox.Create( Rx * 2, Ry * 2, center );
		}

		public BoundingBox Bounds { get; }

		public decimal Rx { get; }
		public decimal Ry { get; }
		public Coordinate Center { get; }

		IDiagram IScalable.PerformScaling( decimal x, decimal y ) =>
			FluentDiagrams.Shapes.Ellipse( Rx * x, Ry * y, Center );
	}
}
