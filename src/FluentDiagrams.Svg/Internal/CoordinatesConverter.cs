using FluentDiagrams.Primitives;
using LinqGarden.Functions;

namespace FluentDiagrams.Svg.Internal
{
	internal class CoordinatesConverter
	{
		public BoundingBox BoundingBox { get; }
		private readonly int m_InternalToSvgScale;

		public CoordinatesConverter( BoundingBox boundingBox, int internalToSvgScale )
		{
			BoundingBox = boundingBox;
			m_InternalToSvgScale = internalToSvgScale;

		}

		public int ScaleDistance( decimal internalLength ) =>
			 (int)( internalLength * m_InternalToSvgScale );

		public int SvgWidth => ScaleDistance( BoundingBox.Width );

		public int SvgHeight => ScaleDistance( BoundingBox.Height );

		private static int RescaleValue( decimal input, decimal inputRangeStart, decimal inputRangeEnd, int outputRangeStart, int outputRangeEnd ) =>
			input
			//remove the previous offset
			.Pipe( x => x - inputRangeStart )

			//scale to between 0 and 1
			.Pipe( x => x / ( inputRangeEnd - inputRangeStart ) )

			//re-scale to the output
			.Pipe( x => x * ( outputRangeEnd - outputRangeStart ) )

			//re-apply the offset
			.Pipe( x => x + outputRangeStart )
			.Pipe( x => (int)x );

		public int TranslateX( decimal x ) => RescaleValue( x, BoundingBox.XMin, BoundingBox.XMax, 0, SvgWidth );

		public int TranslateY( decimal y ) => RescaleValue( y, BoundingBox.YMax, BoundingBox.YMin, 0, SvgHeight );

		public SvgCoordinate ToSvgCoord( Coordinate coordinate ) =>
			new SvgCoordinate( TranslateX( coordinate.X ), TranslateY( coordinate.Y ) );

		public SvgCoordinate3d ToSvgCoord( Coordinate3d coordinate ) =>
			new SvgCoordinate3d( TranslateX( coordinate.X ), TranslateY( coordinate.Y ), ScaleDistance( coordinate.Z ) );

		public SvgVector ToSvgVector( Vector vector ) =>
			new SvgVector( ScaleDistance( vector.Dx ), -ScaleDistance( vector.Dy ) );
	}
}
