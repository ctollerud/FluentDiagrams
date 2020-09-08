using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams.Primitives
{
	public class Coordinate
	{
		public Coordinate( decimal x, decimal y )
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Get the cartesian coordinate X position
		/// </summary>
		public decimal X { get; }

		/// <summary>
		/// Get the cartesian coordinate Y position
		/// </summary>
		public decimal Y { get; }
	}
}
