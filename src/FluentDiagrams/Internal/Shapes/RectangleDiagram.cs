﻿using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqGarden.Functions;
using FluentDiagrams.Internal.Shapes;

namespace FluentDiagrams.Internal.Shapes
{
	public class RectangleDiagram : IDiagram, IRotatable, IScalable, ITranslatable
	{
		public RectangleDiagram( decimal width, decimal height, Coordinate origin )
		{
			if( width <= 0M )
			{
				throw new ArgumentException( $"{nameof( width )} must be greater than 0." );
			}
			if( height <= 0M )
			{
				throw new ArgumentException( $"{nameof( height )} must be greater than 0." );
			}

			Bounds = BoundingBox.Create( width, height, origin );

		}

		public decimal Width => Bounds.Width;
		public decimal Height => Bounds.Height;

		public BoundingBox Bounds { get; }

		public IDiagram PerformScaling( decimal x, decimal y ) =>
			new RectangleDiagram( Width * x, Height * y, Bounds.Center() );

		IDiagram ITranslatable.PerformTranslate( decimal x, decimal y ) =>
			new RectangleDiagram( Width, Height, Bounds.Center().Translate( x, y ) );

		IDiagram IRotatable.PerformRotate( Angle angle )
		{
			var center = Bounds.Center();

			bool isRotationOrthogonal = ( angle.Rotations * 4 ).Pipe( x => x - Math.Floor( x ) ) == 0M;

			return isRotationOrthogonal
				? Bounds.RotateAbout( center, angle ).Pipe( x => new RectangleDiagram( x.Width, x.Height, x.Center() ) ).Pipe( x => (IDiagram)x )
				: new[] { Bounds.TopLeft, Bounds.TopRight, Bounds.BottomRight, Bounds.BottomLeft }
				  .Select( x => x.RotateAbout( center, angle ) )
				  .Pipe( x => new PolygonDiagram( x ) );
		}
	}
}
