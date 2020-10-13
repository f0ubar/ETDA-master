using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mapping
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private Map Map;
        private Bitmap Bitmap;

        private void loadMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON Map Files|*.map.json";
            DialogResult result = ofd.ShowDialog(this);
            if (result == DialogResult.OK && ofd.FileName.ToLower().EndsWith(".map.json") && ofd.CheckFileExists)
            {
                Map = new Map(ofd.FileName);
                DrawBitmap();
            }
        }

        private void DrawBitmap()
        {
            if (Map == null) return;
            CheckPictureSize();
            Map.TabMap.Draw();
            pictureBox1.Image = Map.TabMap.Bitmap;
        }

        public class WayPoint : MapObject
        {
            public WayPoint(Map Map, Point Point) : base(Map, Point) { IsDrawable = true; }
            public List<PathFinder.Node> PathNodes = new List<PathFinder.Node>();
            private WayPoint _nextWayPoint;
            public WayPoint NextWayPoint
            {
                get
                {
                    return NextWayPoint;
                }
                set
                {
                    _nextWayPoint = value;
                    PathFinder.Node path = Map.PathFinder.FindPath(Point, value.Point);
                    while (path != null)
                    {
                        PathNodes.Add(path);
                        path = path.NextNode;
                    }
                }
            }
            public override void OnDraw(Graphics Graphics, Point Aspect)
            {
                Graphics.FillRectangle(Brushes.Yellow, (Point * Aspect).ToRectangle(Aspect));
            }
        }


        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            DrawBitmap();
        }

        private void CheckPictureSize()
        {
            Point pictureSize = new Point(pictureBox1.Size);
            if (pictureSize.X < pictureSize.Y)
                Map.TabMap.Aspect = new Point(pictureSize.X / Map.Bounds.X, pictureSize.X / Map.Bounds.X);
            else
                Map.TabMap.Aspect = new Point(pictureSize.Y / Map.Bounds.Y, pictureSize.Y / Map.Bounds.Y);
        }

        public List<WayPoint> WayPoints = new List<WayPoint>();

        public void AddWayPoint(Point Point)
        {
            WayPoint newWayPoint = new WayPoint(Map, Point);
            if (WayPoints.Count > 0)
            {
                var lastWayPoint = WayPoints[WayPoints.Count - 1];
                ConnectWayPoints(lastWayPoint, newWayPoint);
                ConnectWayPoints(newWayPoint, WayPoints[0]);
            }
            WayPoints.Add(newWayPoint);
        }

        private void ConnectWayPoints(WayPoint A, WayPoint B)
        {
            foreach (var p in A.PathNodes)
                Map.MapObjects.Remove(p);
            A.PathNodes.Clear();
            PathFinder.Node path = Map.PathFinder.FindPath(A.Point, B.Point);
            while (path != null)
            {
                Map.MapObjects.Add(path);
                A.PathNodes.Add(path);
                path = path.NextNode;
            }
            A.NextWayPoint = B;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Map != null)
            {
                int X = (int)(((float)e.X / Map.TabMap.Bitmap.Width) * Map.Bounds.X),
                   Y = (int)(((float)e.Y / Map.TabMap.Bitmap.Height) * Map.Bounds.X);
                Point point = new Point(X, Y);
                if (Map.PointInBounds(point))
                    AddWayPoint(new Point(X, Y));
                DrawBitmap();
            }
        }

        private void removeSelectedWayPoint_Click(object sender, EventArgs e)
        {
            if (wayPointList.SelectedItems.Count > 0)
            {
                for (int i = wayPointList.SelectedItems.Count - 1; i >= 0; i--)
                {
                    wayPointList.Items.Remove(wayPointList.SelectedItems[i]);
                }
            }
        }
    }
}
