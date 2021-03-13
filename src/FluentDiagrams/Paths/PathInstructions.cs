using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqGarden.Enumerables;
using LinqGarden.Functions;

namespace FluentDiagrams.Paths
{
	public class PathInstructions
	{
		public IPathInstruction[] Instructions { get; }
		public Coordinate StartLocation { get; }

		public PathInstructions RotateAbout( Coordinate rotationOrigin, Angle angle )
		{
			return new PathInstructions( StartLocation.RotateAbout( rotationOrigin, angle ), Instructions.Select( x => x.RotateAbout( rotationOrigin, angle ) ).ToArray() );
		}

		public PathInstructions( Coordinate startLocation, params IPathInstruction[] instructions )
		{
			Instructions = instructions;
			StartLocation = startLocation;
		}

		public static PathInstructions Segments( IEnumerable<Coordinate> coordinates )
		{
			var coords = coordinates.AsCollection();
			var startLocation = coordinates.First();
			var moveInstructions = coords.Skip( 1 ).Select( x => new MoveInstruction( x ) );

			return new PathInstructions( startLocation, moveInstructions.ToArray() );
		}

		internal BoundingBox GetBoundingBox() =>
			Instructions.SelectMany( x => x.GetBoundingCoordinates() )
			.StartWith( StartLocation )
			.Pipe( BoundingBox.Compose )
			.Pipe( x =>
			{

				var width = x.Width;
				var height = x.Height;
				if( x.Width == 0 )
				{
					width = 1;
				}
				if( x.Height == 0 )
				{
					height = 1;
				}

				return BoundingBox.Create( width, height, x.Center() );
			} );

		public static PathInstructions Segments( params Coordinate[] coordinates ) =>
			Segments( coordinates.AsEnumerable() );
	}
}
