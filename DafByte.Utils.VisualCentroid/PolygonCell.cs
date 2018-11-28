using System;
using System.Collections.Generic;
using System.Linq;


namespace DafByte.Utils.VisualCentroid
{
	internal class PolygonCell : IComparable<PolygonCell>
	{
		public static readonly double SQRT_2 = Math.Sqrt(2.0);

		public PolygonCell(double x, double y, double halfSize, IEnumerable<Point> polygon)
			: this(new Point(x, y), halfSize, polygon)
		{
		}

		public PolygonCell(Point center, double halfSize, IEnumerable<Point> polygon)
		{
			Center = center;
			HalfSize = halfSize;
			CenterToPolygonDistance = GetPointToPolygonDistance(center, polygon);
			MaxPolygonDistance = CenterToPolygonDistance + halfSize * SQRT_2;
		}

		/// <summary>
		/// Calculates the signed distance from point to polygon outline (negative if point is outside).
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="polygon"></param>
		/// <returns>Signed distance from point to polygon outline (negative if point is outside)</returns>
		private static double GetPointToPolygonDistance(Point pt, IEnumerable<Point> polygon)
		{
			var inside = false;
			var minDistSquare = double.MaxValue;

			var arrPolygon = polygon as Point[] ?? polygon.ToArray();
			for (int i = 0, j = arrPolygon.Length - 1; i < arrPolygon.Length; j = i++)
			{
				var a = arrPolygon[i];
				var b = arrPolygon[j];

				if ((a.Y > pt.Y != b.Y > pt.Y) && (pt.X < (b.X - a.X) * (pt.Y - a.Y) / (b.Y - a.Y) + a.X))
					inside = !inside;

				minDistSquare = Math.Min(minDistSquare, GetSegmentDistanceSquare(pt, a, b));
			}

			return (inside ? Math.Sqrt(minDistSquare) : -Math.Sqrt(minDistSquare));
		}

		/// <summary>
		/// Gets the squared distance from pt point to a segment.
		/// </summary>
		/// <param name="pt">Point to measure from.</param>
		/// <param name="ptA">First point on segment.</param>
		/// <param name="ptB">Second point on segment.</param>
		private static double GetSegmentDistanceSquare(Point pt, Point ptA, Point ptB, double tolerance = 1E-10)
		{
			var x = ptA.X;
			var y = ptA.Y;
			var dx = ptB.X - ptA.X;
			var dy = ptB.Y - ptA.Y;

			if (Math.Abs(dx) > tolerance || Math.Abs(dy) > tolerance)
			{
				var t = ((pt.X - ptA.X) * dx + (pt.Y - ptA.Y) * dy) / (dx * dx + dy * dy);

				if (t > 1)
				{
					x = ptB.X;
					y = ptB.Y;
				}
				else if (t > 0)
				{
					x += dx * t;
					y += dy * t;
				}
			}

			dx = pt.X - x;
			dy = pt.Y - y;

			return dx * dx + dy * dy;
		}

		public int CompareTo(PolygonCell other)
		{
			var diff = MaxPolygonDistance - other.MaxPolygonDistance;
			return diff > 0 ? 1 : diff < 0 ? -1 : 0;
		}


		public Point Center { get; private set; }

		/// <summary>
		/// Half the cell size (AKA h)
		/// </summary>
		public double HalfSize { get; private set; }

		/// <summary>
		/// Distance from cell center to polygon (AKA d)
		/// </summary>
		public double CenterToPolygonDistance { get; private set; }

		/// <summary>
		/// Max distance to polygon within ptA cell (AKA max)
		/// </summary>
		public double MaxPolygonDistance { get; private set; }
	}


	internal class CentroidCellComparer : Comparer<PolygonCell>
	{
		public override int Compare(PolygonCell x, PolygonCell y)
		{
			if (null == x)
				return null == y ? 0 : -1;

			if (null == y)
				return 1;

			var diff = x.MaxPolygonDistance - y.MaxPolygonDistance;
			return diff > 0 ? 1 : diff < 0 ? -1 : 0;
		}
	}
}
