using Binarysharp.MemoryManagement.Native;
using BotCore.Components;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BotCore
{
    public class SonarV2
    {
        public SonarV2(Client Client)
        {
            this.Client = Client;
        }
        Client Client;
        Position Position { get { return Client.Attributes.ServerPosition; } }
        Map Map { get { return Client.FieldMap; } }
        IEnumerable<MapObject> Obstacles
        {
            get
            {
                return Map.MapObjects.Where(
                    o =>
                        o.Type == MapObjectType.Aisling |
                        o.Type == MapObjectType.Monster |
                        o.Type == MapObjectType.NPC
                );
            }
        }
        public class Node
        {
            public Node(Position Position, int NodeNum = 0)
            {
                this.Position = Position;
                if (Position.X < 0 || Position.Y < 0)
                    throw new Exception();
                this.NodeNumber = NodeNum;
            }
            public Position Position;
            public Node PreviousNode, NextNode;
            public bool Closed;
            public int NodeNumber;
            public int Attempts = 0;
            public Direction Direction
            {
                get
                {
                    return DirectionTo(NextNode);
                }
            }
            public Direction DirectionTo(Node Node)
            {
                if (Node == null)
                    return Direction.None;
                if (Position.X == Node.Position.X && Position.Y == Node.Position.Y - 1)
                    return Direction.South;
                else if (Position.X == Node.Position.X && Position.Y == Node.Position.Y + 1)
                    return Direction.North;
                else if (Position.X == Node.Position.X - 1 && Position.Y == Node.Position.Y)
                    return Direction.East;
                else if (Position.X == Node.Position.X + 1 && Position.Y == Node.Position.Y)
                    return Direction.West;
                else return Direction.None;
            }
            public Node BackConnectToFirstNode()
            {
                Node node = this;
                while (node.PreviousNode != null)
                {
                    Node previousNode = node.PreviousNode;
                    previousNode.NextNode = node;
                    node = previousNode;
                }
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
                Node node = FirstNode;
                while (node != null)
                {
                    if (func(node))
                        return node;
                    node = node.NextNode;
                }
                return null;
            }
            public Node FirstAtPosition(Position Position)
            {
                return FirstNodeWhere(o => Position.DistanceFrom(o.Position) == 0);
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
        }
        bool[,] ObstacleMap
        {
            get
            {
                bool[,] obstacleMap = new bool[Map.Width, Map.Height];
                Obstacles.ToList().ForEach(o => obstacleMap[o.ServerPosition.X, o.ServerPosition.Y] = true);
                return obstacleMap;
            }
        }
        public Node FindPathTo(Position End)
        {
            bool[,] obstacleMap = ObstacleMap;
            if (!UnBlockedLocation(End, obstacleMap)) return null;
            Node[,] nodeArray = new Node[Map.Width, Map.Height];
            nodeArray[Position.X, Position.Y] = new Node(new Position(Position.X, Position.Y));
            IEnumerable<Node> openNodeQuery = nodeArray.Cast<Node>().Where(node => node != null && node.Closed == false).OrderBy(o => o.NodeNumber);
            Node[] openNodes = openNodeQuery.ToArray();
            int nodeNum = 1;
            while (openNodes.Length > 0)
            {
                // Priority for maintaining direction
                for (int i = 0; i < openNodes.Length; i++)
                {
                    Node openNode = openNodes[i];
                    openNode.Closed = true;
                    if (openNode.Position.X == End.X && openNode.Position.Y == End.Y)
                    {
                        return openNode.BackConnectToFirstNode();
                    }
                    Direction direction = openNode.DirectionTo(openNode.PreviousNode);
                    if (direction != Direction.None)
                        if (OpenNewNodes(obstacleMap, nodeArray, openNode, openNode.Position + direction, nodeNum)) nodeNum++;
                }
                // Open other directions last
                for (int i = 0; i < openNodes.Length; i++)
                {
                    Node openNode = openNodes[i];
                    if (OpenNewNodes(obstacleMap, nodeArray, openNode, new Position(openNode.Position.X - 1, openNode.Position.Y), nodeNum)) nodeNum++;
                    if (OpenNewNodes(obstacleMap, nodeArray, openNode, new Position(openNode.Position.X, openNode.Position.Y - 1), nodeNum)) nodeNum++;
                    if (OpenNewNodes(obstacleMap, nodeArray, openNode, new Position(openNode.Position.X + 1, openNode.Position.Y), nodeNum)) nodeNum++;
                    if (OpenNewNodes(obstacleMap, nodeArray, openNode, new Position(openNode.Position.X, openNode.Position.Y + 1), nodeNum)) nodeNum++;
                }
                openNodes = openNodeQuery.ToArray();
            }
            return null;
        }

        private bool UnBlockedLocation(Position pos, bool[,] obstacleMap, Node[,] nodeArray = null)
        {
            return
                pos.X >= 0 && pos.Y >= 0 &&
                pos.X < Map.Width && pos.Y < Map.Height &&
                !Map.IsWall(pos.X, pos.Y) &&
                !obstacleMap[pos.X, pos.Y] &&
                (nodeArray == null || nodeArray[pos.X, pos.Y] == null);
        }
        private bool OpenNewNodes(bool[,] obstacleMap, Node[,] nodeArray, Node openNode, Position pos, int NodeNum)
        {
            if (UnBlockedLocation(pos, obstacleMap, nodeArray))
            {
                nodeArray[pos.X, pos.Y] = new Node(pos, NodeNum) { PreviousNode = openNode };
                return true;
            }
            return false;
        }
    }
}
