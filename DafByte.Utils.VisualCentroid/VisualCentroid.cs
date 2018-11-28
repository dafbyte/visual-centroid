using System;
using System.Collections.Generic;
using System.Linq;


namespace DafByte.Utils.VisualCentroid
{
	// Based on Vladimir Agafonkin's polylabel, as depicted on https://blog.mapbox.com/a-new-algorithm-for-finding-a-visual-center-of-a-polygon-7c77e6492fbc
	// See source https://github.com/mapbox/polylabel

	public class VisualCentroid
	{
		private readonly double _precision;
		private readonly double _tolerance;
		private readonly PriorityQueue<PolygonCell> _cellQueue = new PriorityQueue<PolygonCell>(new CentroidCellComparer());
		private Point[] _polygon;

		public VisualCentroid(double precision = 1D, double tolerance = 1E-10)
		{
			_precision = precision;
			_tolerance = tolerance;
		}

		public Point GetVisualCenter(IEnumerable<Point> polygon)
		{
			_polygon = polygon as Point[] ?? polygon.ToArray();

			var rect = new Rectangle(_polygon);

			var minX = rect.MinX;
			var minY = rect.MinY;
			var width = rect.Width;
			var height = rect.Height;
			var cellSize = Math.Min(width, height);

			if (Math.Abs(cellSize) < _tolerance)
				return new Point(minX, minY);

			GenerateInitialCells(rect);

			// take centroid as the first best guess
			var bestCell = GetCentroidCell(_polygon);

			// special case for rectangular polygons
			var boundingRectCell = new PolygonCell(minX + width * .5D, minY + height * .5D, 0, _polygon);
			if (boundingRectCell.CenterToPolygonDistance > bestCell.CenterToPolygonDistance)
				bestCell = boundingRectCell;

			while (_cellQueue.Any())
			{
				var cell = _cellQueue.Dequeue();

				if (cell.CenterToPolygonDistance > bestCell.CenterToPolygonDistance)
					bestCell = cell;

				// ReSharper disable once InvertIf
				if (cell.MaxPolygonDistance - bestCell.CenterToPolygonDistance > _precision)
					EnqueueCellFragments(cell);
			}

			return bestCell.Center;
		}

		private void GenerateInitialCells(Rectangle rect)
		{
			var cellSize = Math.Min(rect.Width, rect.Height);
			var halfSize = cellSize * .5F;

			for (var x = rect.MinX; x < rect.MaxX; x += cellSize)
				for (var y = rect.MinY; y < rect.MaxY; y += cellSize)
				{
					var newCell = new PolygonCell(x + halfSize, y + halfSize, halfSize, _polygon);
					_cellQueue.Enqueue(newCell);
				}
		}

		private PolygonCell GetCentroidCell(IList<Point> polygon)
		{
			var area = 0D;
			var cx = 0D;
			var cy = 0D;

			for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
			{
				var a = polygon[i];
				var b = polygon[j];
				var d = a.X * b.Y - b.X * a.Y;
				cx += (a.X + b.X) * d / 6D;
				cy += (a.Y + b.Y) * d / 6D;
				area += d * .5D;
			}
			return Math.Abs(area) < _tolerance
						? new PolygonCell(polygon[0], 0, polygon)
						: new PolygonCell(cx / area, cy / area, 0, polygon);
		}
		private void EnqueueCellFragments(PolygonCell cell)
		{
			var h = cell.HalfSize * .5F;
			_cellQueue.Enqueue(new PolygonCell(cell.Center.X - h, cell.Center.Y - h, h, _polygon));
			_cellQueue.Enqueue(new PolygonCell(cell.Center.X + h, cell.Center.Y - h, h, _polygon));
			_cellQueue.Enqueue(new PolygonCell(cell.Center.X - h, cell.Center.Y + h, h, _polygon));
			_cellQueue.Enqueue(new PolygonCell(cell.Center.X + h, cell.Center.Y + h, h, _polygon));
		}
	}
}
