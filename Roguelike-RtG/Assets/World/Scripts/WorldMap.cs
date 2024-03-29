using System.Collections.Generic;
using UnityEngine;

namespace JS.World.Map
{
    public static class WorldMap
    {
        private static bool saveExists = false; //Does a WorldSaveData file exist in the directory?
        private static bool saveIsLoaded = false; //Has the data from that file been loaded in?
        public static bool SaveExists
        {
            get => saveExists;
            set => saveExists = value;
        }

        public static bool IsLoaded
        {
            get => saveExists && saveIsLoaded;
            set => saveIsLoaded = value;
        }

        private static int seed;
        public static int Seed
        {
            get => seed; 
            set => seed = value;
        }

        private static Grid<WorldTile> worldMap;

        public static int Height => worldMap.Height;
        public static int Width => worldMap.Width;

        public static void CreateWorldGrid(int width, int height)
        {
            worldMap = new Grid<WorldTile>(width, height, 1, Vector3.zero, (Grid<WorldTile> g, int x, int y) => new WorldTile(g, x, y));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    worldMap.GetGridObject(x, y).SetNeighbors();
                }
            }
        }

        public static WorldTile GetNode(int x, int y) => worldMap.GetGridObject(x, y);

        public static WorldTile GetNode(Vector3 worldPosition) => worldMap.GetGridObject(worldPosition);

        public static Vector3 GetWorldPosition(int x, int y) => worldMap.GetWorldPosition(x, y);

        public static int GetNodeDistance_Path(WorldTile fromNode, WorldTile toNode)
        {
            int x = Mathf.Abs(fromNode.x - toNode.x);
            int y = Mathf.Abs(fromNode.y - toNode.y);
            return x + y;
        }

        public static List<WorldTile> GetNodesInRange_Circle(WorldTile fromNode, int range)
        {
            var nodes = new List<WorldTile>();

            for (int x = fromNode.x - range; x < fromNode.x + range + 1; x++)
            {
                for (int y = fromNode.y - range; y < fromNode.y + range + 1; y++)
                {
                    if (x < 0 || x > worldMap.Width - 1) continue;
                    if (y < 0 || y > worldMap.Height - 1) continue;

                    var toNode = worldMap.GetGridObject(x, y);
                    if (GridMath.GetStraightDist(fromNode.x, fromNode.y, toNode.x, toNode.y) <= range) nodes.Add(toNode);
                }
            }
            return nodes;
        }

        public static List<WorldTile> GetNodesInRange_Square(WorldTile fromNode, int range)
        {
            var nodes = new List<WorldTile>();

            for (int x = fromNode.x - range; x < fromNode.x + range + 1; x++)
            {
                for (int y = fromNode.y - range; y < fromNode.y + range + 1; y++)
                {
                    if (x < 0 || x > worldMap.Width - 1) continue;
                    if (y < 0 || y > worldMap.Height - 1) continue;

                    var toNode = worldMap.GetGridObject(x, y);
                    nodes.Add(toNode);
                }
            }
            return nodes;
        }

        #region - Pathfinding -
        private static Heap<WorldTile> openList; //nodes to search
        //private List<WorldTile> openList; //nodes to search
        private static HashSet<WorldTile> closedSet; //already searched
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public static int GetPathCount(WorldTile startNode, WorldTile endNode, bool allowDiagonals = false, Settlement settlement = null)
        {
            return (FindNodePath(startNode.x, startNode.y, endNode.x, endNode.y, allowDiagonals, settlement)).Count;
        }

        //Returns a list of nodes that can be travelled to reach a target destination
        public static List<WorldTile> FindNodePath(int startX, int startY, int endX, int endY, bool allowDiagonals = false, Settlement settlement = null)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            WorldTile startNode = worldMap.GetGridObject(startX, startY);
            WorldTile endNode = worldMap.GetGridObject(endX, endY);

            openList = new Heap<WorldTile>(worldMap.MaxSize);
            //openList = new List<WorldTile> { startNode };
            closedSet = new HashSet<WorldTile>();
            openList.Add(startNode);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    WorldTile pathNode = worldMap.GetGridObject(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.cameFromTile = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = GetDistance(startNode, endNode);

            while (openList.Count > 0)
            {
                WorldTile currentNode = openList.RemoveFirst();
                closedSet.Add(currentNode);

                //Reached final node
                if (currentNode == endNode)
                {
                    //sw.Stop();
                    //UnityEngine.Debug.Log("Path found: " + sw.ElapsedMilliseconds + "ms");
                    return CalculatePath(endNode);
                }

                foreach (WorldTile neighbour in GetNeighbourList(currentNode, allowDiagonals))
                {
                    if (!neighbour.IsLand || closedSet.Contains(neighbour)) continue;

                    /*if (!neighbour.IsLand)
                    {
                        //Debug.Log(startX + "," + startY +  ": Removing unwalkable/occupied tile " + neighbour.x + "," + neighbour.y);
                        closedSet.Add(neighbour);
                        continue;
                    }*/

                    //Don't allow the passage of one settlement through another's territory
                    if (settlement != null)
                    {
                        var claimant = SettlementData.FindClaimedTerritory(neighbour.x, neighbour.y);
                        if (claimant != null && claimant != settlement)
                        {
                            closedSet.Add(neighbour);
                            continue;
                        }
                    }

                    //Adding in movement cost here of the neighbor node to account for areas that are more difficult to move through
                    int tentativeGCost = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementCost;

                    if (tentativeGCost < neighbour.gCost)
                    {
                        //If it's lower than the cost previously stored on the neightbor, update it
                        neighbour.cameFromTile = currentNode;
                        neighbour.gCost = tentativeGCost;
                        neighbour.hCost = GetDistance(neighbour, endNode);

                        if (!openList.Contains(neighbour)) openList.Add(neighbour);
                        else openList.UpdateItem(neighbour);
                    }
                }
            }

            //Out of nodes on the openList
            //Debug.Log("Path could not be found");
            return null;
        }

        private static List<WorldTile> GetNeighbourList(WorldTile currentNode, bool allowDiagonals)
        {
            if (allowDiagonals) return currentNode.neighbors_all;
            return currentNode.neighbors_adj;
        }

        private static int GetDistance(WorldTile a, WorldTile b)
        {
            int xDistance = Mathf.Abs(a.x - b.x);
            int yDistance = Mathf.Abs(a.y - b.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private static List<WorldTile> CalculatePath(WorldTile endNode)
        {
            List<WorldTile> path = new List<WorldTile>();
            path.Add(endNode);
            WorldTile currentNode = endNode;
            while (currentNode.cameFromTile != null)
            {
                //Start at the end and work backwards
                path.Add(currentNode.cameFromTile);
                currentNode = currentNode.cameFromTile;
            }
            path.Reverse();
            return path;
        }
        #endregion
    }
}