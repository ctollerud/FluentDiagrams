using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqGarden.Functions;
using FluentDiagrams.Internal.Shapes;

namespace FluentDiagrams.Internal.Transformations
{
	public class RectangleDiagram : IDiagram
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

		public IDiagram DeepRotate( Coordinate coordinate, Angle angle )
		{
			var newCenter = Bounds.Center().RotateAbout( coordinate, angle );

			bool isRotationOrthogonal = ( angle.Rotations * 4 ).Pipe( x => x - Math.Floor( x ) ) == 0M;

			return isRotationOrthogonal
				? Bounds.RotateAbout( coordinate, angle ).Pipe( x => new RectangleDiagram( x.Width, x.Height, x.Center() ) ).Pipe( x => (IDiagram)x )
				: new[] { Bounds.TopLeft, Bounds.TopRight, Bounds.BottomRight, Bounds.BottomLeft }
				  .Select( x => x.RotateAbout( coordinate, angle ) )
				  .Pipe( x => new PolygonDiagram( x ) );
		}
	}
}
