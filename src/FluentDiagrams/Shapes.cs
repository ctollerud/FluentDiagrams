using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams
{
	public static class Shapes
	{
		/// <summary>
		/// A circle with a radius of 0.5
		/// </summary>
		/// <returns></returns>
		public static IDiagram Circle() => CircleDiagram.Default;
	}
}
