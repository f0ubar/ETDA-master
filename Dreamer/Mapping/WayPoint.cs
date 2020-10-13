using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dreamer.Mapping
{
    public class WayPoint
    {
        public WayPoint(Map Map, Point Point, PathFinder.PathLayer PathLayer)
        {
            this.Map = Map;
            this.Point = Point;
            this.PathLayer = PathLayer;
        }
        public Map Map { get; set; }
        public Point Point { get; set; }
        public WayPoint PreviousWayPoint { get; set; }
        public WayPoint NextWayPoint { get; set; }
        public PathFinderNode Path { get; set; }
        public PathFinder.PathLayer PathLayer { get; set; }
        public void Connect(WayPoint NewWayPoint)
        {
            PathFinderNode currentNode = Path;
            while (currentNode != null)
            {
                currentNode.RemoveFromLayer();
                currentNode = currentNode.NextNode;
            }
            if (NextWayPoint != null && NextWayPoint.PreviousWayPoint == this)
                NextWayPoint.PreviousWayPoint = null;
            
            Path = Map.PathFinder.FindPath(Point, NewWayPoint.Point);
            if (Path != null)
            {
                Path.PathLayer = PathLayer;
                NextWayPoint = NewWayPoint;
                NewWayPoint.PreviousWayPoint = this;
                currentNode = Path;
                while (currentNode != null)
                {
                    PathLayer.AddObject(currentNode);
                    currentNode = currentNode.NextNode;
                }
            }
        }
        public void Disconnect()
        {
            PathFinderNode currentNode = Path;
            while (currentNode != null)
            {
                currentNode.RemoveFromLayer();
                currentNode = currentNode.NextNode;
            }
        }
    }
}
