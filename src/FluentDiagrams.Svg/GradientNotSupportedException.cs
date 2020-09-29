using System;

namespace FluentDiagrams.Svg
{
	public class GradientNotSupportedException : Exception
	{

		public GradientNotSupportedException()
		{
		}

		public GradientNotSupportedException( Type type ) : base( $"Gradient of type '{type}' is not supported" )
		{
		}
	}
}