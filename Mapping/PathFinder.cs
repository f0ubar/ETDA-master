using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mapping
{
    public class PathFinder
    {
        public PathFinder(Map Map)
        {
            this.Map = Map;
        }
        public Map Map;
        public class Node : MapObject
        {
            public Node(PathFinder PathFinder, MapTile MapTile) : base(MapTile.Map, MapTile.Point)
            {
                this.MapTile = MapTile;
                Blocked = MapTile.IsWall;
                IsDrawable = true;
            }
            public MapTile MapTile { get; set; }
            public Node PreviousNode { get; set; }
            public Node NextNode { get; set; }
            public bool Opened { get; set; } = false;
            public bool Closed { get; set; } = false;
            public bool Blocked { get; set; } = false;
            public Node[] AdjacentNodes { get; set; }
            public Direction Direction { get { return NextNode == null ? Direction.None : MapTile.Point.DirectDirectionTowards(NextNode.Point); } }
            
            public Node BackConnectToFirstNode()
            {
                Node node = this;
                while (node.PreviousNode != null) node = (node.PreviousNode.NextNode = node).PreviousNode;
                return node;
            }
            public Node FirstNode
            {
                get
                {
                    Node node = this;
                    while (node.PreviousNode != null) node = node.PreviousNode;
                    return node;
                }
            }
            public Node LastNode
            {
                get
                {
                    Node node = this;
                    while (node.NextNode != null) node = node.NextNode;
                    return node;
                }
            }
            public Node FirstNodeWhere(Func<Node, bool> func)
            {
                Node node = this;
                while (node != null)
                {
                    if (func(node))
                        return node;
                    node = node.NextNode;
                }
                return null;
            }
            public Node FirstAtPoint(Point Point)
            {
                return FirstNodeWhere(o => Point == o.Point);
            }
            public Node FirstNodeBackwardsWhere(Func<Node, bool> func)
            {
                Node node = this;
                while (node != null)
                {
                    if (func(node))
                        return node;
                    node = node.PreviousNode;
                }
                return null;
            }
            static Brush SightBrush = new SolidBrush(Color.FromArgb(200, 173, 216, 230));
            public override void OnDrawBackground(Graphics Graphics, Point Aspect)
            {
                Rectangle sightRectangle = ((Point - new Point(2, 2)) * Aspect).ToRectangle(new Point(5, 5) * Aspect);
                Graphics.FillRectangle(SightBrush, sightRectangle);
            }
            static Brush PointBrush = new SolidBrush(Color.Blue);
            public override void OnDraw(Graphics Graphics, Point Aspect)
            {
                Rectangle rectangle = (Point * Aspect).ToRectangle(Aspect);
                Graphics.FillRectangle(PointBrush, rectangle);
            }
        }
        public Node FindPath(Point Begin, Point End)
        {
            Node[,] nodeGrid = new Node[Map.Bounds.X, Map.Bounds.Y];
            nodeGrid.Function((x, y, v) => new Node(this, Map[new Point(x, y)]));
            Map.MapObjects.ToArray().Action(
                (i, mapObject) =>
                {
                    if (mapObject.IsWall)
                        nodeGrid[mapObject.Point.X, mapObject.Point.Y].Blocked = true;
                }
            );
            nodeGrid.Action(
                (x, y, node) =>
                    node.AdjacentNodes =
                        node.MapTile.AdjacentTiles
                        .Select(mapTile => nodeGrid[mapTile.Point.X, mapTile.Point.Y])
                        .Where(node => !node.Blocked)
                        .ToArray()
            );
            nodeGrid[Begin.X, Begin.Y].Opened = true;
            IEnumerable<Node> openNodeQuery = nodeGrid.Cast<Node>().Where(node => node.Opened && !node.Closed);
            Node[] openNodes = openNodeQuery.ToArray();
            Node endNode = openNodes.FirstOrDefault(node => node.Point == End);
            while (openNodes.Length > 0)
            {
                openNodes.Action(
                    (i, openNode) =>
                    {
                        openNode.Closed = true;
                        openNode.AdjacentNodes.Action(
                            (i, adjacentNode) =>
                            {
                                if (!adjacentNode.Opened && !adjacentNode.Closed)
                                {
                                    adjacentNode.Opened = true;
                                    adjacentNode.PreviousNode = openNode;
                                }
                            }
                        );
                    }
                );
                if ((endNode = openNodes.FirstOrDefault(node => node.Point == End)) != null) break;
                openNodes = openNodeQuery.ToArray();
            }
            if (endNode != null)
                return endNode.BackConnectToFirstNode();
            return null;
        }
    }
}
