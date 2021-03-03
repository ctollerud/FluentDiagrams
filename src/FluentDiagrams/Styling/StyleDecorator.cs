﻿using FluentDiagrams.Primitives;
using FluentDiagrams.StyleProperties;

namespace FluentDiagrams.Styling
{
	/// <summary>
	/// Represents a diagram with some styling applied.
	/// </summary>
	public class StyleDecorator : IDiagram, IRotatable, ITranslatable, IScalable
	{
		public StyleDecorator( IDiagram diagram, IStyleProperty property )
		{
			Diagram = diagram;
			Property = property;
		}

		public BoundingBox Bounds => Diagram.Bounds;

		public IDiagram Diagram { get; }
		public IStyleProperty Property { get; }


		private StyleDecorator WithNewDiagram( IDiagram newDiagram ) =>
			new StyleDecorator( newDiagram, Property );

		IDiagram IScalable.PerformScaling( decimal x, decimal y ) =>
			this.WithNewDiagram( Diagram.Scale( x, y ) );

		public IDiagram PerformTranslate( decimal x, decimal y ) =>
			this.WithNewDiagram( Diagram.Offset( x, y ) );

		IDiagram IRotatable.PerformRotate( Angle angle ) =>
			this.WithNewDiagram( Diagram.Rotate( angle ) );
	}
}
