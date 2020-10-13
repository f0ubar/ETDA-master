namespace Dreamer.Mapping
{
    public class PointGrid<T> where T : class
    {
        public Point Bounds { get; set; }
        public T[,] Grid { get; set; }
        public bool PointInBounds(Point Point)
        {
            return Point >= Point.Zero && Point < Bounds;
        }
        public T this[Point Point]
        {
            get { return PointInBounds(Point) ? Grid[Point.X, Point.Y] : null; }
            set { if (PointInBounds(Point)) Grid[Point.X, Point.Y] = value; }
        }
        public T this[int X, int Y]
        {
            get { return this[new Point(X, Y)]; }
            set { this[new Point(X, Y)] = value; }
        }
    }
}
