using FluentDiagrams.Primitives;
using LinqGarden.Enumerables;
using LinqGarden.Functions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FluentDiagrams
{
	/// <summary>
	/// Used to represent the boundaries of an object in 2d space.
	/// </summary>
	public class BoundingBox
	{
		public BoundingBox( decimal xMin, decimal xMax, decimal yMin, decimal yMax )
		{
			if( xMax < xMin )
			{
				throw new InvalidOperationException( "bounding boxes can't have a negative width" );
			}
			if( yMax < yMin )
			{
				throw new InvalidOperationException( "bounding boxes can't have a negative height." );
			}

			XMin = xMin;
			XMax = xMax;
			YMin = yMin;
			YMax = yMax;
		}
		internal BoundingBox Scale( decimal x, decimal y )
		{
			return BoundingBox.Create( Math.Abs( this.Width * x ), Math.Abs( this.Height * y ), Center() );
		}

		public static BoundingBox Create( decimal width, decimal height, Coordinate centerPoint ) =>
			BoundingBox.Create( width, height ).Offset( new Vector( centerPoint.X, centerPoint.Y ) );

		internal BoundingBox RotateAbout( Coordinate rotationOrigin, Angle angle )
		{
			return
				new[]
				{
					( XMin, YMax ), //top left
					( XMax, YMax ), //top right
					( XMax, YMin ), //bottom right
					( XMin, YMin ), // bottom left
				}.Select( tuple => Coordinate.Cartesian( tuple.Item1, tuple.Item2 ) )
				.Select( coord => coord.RotateAbout( rotationOrigin, angle ) )
				.Pipe( BoundingBox.Compose );
		}

		public static BoundingBox Create( decimal width, decimal height )
		{
			var xValue = width / 2;
			var yValue = height / 2;

			return new BoundingBox(
				xMin: -xValue,
				yMin: -yValue,
				xMax: xValue,
				yMax: yValue );
		}

		public BoundingBox Offset( Vector vector ) =>
			Offset( vector.Dx, vector.Dy );

		public BoundingBox Offset( decimal dx, decimal dy ) =>
			new BoundingBox(
				xMin: this.XMin + dx,
				xMax: this.XMax + dx,
				yMin: this.YMin + dy,
				yMax: this.YMax + dy );

		/// <summary>
		/// create a bounding-box of width/height of 1, with (0,0) at the center.
		/// </summary>
		/// <returns></returns>
		public static BoundingBox BasicSquare() =>
			new BoundingBox( xMin: -0.5M, xMax: 0.5M, yMin: -0.5M, yMax: 0.5M );

		public static BoundingBox Compose( IEnumerable<BoundingBox> boxes )
		{
			var collection = boxes.AsCollection();

			return
				new BoundingBox(
					xMin: collection.Min( x => x.XMin ),
					xMax: collection.Max( x => x.XMax ),
					yMin: collection.Min( x => x.YMin ),
					yMax: collection.Max( x => x.YMax ) );

		}

		public static BoundingBox Compose( IEnumerable<Coordinate> coordinates )
		{
			var collection = coordinates.AsCollection();

			return
				new BoundingBox(
					xMin: collection.Min( x => x.X ),
					xMax: collection.Max( x => x.X ),
					yMin: collection.Min( x => x.Y ),
					yMax: collection.Max( x => x.Y ) );

		}

		public Coordinate Center() =>
			new Coordinate( ( XMax + XMin ) / 2, ( YMax + YMin ) / 2 );

		public decimal XMin { get; }
		public decimal XMax { get; }
		public decimal YMin { get; }
		public decimal YMax { get; }

		public decimal Width => XMax - XMin;
		public decimal Height => YMax - YMin;

		public Coordinate TopLeft => Coordinate.Cartesian( XMin, YMax );
		public Coordinate TopRight => Coordinate.Cartesian( XMax, YMax );
		public Coordinate BottomLeft => Coordinate.Cartesian( XMin, YMin );
		public Coordinate BottomRight => Coordinate.Cartesian( XMax, YMin );

		public Coordinate TopMiddle => Coordinate.Cartesian( Center().X, YMax );
	}
}
