using System;
using System.Collections.Generic;
using System.Text;
using LinqGarden.Functions;

namespace FluentDiagrams.Internal
{
	public static class MathUtilities
	{
		public static decimal Rescale(
			decimal input,
			decimal inMin,
			decimal inMax,
			decimal outMin,
			decimal outMax ) =>
				input
				.Pipe( x => x - inMin )
				.Pipe( x => x / ( inMax - inMin ) )
				.Pipe( x => x * ( outMax - outMin ) )
				.Pipe( x => x + outMin );

	}
}
