using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Dreamer.Mapping
{
    public class MapTile : MapObject
    {
        public MapTile(Map Map, Point Point, short Floor, short LeftWall, short RightWall) : base(Map, Point)
        {
            this.Map = Map;
            this.Floor = Floor;
            this.LeftWall = LeftWall;
            this.RightWall = RightWall;
            IsWall =
                LeftWall == 0 ?
                    RightWall != 0 && sotp[RightWall - 1] == 0x0F :
                    RightWall == 0 ?
                        sotp[LeftWall - 1] == 0x0F :
                        sotp[LeftWall - 1] == 0x0F && sotp[RightWall - 1] == 0x0F;
            IsDrawable = true;
        }
        public short Floor { get; set; } = 0;
        public short LeftWall { get; set; } = 0;
        public short RightWall { get; set; } = 0;
        private static readonly byte[] sotp = File.ReadAllBytes("sotp.dat");
        public MapTile[] AdjacentTiles { get; private set; }
        public virtual void OnMapLoaded() { AdjacentTiles = Point.SurroundingPoints.Select(p => Map[p]).Where(mapTile => mapTile != null).ToArray(); }
        public override void OnDraw(Graphics Graphics, Point Aspect, int LayerHeight) { if (IsWall) Graphics.FillRectangle(Brushes.Black, (Point * Aspect).ToRectangle(Aspect)); }
    }
}
