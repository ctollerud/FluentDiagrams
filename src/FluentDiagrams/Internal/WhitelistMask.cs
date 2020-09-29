using FluentDiagrams.Primitives;

namespace FluentDiagrams.Internal
{
	public class WhitelistMask : IDiagram
	{
		public IDiagram Mask { get; }
		public IDiagram Maskee { get; }

		public BoundingBox Bounds => Mask.Bounds;

		public WhitelistMask( IDiagram mask, IDiagram maskee )
		{
			Mask = mask;
			Maskee = maskee;
		}

		public IDiagram DeepRotate( Coordinate coordinate, Angle angle ) =>
			new WhitelistMask( Mask.DeepRotate( coordinate, angle ), Maskee.DeepRotate( coordinate, angle ) );
	}
}