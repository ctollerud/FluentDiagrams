using FluentDiagrams.Styling;

namespace FluentDiagrams.StyleProperties
{
	public class StrokeWidthProperty : IStyleProperty
	{
		public StrokeWidthProperty( IStrokeWidth strokeWidth )
		{
			StrokeWidth = strokeWidth;
		}

		public IStrokeWidth StrokeWidth { get; }
	}
}
