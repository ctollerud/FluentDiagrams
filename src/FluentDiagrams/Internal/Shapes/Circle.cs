using FluentDiagrams.Primitives;
using LinqGarden;
using LinqGarden.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Primitives
{
	public class CircleDiagram : IDiagram
	{
		public Maybe<BoundingBox> Bounds { get; }

		public static CircleDiagram Default => 
			new CircleDiagram(
				radius: 0.5M,
				origin: Coordinate.Origin() );

		internal CircleDiagram( decimal radius, Coordinate origin )
		{
			Radius = radius;
			Origin = origin;

			Bounds =
				( radius * 2 )
				.Pipe( diameter => BoundingBox.Create( diameter, diameter, origin ) )
				.Pipe( Maybe.Some );
		}
		public decimal Radius { get; }
		public Coordinate Origin { get; }
	}
}
