using FluentDiagrams.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentDiagrams
{
	public static class BehavioralExtensions
	{
		public static IDiagram Defined( this IDiagram diagram )
		{
			if( diagram is DefinedDiagram alreadyDefined )
			{
				return alreadyDefined;
			}
			return new DefinedDiagram( diagram );
		}

		/// <summary>
		/// Uses the diagram as a white-listing mask over another item.
		/// </summary>
		/// <param name="mask"></param>
		/// <param name="maskee"></param>
		/// <returns></returns>
		public static IDiagram AsMaskOver( this IDiagram mask, IDiagram maskee )
		{
			return new WhitelistMask( mask, maskee );
		}
	}
}
