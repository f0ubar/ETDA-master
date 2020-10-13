using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;

namespace Dreamer.Mapping
{
    
    
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
                Opacity.Add(100, 1);
                Redraw = true;
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
            public override void OnDraw(int LayerHeight, MapTile MapObject) { if (Redraw) base.OnDraw(LayerHeight, MapObject); }
        }
        public FloorLayer Floor;
    }
}