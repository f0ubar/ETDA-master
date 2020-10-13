using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Dreamer.Mapping
{
    public struct Point
    {
        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Point(Size Size) : this(Size.Width, Size.Height) { }

        public Point(System.Drawing.Point Location) : this(Location.X, Location.Y) { }

        public int X { get; set; }
        public int Y { get; set; }
        #region Logic
        public static bool operator ==(Point A, Point B) { return A.X == B.X && A.Y == B.Y; }
        public override bool Equals(object obj)
        {
            return obj is Point point && point == this;
        }
        public override int GetHashCode()
        {
            byte[] data = new byte[4];
            Array.Copy(BitConverter.GetBytes((short)X), 0, data, 0, 2);
            Array.Copy(BitConverter.GetBytes((short)Y), 2, data, 0, 2);
            return BitConverter.ToInt32(data, 0);
        }
        public static bool operator !=(Point A, Point B) { return A.X != B.X || A.Y != B.Y; }
        public static bool operator <(Point A, Point B) { return A.X < B.X && A.Y < B.Y; }
        public static bool operator >(Point A, Point B) { return A.X > B.X && A.Y > B.Y; }
        public static bool operator <=(Point A, Point B) { return A.X <= B.X && A.Y <= B.Y; }
        public static bool operator >=(Point A, Point B) { return A.X >= B.X && A.Y >= B.Y; }
        #endregion
        #region Math
        public static Point operator -(Point A, Point B) { return new Point(A.X - B.X, A.Y - B.Y); }
        public static Point operator -(Point A, int B) { return new Point(A.X - B, A.Y - B); }
        public static Point[] operator -(Point Point, Point[] PointArray) { return PointArray.Select(point => Point - point).ToArray(); }
        public static Point operator +(Point A, Point B) { return new Point(A.X + B.X, A.Y + B.Y); }
        public static Point operator +(Point A, int B) { return new Point(A.X + B, A.Y + B); }
        public static Point[] operator +(Point Point, Point[] PointArray)
        {
            IEnumerable<Point> points = PointArray.Select(point => Point + point);
            return points.ToArray();
        }
        public static Point operator *(Point A, Point B) { return new Point(A.X * B.X, A.Y * B.Y); }
        public static Point operator *(Point A, int B) { return new Point(A.X * B, A.Y * B); }
        public static Point[] operator *(Point Point, Point[] PointArray) { return PointArray.Select(point => Point * point).ToArray(); }
        public static Point operator /(Point A, Point B) { return new Point(A.X / B.X, A.Y / B.Y); }
        public static Point operator /(Point A, int B) { return new Point(A.X / B, A.Y / B); }
        public static Point[] operator /(Point Point, Point[] PointArray) { return PointArray.Select(point => Point / point).ToArray(); }
        public Point Abs => new Point(Math.Abs(X), Math.Abs(Y));
        public int Value => X + Y;
        public int Min => Math.Min(X, Y);
        public int Max => Math.Max(X, Y);
        public int Distance(Point Point) { return (this - Point).Abs.Value; }
        #endregion
        #region Directional
        public static readonly Point Zero = new Point(0, 0);
        public static readonly Point West = new Point(-1, 0);
        public Point WestOf => this + West;
        public static readonly Point North = new Point(0, -1);
        public Point NorthOf => this + North;
        public static readonly Point East = new Point(1, 0);
        public Point EastOf => this + East;
        public static readonly Point South = new Point(0, 1);
        public Point SouthOf => this + South;
        public static readonly Point[] AllDirections = new Point[] { West, North, East, South };

        public Point[] SurroundingPoints => this + AllDirections;
        public Direction DirectionToward(Point Point)
        {
            Point difference = this - Point;
            return
                difference == Point.Zero ?
                    Direction.None :
                    Math.Abs(difference.X) >= Math.Abs(difference.Y) ?
                        difference.X < 0 ?
                            Direction.West :
                            Direction.East :
                        difference.Y < 0 ?
                            Direction.North :
                            Direction.South;
        }
        public Direction DirectDirectionTowards(Point Point)
        {
            Point difference = this - Point;
            Point absoluteDifference = difference.Abs;
            return
                absoluteDifference.Max != 1 ? Direction.None :
                absoluteDifference.Min != 0 ? Direction.None :
                difference == North ? Direction.North :
                difference == South ? Direction.South :
                difference == East ? Direction.East :
                difference == West ? Direction.West :
                Direction.None;
        }
        #endregion
        #region System.Drawing Conversions
        public System.Drawing.Point ToPoint()
        {
            return new System.Drawing.Point(X, Y);
        }
        public Size ToSize()
        {
            return new Size(X, Y);
        }
        public Rectangle ToRectangle(Point Size)
        {
            return new Rectangle(ToPoint(), Size.ToSize());
        }
        #endregion
        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
    public class Shape
    {
        public static Point[] GetRhombus(int LegLength)
        {
            Point[] points = new Point(LegLength, LegLength) * Point.AllDirections;
            List<Point> polygon = new List<Point>
            {
                points[0] + Point.South
            };
            for (Point p = points[0]; p != points[1]; p += Point.North + Point.East)
            {
                polygon.AddRange(
                    new Point[]
                    {
                        p,
                        p + Point.East
                    }
                );
            }
            polygon.Add(points[1]);
            for (Point p = points[1]; p != points[2]; p += Point.South + Point.East)
            {
                polygon.AddRange(
                    new Point[]
                    {
                        p + Point.East,
                        p + Point.East + Point.South
                    }
                );
            }
            polygon.Add(points[2] + Point.East);
            for (Point p = points[2]; p != points[3]; p += Point.South + Point.West)
            {
                polygon.AddRange(
                    new Point[]
                    {
                        p + Point.East + Point.South,
                        p + Point.South,
                    }
                );
            }
            polygon.Add(points[3] + Point.East + Point.South);
            for (Point p = points[3]; p != points[0]; p += Point.West + Point.North)
            {
                polygon.AddRange(
                    new Point[]
                    {
                        p + Point.South,
                        p
                    }
                );
            }
            return polygon.ToArray();
        }
    }
}
