using System;
using FluentDiagrams.Core.Filters;

namespace FluentDiagrams.Svg
{
	public class FilterComponentNotSupportedException : Exception
	{

		public FilterComponentNotSupportedException( IFilterComponent component ) : base( $"filter component of type {component.GetType()} is not supported." )
		{
		}
	}
}