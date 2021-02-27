using FluentDiagrams.Primitives;

namespace FluentDiagrams
{
	public interface IDiagram
	{
		BoundingBox Bounds { get; }

		public IDiagram Rotate( Angle angle );

	}
}
