using FluentDiagrams.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqGarden.Functions;

namespace FluentDiagrams.Internal
{
	internal static class EnvelopeCalculation
	{
		public static BoundingBox GetCubicBezierBounds( Coordinate start, Coordinate controlPoint1, Coordinate controlPoint2, Coordinate end )
		{
			var coords =
				new[] { start, controlPoint1, controlPoint2, end }
				.Select( x => (Convert.ToDouble( x.X ), Convert.ToDouble( x.Y )) )
				.ToList();

			return GetCubicBezierBoundsInternal( coords );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="P"></param>
		/// <returns></returns>
		public static BoundingBox GetCubicBezierBoundsInternal( IReadOnlyList<(double X, double Y)> P )
		{
			var PX = P.Select( x => x.X ).ToArray();
			var PY = P.Select( x => x.Y ).ToArray();

			static double evalBez( double[] poly, double t )
			{
				var x = ( poly[0] * ( 1 - t ) * ( 1 - t ) * ( 1 - t ) )
					+ ( 3 * poly[1] * t * ( 1 - t ) * ( 1 - t ) ) + ( 3 * poly[2] * t * t * ( 1 - t ) )
					+ ( poly[3] * t * t * t );
				return x;
			}

			//function findBB()
			//{
			//    var a = 3 * P[3].X - 9 * P[2].X + 9 * P[1].X - 3 * P[0].X;
			var a = ( 3 * P[3].X ) - ( 9 * P[2].X ) + ( 9 * P[1].X ) - ( 3 * P[0].X );
			//    var b = 6 * P[0].X - 12 * P[1].X + 6 * P[2].X;
			var b = ( 6 * P[0].X ) - ( 12 * P[1].X ) + ( 6 * P[2].X );
			//    var c = 3 * P[1].X - 3 * P[0].X;
			var c = ( 3 * P[1].X ) - ( 3 * P[0].X );
			//    //alert("a "+a+" "+b+" "+c);
			//    var disc = b * b - 4 * a * c;
			var disc = ( b * b ) - ( 4 * a * c );
			//    var xl = P[0].X;
			var xl = P[0].X;
			//    var xh = P[0].X;
			var xh = P[0].X;
			//    if( P[3].X < xl ) xl = P[3].X;
			if( P[3].X < xl )
			{
				xl = P[3].X;
			}
			//    if( P[3].X > xh ) xh = P[3].X;
			if( P[3].X > xh )
			{
				xh = P[3].X;
			}
			//    if( disc >= 0 )
			if( disc >= 0 )
			//    {
			{
				//        var t1 = ( -b + Math.sqrt( disc ) ) / ( 2 * a );
				var t1 = ( -b + Math.Sqrt( disc ) ) / ( 2 * a );
				//        alert( "t1 " + t1 );
				//        if( t1 > 0 && t1 < 1 )
				if( t1 > 0 && t1 < 1 )
				//        {
				{

					//            var x1 = evalBez( PX, t1 );
					var x1 = evalBez( PX, t1 );
					//            if( x1 < xl ) xl = x1;
					if( x1 < xl )
					{
						xl = x1;
					}
					//            if( x1 > xh ) xh = x1;
					if( x1 > xh )
					{
						xh = x1;
					}
					//        }
				}

				//        var t2 = ( -b - Math.sqrt( disc ) ) / ( 2 * a );
				var t2 = ( -b - Math.Sqrt( disc ) ) / ( 2 * a );
				//        alert( "t2 " + t2 );
				//        if( t2 > 0 && t2 < 1 )
				if( t2 > 0 && t2 < 1 )
				//        {
				{

					//            var x2 = evalBez( PX, t2 );
					var x2 = evalBez( PX, t2 );
					//            if( x2 < xl ) xl = x2;
					if( x2 < xl )
					{
						xl = x2;
					}
					//            if( x2 > xh ) xh = x2;
					if( x2 > xh )
					{
						xh = x2;
					}
					//        }
				}
				//    }
			}

			//    a = 3 * P[3].Y - 9 * P[2].Y + 9 * P[1].Y - 3 * P[0].Y;
			a = ( 3 * P[3].Y ) - ( 9 * P[2].Y ) + ( 9 * P[1].Y ) - ( 3 * P[0].Y );
			//    b = 6 * P[0].Y - 12 * P[1].Y + 6 * P[2].Y;
			b = ( 6 * P[0].Y ) - ( 12 * P[1].Y ) + ( 6 * P[2].Y );
			//    c = 3 * P[1].Y - 3 * P[0].Y;
			c = ( 3 * P[1].Y ) - ( 3 * P[0].Y );
			//    disc = b * b - 4 * a * c;
			disc = ( b * b ) - ( 4 * a * c );
			//    var yl = P[0].Y;
			var yl = P[0].Y;
			//    var yh = P[0].Y;
			var yh = P[0].Y;
			//    if( P[3].Y < yl ) yl = P[3].Y;
			if( P[3].Y < yl )
			{
				yl = P[3].Y;
			}
			//    if( P[3].Y > yh ) yh = P[3].Y;
			if( P[3].Y > yh )
			{
				yh = P[3].Y;
			}
			//    if( disc >= 0 )
			if( disc >= 0 )
			//    {
			{
				//        var t1 = ( -b + Math.sqrt( disc ) ) / ( 2 * a );
				var t1 = ( -b + Math.Sqrt( disc ) ) / ( 2 * a );
				//        alert( "t3 " + t1 );

				//        if( t1 > 0 && t1 < 1 )
				if( t1 > 0 && t1 < 1 )
				//        {
				{
					//            var y1 = evalBez( PY, t1 );
					var y1 = evalBez( PY, t1 );
					//            if( y1 < yl ) yl = y1;
					if( y1 < yl )
					{
						yl = y1;
					}
					//            if( y1 > yh ) yh = y1;
					if( y1 > yh )
					{
						yh = y1;
					}
					//        }
				}

				//        var t2 = ( -b - Math.sqrt( disc ) ) / ( 2 * a );
				var t2 = ( -b - Math.Sqrt( disc ) ) / ( 2 * a );
				//        alert( "t4 " + t2 );

				//        if( t2 > 0 && t2 < 1 )
				if( t2 > 0 && t2 < 1 )
				//        {
				{
					//            var y2 = evalBez( PY, t2 );
					var y2 = evalBez( PY, t2 );
					//            if( y2 < yl ) yl = y2;
					if( y2 < yl )
					{
						yl = y2;
					}
					//            if( y2 > yh ) yh = y2;
					if( y2 > yh )
					{
						yh = y2;
					}
					//        }
				}
				//    }
			}
			//    ctx.lineWidth = 1;
			//    ctx.beginPath();
			//    ctx.moveTo( xl, yl );
			//    ctx.lineTo( xl, yh );
			//    ctx.lineTo( xh, yh );
			//    ctx.lineTo( xh, yl );
			//    ctx.lineTo( xl, yl );
			//    ctx.stroke();
			var boundingBox =
				new[] { (xl, yl), (xh, yh) }
				.Select( x => Coordinate.Cartesian( Convert.ToDecimal( x.Item1 ), Convert.ToDecimal( x.Item2 ) ) )
				.Pipe( BoundingBox.Compose );

			return boundingBox;

			//    alert( "" + xl + " " + xh + " " + yl + " " + yh );
			//}


		}
	}
}
