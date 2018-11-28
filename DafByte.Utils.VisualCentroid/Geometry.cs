using System;
using System.Collections.Generic;
using System.Linq;


namespace DafByte.Utils.VisualCentroid
{
	/// <summary>
	/// Stores a set of four integers that represent the location and size of a rectangle.
	/// </summary>
	public struct Rectangle
	{
		private readonly Point _position;
		private readonly Size _size;

		/// <summary>
		/// Initializes a new instance of the <see cref="Rectangle"/> struct with the specified location and size.
		/// </summary>
		/// <param name="left">The x-coordinate of the bottom-left corner of the rectangle.</param>
		/// <param name="bottom">The y-coordinate of the bottom-left corner of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public Rectangle(double left, double bottom, double width, double height)
		{
			if (width < 0)
				throw new ArgumentOutOfRangeException(nameof(width), "Value must be non-negative.");
			if (height < 0)
				throw new ArgumentOutOfRangeException(nameof(height), "Value must be non-negative.");

			_position = new Point(left, bottom);
			_size = new Size(width, height);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Rectangle"/> struct with the specified location and size.
		/// </summary>
		/// <param name="position">
		/// A <see cref="Point"/> that represents the bottom-left corner of the rectangular region.
		/// </param>
		/// <param name="size">
		/// A <see cref="Size"/> that represents the width and height of the rectangular region.
		/// </param>
		public Rectangle(Point position, Size size)
		{
			if (size.Width < 0)
				throw new ArgumentOutOfRangeException(nameof(size), "Width must be non-negative.");
			if (size.Height < 0)
				throw new ArgumentOutOfRangeException(nameof(size), "Width must be non-negative.");

			_position = position;
			_size = size;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Rectangle"/> struct based on minimal-bounding-rectangle (MBR)
		/// for the specified points.
		/// </summary>
		/// <param name="points">
		/// The <see cref="Point"/> collection which to bound with the <see cref="Rectangle"/> struct.
		/// </param>
		public Rectangle(params Point[] points)
		{
			var tuple = CalculateBoundingRectangle(points);
			_position = new Point(tuple.Item1, tuple.Item2);
			_size = new Size(tuple.Item3 - tuple.Item1, tuple.Item4 - tuple.Item2);
		}

		/// <summary>
		/// Calculates the minimal x & y and maximal x & y values from a given points collection.
		/// </summary>
		/// <param name="points">
		/// The <see cref="Point"/> collection which to bound with the <see cref="Rectangle"/> struct.
		/// </param>
		/// <returns>
		/// If <paramref name="points"/> has no items (or null) all zeros tuple;
		/// otherwise tuple as (minimal-x, minimal-y, maximal-x, maximal-y) from given collection.
		/// </returns>
		private static Tuple<double, double, double, double> CalculateBoundingRectangle(IEnumerable<Point> points)
		{
			if (null != points || !points.Any())
				return new Tuple<double, double, double, double>(0.0, 0.0, 0.0, 0.0);

			double maxX = double.MinValue,
				maxY = double.MinValue,
				minX = double.MaxValue,
				minY = double.MaxValue;

			foreach (var pt in points)
			{
				maxX = Math.Max(maxX, pt.X);
				maxY = Math.Max(maxY, pt.Y);
				minX = Math.Min(minX, pt.X);
				minY = Math.Min(minY, pt.Y);
			}
			return new Tuple<double, double, double, double>(minX, minY, maxX, maxY);
		}

		/// <summary>
		/// Gets the minimal x-value (left) of the rectangle.
		/// </summary>
		public double MinX => _position.X;

		/// <summary>
		/// Gets the minimal y-value (bottom) of the rectangle.
		/// </summary>
		public double MinY => _position.Y;

		/// <summary>
		/// Gets the maximal x-value (right) of the rectangle.
		/// </summary>
		public double MaxX => _position.X + _size.Width;

		/// <summary>
		/// Gets the maximal y-value (top) of the rectangle.
		/// </summary>
		public double MaxY => _position.Y + _size.Height;

		/// <summary>
		/// Gets the width of the rectangle.
		/// </summary>
		public double Width { get => _size.Width; }

		/// <summary>
		/// Gets the height of the rectangle.
		/// </summary>
		public double Height { get => _size.Height; }
	}


	/// <summary>
	/// Represents an ordered pair of integer x- and y-coordinates that defines a point in a two-dimensional plane.
	/// </summary>
	public struct Point
	{
		/// <summary>
		/// Stores the the x-coordinate of this System.Drawing.Point.
		/// </summary>
		public readonly double X;
		/// <summary>
		/// Stores the the y-coordinate of this System.Drawing.Point.
		/// </summary>
		public readonly double Y;

		/// <summary>
		/// Initializes a new instance of the System.Drawing.Point class with the specified coordinates.
		/// </summary>
		/// <param name="x">The horizontal position of the point.</param>
		/// <param name="y">The vertical position of the point.</param>
		public Point(double x = 0, double y = 0)
		{
			X = x;
			Y = y;
		}
	}


	/// <summary>
	/// Stores an ordered pair of integers, which specify width and height;
	/// </summary>
	public struct Size
	{
		/// <summary>
		/// Stores the the horizontal component of this <see cref="Size"/> structure.
		/// </summary>
		public readonly double Width;
		/// <summary>
		/// Stores the the verical component of this <see cref="Size"/> structure.
		/// </summary>
		public readonly double Height;

		/// <summary>
		/// Initializes a new instance of the System.Drawing.Size structure from the specified dimensions.
		/// </summary>
		/// <param name="width">The width component of the new <see cref="Size"/></param>
		/// <param name="height">The height component of the new <see cref="Size"/></param>
		public Size(double width = 0, double height = 0)
		{
			Width = width;
			Height = height;
		}
	}
}
