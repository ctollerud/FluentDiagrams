using FluentDiagrams.Internal;
using FluentDiagrams.Paths;
using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Internal
{
	public enum StrokeStyle
	{
		RoundedNoMargin,
		RoundedMargin,
		Rectangle

	}

	public class PathDiagram : IDiagram
	{
		public PathDiagram( PathInstructions instructions, decimal strokeWidth, StrokeStyle strokeStyle )
		{
			Instructions = instructions;
			StrokeWidth = strokeWidth;
			StrokeStyle = strokeStyle;
		}

		public BoundingBox Bounds => Instructions.GetBoundingBox();

		public PathInstructions Instructions { get; }
		public decimal StrokeWidth { get; }
		public StrokeStyle StrokeStyle { get; }

		public IDiagram DeepRotate( Coordinate coordinate, Angle angle )
		{
			return new PathDiagram( Instructions.RotateAbout( coordinate, angle ), StrokeWidth, StrokeStyle );
		}
	}
}
