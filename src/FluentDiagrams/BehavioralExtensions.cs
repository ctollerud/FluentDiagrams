using FluentDiagrams.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using LinqGarden.Functions;

namespace FluentDiagrams
{
	public static class BehavioralExtensions
	{

		/// <summary>
		/// "defines" the diagram so that it isn't written into the output multiple times.
		/// This allows technologies like SVG, for instance, to more effectively decrease their over-all size.
		///
		/// A diagram that's already defined can't be geometrically translated, scaled, or rotated,
		/// and will rely more heavilly on the renderer's ability to handle these processes.
		/// </summary>
		/// <param name="diagram"></param>
		/// <returns></returns>
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
		/// the darker a given mask pixel is, the more transparent the masked diagram's pixels are.
		/// 
		/// This diagram uses the mask's envelope.
		/// </summary>
		/// <param name="mask"></param>
		/// <param name="maskee"></param>
		/// <returns></returns>
		public static IDiagram AsMaskOver( this IDiagram mask, IDiagram maskee )
		{
			return new WhitelistMask( mask, maskee );
		}

		public static IDiagram AsMaskOver( this IDiagram mask, Func<IDiagram, IDiagram> backgroundModifier ) =>
			mask.AsMaskOver( Shapes.Square().ScaleTo( mask ).OffsetTo( mask ).Pipe( backgroundModifier ) );
	}
}
