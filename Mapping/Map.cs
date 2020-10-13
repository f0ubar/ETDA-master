using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using System.Drawing.Text;
using System.CodeDom;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Mapping
{
    #region Array Iterators
    public static class ArrayIterators
    {
        public static void Function<T>(this T[] Array, Func<int, T, T> Function)
        {
            for (int i = 0; i < Array.Length; i++)
                Array[i] = Function(i, Array[i]);
        }
        public static void Function<T>(this T[,] Array, Func<int, int, T, T> Function)
        {
            for (int y = 0; y <= Array.GetUpperBound(1); y++)
                for (int x = 0; x <= Array.GetUpperBound(0); x++)
                    Array[x, y] = Function(x, y, Array[x, y]);
        }
        public static void Action<T>(this T[] Array, Action<int, T> Action)
        {
            for (int i = 0; i < Array.Length; i++)
                Action(i, Array[i]);
        }
        public static void Action<T>(this T[,] Array, Action<int, int, T> Action)
        {
            for (int y = 0; y <= Array.GetUpperBound(1); y++)
                for (int x = 0; x <= Array.GetUpperBound(0); x++)
                    Action(x, y, Array[x, y]);
        }
        public static T AtPoint<T>(this T[,] Array, Point Point) { return Array[Point.X, Point.Y]; }
    } 
    #endregion
    public enum Direction
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
        None = 4,
        Random = 5
    }
    public struct Point
    {
        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Point(Size Size) : this(Size.Width, Size.Height) { }
        public int X { get; set; }
        public int Y { get; set; }
        #region Logic
        public static bool operator ==(Point A, Point B) { return A.X == B.X && A.Y == B.Y; }
        public override bool Equals(object obj)
        {
            return obj is Point && (Point)obj == this;
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
        public static Point[] operator +(Point Point, Point[] PointArray) {
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
                            Direction.North:
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
    public abstract class MapObject
    {
        public MapObject(Map Map, Point Point)
        {
            this.Point = Point;
            this.Map = Map;
        }
        public Map Map { get; set; }
        public Point Point { get; set; }
        public virtual bool IsWall { get; set; } = false;
        public virtual bool IsDrawable { get; set; } = false;
        public virtual void OnDrawBackground(Graphics Graphics, Point Aspect) { }
        public virtual void OnDraw(Graphics Graphics, Point Aspect) { }
        public virtual void OnDrawOverlay(Graphics Graphics, Point Aspect) { }
        public List<TabMap.ILayer> TabMapLayers { get; } = new List<TabMap.ILayer>();
        public void Register()
        {
            Map.MapObjects.Add(this);
        }
        public void RegisterLayers(params TabMap.ILayer[] Layers)
        {
            Layers.Action((layerIndex, layer) => layer.AddObject(this));
        }
        public void UnRegisterLayers(params TabMap.ILayer[] Layers)
        {
            Layers.Action((layerIndex, layer) => layer.RemoveObject(this));
        }
        public void UnRegister()
        {
            Map.MapObjects.Remove(this);
        }
    }
    public class MapTile : MapObject
    {
        public MapTile(Map Map, Point Point, short Floor, short LeftWall, short RightWall) : base(Map, Point)
        {
            this.Map = Map;
            IsWall =
                LeftWall == 0 ?
                    RightWall == 0 ?
                        false :
                        sotp[RightWall - 1] == 0x0F :
                    RightWall == 0 ?
                        sotp[LeftWall - 1] == 0x0F :
                        sotp[LeftWall - 1] == 0x0F && sotp[RightWall - 1] == 0x0F;
        }
        private static readonly byte[] sotp = File.ReadAllBytes("sotp.dat");
        public MapTile[] AdjacentTiles { get; private set; }
        public virtual void OnMapLoaded() { AdjacentTiles = Point.SurroundingPoints.Select(p => Map[p]).Where(mapTile => mapTile != null).ToArray(); }
        public override void OnDraw(Graphics Graphics, Point Aspect) { if (IsWall) Graphics.FillRectangle(Brushes.Black, (Point * Aspect).ToRectangle(Aspect)); }
    }
    public class Map : PointGrid<MapTile>
    {
        public Map(string JSONFileName)
        {
            dynamic mapInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(JSONFileName));
            Bounds = new Point((int)mapInfo.Width, (int)mapInfo.Height);
            Grid = new MapTile[Bounds.X, Bounds.Y];
            Number = (int)mapInfo.Number;
            Name = (string)mapInfo.Name;
            string lodMapName = $"lod{mapInfo.Number}.map";
            string directory = Path.GetDirectoryName(JSONFileName);
            string lodMapPath = directory + "\\" + lodMapName;
            ReadFile(lodMapPath);
            TabMap = new TabMap(this);
            Floor = new FloorLayer(TabMap);
            TabMap.Layers.Add(Floor);
            Grid.Action(
                (x, y, mapTile) =>
                {
                    mapTile.OnMapLoaded();
                    mapTile.Register();
                    mapTile.RegisterLayers(Floor);
                }
            );
            PathFinder = new PathFinder(this);
            Loaded = true;
        }
        private bool Loaded { get; set; } = false;
        public string Name { get; private set; } = "";
        public int Number { get; private set; } = 0;
        public PathFinder PathFinder { get; private set; }
        public void ReadFile(string FileName)
        {
            using FileStream Stream = File.OpenRead(FileName);
            ReadSteam(Stream);
        }
        private readonly byte[] sotp = File.ReadAllBytes("sotp.dat");
        public void ReadSteam(Stream Stream)
        {
            using BinaryReader mapFileReader = new BinaryReader(Stream);
            Grid.Function((x, y, v) => new MapTile(this, new Point(x, y), mapFileReader.ReadInt16(), mapFileReader.ReadInt16(), mapFileReader.ReadInt16()));
        }
        public List<MapObject> MapObjects = new List<MapObject>();
        public TabMap TabMap { get; private set; }
        public class FloorLayer : TabMap.Layer<MapTile>
        {
            public FloorLayer(TabMap TabMap)
            {
                this.TabMap = TabMap;
                LayerHeights = new int[] { 100 };
            }
            private Point LastSize = Point.Zero;
            private bool Redraw = false;
            public override void OnBeginDraw()
            {
                Graphics = Graphics.FromImage(Bitmap);
                Point newSize = new Point(Bitmap.Size);
                if (newSize != LastSize)
                {
                    Redraw = true;
                    Graphics.Clear(Color.Transparent);
                }
                LastSize = newSize;
            }
            public override void OnDraw(int LayerHeight, MapTile MapObject)
            {
                if (Redraw)
                    base.OnDraw(LayerHeight, MapObject);
            }
        }
        public FloorLayer Floor;
    }
    public class TabMap
    {
        public TabMap(Map Map)
        {
            this.Map = Map;
        }
        public Map Map { get; set; }
        public Point Size { get { return Map.Bounds; } }

        private static Bitmap ResizeBitmap(Bitmap Bitmap, Point AspectSize)
        {
            if (Bitmap == null)
                return new Bitmap(AspectSize.X, AspectSize.Y);
            else
            {
                Bitmap newBitmap = new Bitmap(Bitmap, AspectSize.ToSize());
                Bitmap.Dispose();
                return newBitmap;
            }
        }

        public Point Aspect { get; set; } = Point.Zero;
        public Point AspectSize => Size * Aspect;
        public void SetAspectSize(Point AspectSize) { Aspect = AspectSize / Size; }
        public interface ILayer
        {
            int[] LayerHeights { get; }
            IEnumerable<MapObject> MapObjects { get; }
            void AddObject(MapObject MapObject);
            void RemoveObject(MapObject MapObject);
            Bitmap Bitmap { get; set; }
            float Opacity { get; }
            void OnBeginDraw();
            void OnBeginLayer(int LayerHeight);
            void OnDraw(int LayerHeight, MapObject MapObject);
            void OnEndDraw();
        }
        public abstract class Layer<T> : ILayer where T : MapObject
        {
            public TabMap TabMap;
            public int[] LayerHeights { get; set; } = new int[] { };
            public List<T> Objects { get; } = new List<T>();
            public void AddObject(MapObject MapObject) { AddObject(MapObject as T); }
            public void AddObject(T MapObject) { Objects.Add(MapObject); }
            public void RemoveObject(MapObject mapObject) { Objects.Remove(mapObject as T); }
            public void RemoveObject(T MapObject) { Objects.Remove(MapObject); }
            public IEnumerable<MapObject> MapObjects { get { return Objects.ToArray().Cast<MapObject>(); } }
            
            public Bitmap Bitmap { get; set; } = null;
            public float Opacity { get; set; } = 1;
            public Graphics Graphics { get; set; } = null;
            public virtual void OnBeginDraw() { Graphics = Graphics.FromImage(Bitmap); }
            public virtual void OnBeginLayer(int LayerHeight) { Graphics.Clear(Color.Transparent); }
            public void OnDraw(int LayerHeight, MapObject MapObject) { OnDraw(LayerHeight, MapObject as T); }
            public virtual void OnDraw(int LayerHeight, T MapObject) { MapObject.OnDraw(Graphics, TabMap.Aspect); }
            public virtual void OnEndDraw()
            {
                Graphics.Dispose();
            }
        }
        public List<ILayer> Layers = new List<ILayer>();
        public Bitmap Bitmap { get; set; } = null;
        private Point lastAspectSize { get; set; } = Point.Zero;
        public void Draw()
        {
            Point currentAspectSize = AspectSize;
            if (currentAspectSize <= Point.Zero) return;
            if (currentAspectSize != lastAspectSize)
                Bitmap = ResizeBitmap(Bitmap, currentAspectSize);
            lastAspectSize = currentAspectSize;
            System.Drawing.Point PointZero = Point.Zero.ToPoint();
            using Graphics graphics = Graphics.FromImage(Bitmap);
            graphics.Clear(Color.White);
            ILayer[] layers = Layers.ToArray().ToArray();
            layers.Action(
                (i, layer) =>
                {
                    if (layer.Bitmap == null)
                        layer.Bitmap = new Bitmap(currentAspectSize.X, currentAspectSize.Y);
                    else if (new Point(layer.Bitmap.Size) != currentAspectSize)
                        layer.Bitmap = ResizeBitmap(layer.Bitmap, currentAspectSize);
                    layer.OnBeginDraw();
                }
            );
            int[] LayerHeights = layers.SelectMany(layer => layer.LayerHeights)
                .Distinct()
                .OrderBy(layerHeight => layerHeight)
                .ToArray();
            LayerHeights.Action(
                (layerHeightIndex, layerHeight) =>
                {
                    ILayer[] layersWithinHeight = Layers.Where(layer => layer.LayerHeights.Contains(layerHeight)).ToArray();
                    layersWithinHeight.Action(
                        (layerArrayIndex, layer) =>
                        {
                            layer.OnBeginLayer(layerHeight);
                            layer.MapObjects.ToArray().Action(
                                (mapObjectArrayIndex, mapObject) =>
                                    layer.OnDraw(layerHeight, mapObject)
                            );
                            graphics.DrawImage(layer.Bitmap, PointZero);
                        }
                    );
                }
            );
            layers.Action((i, v) => v.OnEndDraw());
        }
    }
}