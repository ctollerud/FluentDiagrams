namespace FluentDiagrams
{
	/// <summary>
	/// Implemented by IDiagrams that are capable scaling their underlying geometry, without needing to rely on SVG transformations.
	/// </summary>
	public interface IScalable
	{
		IDiagram PerformScaling( decimal x, decimal y );
	}
}
