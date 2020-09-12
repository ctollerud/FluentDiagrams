using FluentDiagrams;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Svg
{
	public class DiagramNotRenderableException : Exception
	{
		public DiagramNotRenderableException( IDiagram diagram ) : base( BuildMessage( diagram ) )
		{}

		private static string BuildMessage( IDiagram diagram ) =>
			$"No procedure for handling '{diagram.GetType()}' could be found.";
	}
}
