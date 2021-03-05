using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Internal.Transformations
{
	public class ViewboxDiagram : IDiagram
	{
		public IDiagram Diagram { get; }
		public BoundingBox Bounds { get; }

		public decimal XMin { get; }
		public decimal YMin { get; }

		public ViewboxDiagram( IDiagram innerDiagram, BoundingBox newBounds, decimal xMin, decimal yMin )
		{
			Bounds = newBounds;
			Diagram = innerDiagram;
			XMin = xMin;
			YMin = yMin;
		}
	}
}
