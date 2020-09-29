namespace FluentDiagrams.Styling
{
	public class SvgStrokeWidth : IStrokeWidth
	{
		public decimal StrokeWidth { get; }

		public SvgStrokeWidth( decimal strokeWidth )
		{
			StrokeWidth = strokeWidth;
		}
	}
}