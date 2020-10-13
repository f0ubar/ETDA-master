using System;
using System.Collections;
using System.Drawing;
using System.Linq;

namespace Dreamer.Mapping
{
    public class PathFinderNode : MapObject
    {
        public PathFinderNode(MapTile MapTile) : base(MapTile.Map, MapTile.Point)
        {
            this.MapTile = MapTile;
            Blocked = MapTile.IsWall;
            IsDrawable = true;
        }
        public MapTile MapTile { get; set; }
        public PathFinder.PathLayer PathLayer { get; set; }
        public PathFinderNode PreviousNode { get; set; }
        public PathFinderNode NextNode { get; set; }
        public bool Opened { get; set; } = false;
        public bool Closed { get; set; } = false;
        public bool Blocked { get; set; } = false;
        public PathFinderNode[] AdjacentNodes { get; set; }
        public Direction Direction { get { return NextNode == null ? Direction.None : MapTile.Point.DirectDirectionTowards(NextNode.Point); } }

        public PathFinderNode BackConnectToFirstNode()
        {
            PathFinderNode node = this;
            while (node.PreviousNode != null) node = (node.PreviousNode.NextNode = node).PreviousNode;
            return node;
        }
        public PathFinderNode FirstNode
        {
            get
            {
                PathFinderNode node = this;
                while (node.PreviousNode != null) node = node.PreviousNode;
                return node;
            }
        }
        public PathFinderNode LastNode
        {
            get
            {
                PathFinderNode node = this;
                while (node.NextNode != null) node = node.NextNode;
                return node;
            }
        }
        public PathFinderNode FirstNodeWhere(Func<PathFinderNode, bool> func)
        {
            PathFinderNode node = this;
            while (node != null)
            {
                if (func(node))
                    return node;
                node = node.NextNode;
            }
            return null;
        }
        public PathFinderNode FirstAtPoint(Point Point)
        {
            return FirstNodeWhere(o => Point == o.Point);
        }
        public PathFinderNode FirstNodeBackwardsWhere(Func<PathFinderNode, bool> func)
        {
            PathFinderNode node = this;
            while (node != null)
            {
                if (func(node))
                    return node;
                node = node.PreviousNode;
            }
            return null;
        }
        static readonly Brush
            ScreenBrush = new SolidBrush(Color.FromArgb(158, 195, 255)),
            SightBrush = new SolidBrush(Color.FromArgb(102, 161, 255)),
            PickupBrush = new SolidBrush(Color.FromArgb(60, 100, 255)),
            LineBrush = new SolidBrush(Color.FromArgb(0, 0, 255)),
            PointBrush = new SolidBrush(Color.Orange);
        public override void OnDrawBackground(Graphics Graphics, Point Aspect)
        {
            
        }
        public override void OnDraw(Graphics Graphics, Point Aspect, int LayerHeight)
        {
            switch (LayerHeight)
            {
                case 25:    
                    Graphics.FillPolygon(ScreenBrush, (Aspect * (Point + Shape.GetRhombus(8))).Select(point => point.ToPoint()).ToArray());
                    break;
                case 50:
                    Graphics.FillRectangle(SightBrush, ((Point - new Point(4, 4)) * Aspect).ToRectangle(Aspect * new Point(9, 9)));
                    break;
                case 75:
                    Graphics.FillRectangle(PickupBrush, ((Point - new Point(2, 2)) * Aspect).ToRectangle(Aspect * new Point(5, 5)));
                    break;
                case 125:
                    Graphics.FillRectangle(LineBrush, (Point * Aspect).ToRectangle(Aspect));
                    break;
                case 150:
                    if (FirstNode == this || LastNode == this)
                    {
                        Graphics.FillRectangle(PointBrush, (FirstNode.Point * Aspect).ToRectangle(Aspect));
                        Graphics.FillRectangle(PointBrush, (LastNode.Point * Aspect).ToRectangle(Aspect));
                    }
                    break;
            }
        }
        private class NodeEnumerator : IEnumerator
        {
            public NodeEnumerator(PathFinderNode CurrentNode)
            {
                FirstNode = this.CurrentNode = CurrentNode;
            }
            private PathFinderNode FirstNode { get; set; }
            private PathFinderNode CurrentNode { get; set; }
            public object Current { get { return CurrentNode; } }
            public bool MoveNext()
            {
                CurrentNode = CurrentNode.NextNode;
                return CurrentNode != null;
            }
            public void Reset() { CurrentNode = FirstNode; }
        }
        public IEnumerator GetEnumerator()
        {
            return new NodeEnumerator(this);
        }
        public void RemoveFromLayer()
        {
            if (PathLayer != null)
                PathLayer.RemoveObject(this);
        }
    }
}
