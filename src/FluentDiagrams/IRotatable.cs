using FluentDiagrams.Primitives;

namespace FluentDiagrams
{

	/// <summary>
	/// Implemented by IDiagrams that are capable of
	/// rotating its underlying geometry, instead of solely 
	/// relying on SVG transforms and the like.
	/// </summary>
	public interface IRotatable
	{
		IDiagram PerformRotate( Angle angle );
	}
}
