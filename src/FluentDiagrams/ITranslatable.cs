namespace FluentDiagrams
{
	/// <summary>
	/// Implemented by IDiagrams that are capable of translating
	/// their underlying geometry, without needing to rely on SVG transformation
	/// </summary>
	public interface ITranslatable
	{
		IDiagram PerformTranslate( decimal x, decimal y );
	}
}
