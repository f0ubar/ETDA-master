using System.Collections.Generic;
using System.Drawing;

namespace Dreamer.Mapping
{
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
        public virtual void OnDraw(Graphics Graphics, Point Aspect, int LayerHeight) { }
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
}
