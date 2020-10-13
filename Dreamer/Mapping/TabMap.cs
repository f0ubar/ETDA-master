using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Dreamer.Mapping
{
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
            bool SkipDraw { get; }
            int[] LayerHeights { get; }
            IEnumerable<MapObject> MapObjects { get; }
            void AddObject(MapObject MapObject);
            void RemoveObject(MapObject MapObject);
            Bitmap Bitmap { get; set; }
            Dictionary<int, float> Opacity { get; }
            void OnBeginDraw();
            void OnBeginLayer(int LayerHeight);
            void OnDraw(int LayerHeight, MapObject MapObject);
            void OnEndDraw();
        }
        public abstract class Layer<T> : ILayer where T : MapObject
        {
            public TabMap TabMap;
            public bool SkipDraw { get; set; } = false;
            public int[] LayerHeights { get; set; } = new int[] { };
            public List<T> Objects { get; } = new List<T>();
            public void AddObject(MapObject MapObject) { AddObject(MapObject as T); }
            public virtual void AddObject(T MapObject) { Objects.Add(MapObject); }
            public void RemoveObject(MapObject mapObject) { Objects.Remove(mapObject as T); }
            public void RemoveObject(T MapObject) { Objects.Remove(MapObject); }
            public IEnumerable<MapObject> MapObjects { get { return Objects.ToArray().Cast<MapObject>(); } }

            public Bitmap Bitmap { get; set; } = null;
            public Dictionary<int, float> Opacity { get; set; } = new Dictionary<int, float>();
            public Graphics Graphics { get; set; } = null;
            public virtual void OnBeginDraw() { Graphics = Graphics.FromImage(Bitmap); }
            public virtual void OnBeginLayer(int LayerHeight) { Graphics.Clear(Color.Transparent); }
            public void OnDraw(int LayerHeight, MapObject MapObject) { OnDraw(LayerHeight, MapObject as T); }
            public virtual void OnDraw(int LayerHeight, T MapObject) { MapObject.OnDraw(Graphics, TabMap.Aspect, LayerHeight); }
            public virtual void OnEndDraw()
            {
                Graphics.Dispose();
            }
        }
        public List<ILayer> Layers = new List<ILayer>();
        public Bitmap Bitmap { get; set; } = null;
        private Point LastAspectSize { get; set; } = Point.Zero;
        public void Draw()
        {
            Point currentAspectSize = AspectSize;
            if (currentAspectSize <= Point.Zero) return;
            if (currentAspectSize != LastAspectSize)
                Bitmap = ResizeBitmap(Bitmap, currentAspectSize);
            LastAspectSize = currentAspectSize;
            System.Drawing.Point PointZero = Point.Zero.ToPoint();
            using Graphics graphics = Graphics.FromImage(Bitmap);
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
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
                    ILayer[] layersWithinHeight = Layers.Where(layer => !layer.SkipDraw && layer.LayerHeights.Contains(layerHeight)).ToArray();
                    layersWithinHeight.Action(
                        (layerArrayIndex, layer) =>
                        {
                            if (!layer.SkipDraw)
                            {
                                layer.OnBeginLayer(layerHeight);
                                layer.MapObjects.Where(mapObject => mapObject.IsDrawable).ToArray().Action(
                                    (mapObjectArrayIndex, mapObject) =>
                                        { if (!layer.SkipDraw) layer.OnDraw(layerHeight, mapObject); }
                                );
                                ColorMatrix matrix = new ColorMatrix
                                {
                                    Matrix33 = layer.Opacity.ContainsKey(layerHeight) ? layer.Opacity[layerHeight] : 1
                                };
                                ImageAttributes attributes = new ImageAttributes();
                                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                Rectangle rectangle = Point.Zero.ToRectangle(new Point(layer.Bitmap.Size));
                                graphics.DrawImage(layer.Bitmap, rectangle, 0f, 0f, rectangle.Width, rectangle.Height, GraphicsUnit.Pixel, attributes);
                            }
                        }
                    );
                }
            );
            layers.Action((i, v) => v.OnEndDraw());
        }
    }
}
