using FluentDiagrams.Primitives;

namespace FluentDiagrams.Internal
{
	public class WhitelistMask : IDiagram, IRotatable
	{
		public IDiagram Mask { get; }
		public IDiagram Maskee { get; }

		public BoundingBox Bounds => Mask.Bounds;

		public WhitelistMask( IDiagram mask, IDiagram maskee )
		{
			Mask = mask;
			Maskee = maskee;
		}

		IDiagram IRotatable.PerformRotate( Angle angle ) =>
			new WhitelistMask( Mask.Rotate( angle ), Maskee.RotateAbout( Bounds.Center(), angle ) );
	}
}