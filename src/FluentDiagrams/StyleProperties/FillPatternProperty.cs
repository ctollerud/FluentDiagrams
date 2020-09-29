namespace FluentDiagrams.StyleProperties
{
	public class FillPatternProperty : IStyleProperty
	{
		public IDiagram PatternDiagram { get; }
		public decimal FillWidthRatio { get; }
		public decimal FillHeightRatio { get; }

		public FillPatternProperty( IDiagram patternDiagram, decimal fillWidthRatio, decimal fillHeightRatio )
		{
			PatternDiagram = patternDiagram;
			FillWidthRatio = fillWidthRatio;
			FillHeightRatio = fillHeightRatio;
		}
	}
}
