using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dreamer.Mapping
{
    public class PathFinder
    {
        public PathFinder(Map Map)
        {
            this.Map = Map;
            WayPointLayer = new PathLayer(Map.TabMap);
        }
        public Map Map;
        public class PathLayer : TabMap.Layer<PathFinderNode>
        {
            public PathLayer(TabMap TabMap)
            {
                this.TabMap = TabMap;
                LayerHeights = new int[] { 25, 50, 75, 125, 150 };
                Opacity.Add(25, 1f);
                Opacity.Add(50, 1f);
                Opacity.Add(75, 1f);
                Opacity.Add(125, 1f);
                Opacity.Add(150, 1f);
                Redraw = true;
                TabMap.Layers.Add(this);
            }
            private Point LastSize = Point.Zero;
            private bool Redraw = false;
            public override void AddObject(PathFinderNode MapObject)
            {
                MapObject.PathLayer = this;
                base.AddObject(MapObject);
            }
            public override void OnBeginDraw()
            {
                SkipDraw = false;
                Graphics = Graphics.FromImage(Bitmap);
                Point newSize = new Point(Bitmap.Size);
                if (newSize != LastSize)
                {
                    Redraw = true;
                    Graphics.Clear(Color.Transparent);
                }
                LastSize = newSize;
            }
            public override void OnDraw(int LayerHeight, PathFinderNode Node) { if (Redraw) base.OnDraw(LayerHeight, Node); }
        }
        public PathFinderNode FindPath(Point Begin, Point End)
        {
            PathFinderNode[,] nodeGrid = new PathFinderNode[Map.Bounds.X, Map.Bounds.Y];
            nodeGrid.Function((x, y, v) => new PathFinderNode(Map[new Point(x, y)]));
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
            IEnumerable<PathFinderNode> openNodeQuery = nodeGrid.Cast<PathFinderNode>().Where(node => node.Opened && !node.Closed);
            PathFinderNode[] openNodes = openNodeQuery.ToArray();
            PathFinderNode endNode = openNodes.FirstOrDefault(node => node.Point == End);
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
            {
                return endNode.BackConnectToFirstNode();
            }
            return null;
        }
        public List<WayPoint> WayPoints = new List<WayPoint>();
        PathLayer WayPointLayer;

        public void AddWayPoint(Point Point)
        {
            WayPoint newWayPoint = new WayPoint(Map, Point, WayPointLayer);
            if (WayPoints.Count > 0)
            {
                WayPoints[WayPoints.Count - 1].Connect(newWayPoint);
                newWayPoint.Connect(WayPoints[0]);
            }
            WayPoints.Add(newWayPoint);
        }
        public void RemoveWayPoint(Point Point)
        {
            WayPoint[] wayPoints = WayPoints.Where(wayPoint => wayPoint.Point == Point).ToArray();
            wayPoints.Action(
                (i, wayPoint) => {
                    WayPoints.Remove(wayPoint);
                    wayPoint.Disconnect();
                    if (wayPoint.PreviousWayPoint != null && wayPoint.NextWayPoint != null)
                        wayPoint.PreviousWayPoint.Connect(wayPoint.NextWayPoint);
                    wayPoint.PathLayer.RemoveObject(wayPoint.Path);
                }
            );
            Map.TabMap.Draw();
        }
    }
}
