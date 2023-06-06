using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(menuName = "Scriptable Objects/World Data")]
    public class WorldData : ScriptableObject
    {
        [SerializeField] private bool saveExists = false; //Does a WorldSaveData file exist in the directory?
        [SerializeField] private bool saveIsLoaded = false; //Has the data from that file been loaded in?
        public bool SaveExists
        {
            get => saveExists;
            set => saveExists = value;
        }

        public bool IsLoaded
        {
            get => saveExists && saveIsLoaded;
            set => saveIsLoaded = value;
        }

        private int seed;
        public int Seed
        {
            get => seed; 
            set => seed = value;
        }

        [field: SerializeField] public TerrainData TerrainData { get; private set; }
        [field: SerializeField] public SettlementData SettlementData { get; private set; }
        private Grid<WorldTile> worldMap;

        public int Height => worldMap.Height;
        public int Width => worldMap.Width;

        public void CreateWorldGrid(int width, int height)
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

        public WorldTile GetNode(int x, int y) => worldMap.GetGridObject(x, y);

        public WorldTile GetNode(Vector3 worldPosition) => worldMap.GetGridObject(worldPosition);

        public Vector3 GetWorldPosition(int x, int y) => worldMap.GetWorldPosition(x, y);

        public int GetNodeDistance_Path(WorldTile fromNode, WorldTile toNode)
        {
            int x = Mathf.Abs(fromNode.x - toNode.x);
            int y = Mathf.Abs(fromNode.y - toNode.y);
            return x + y;
        }

        public List<WorldTile> GetNodesInRange_Circle(WorldTile fromNode, int range)
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

        public List<WorldTile> GetNodesInRange_Square(WorldTile fromNode, int range)
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
        private List<WorldTile> openList; //nodes to search
        private List<WorldTile> closedList; //already searched
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public int GetPathCount(WorldTile startNode, WorldTile endNode, bool allowDiagonals = false, Settlement settlement = null)
        {
            return (FindNodePath(startNode.x, startNode.y, endNode.x, endNode.y, allowDiagonals, settlement)).Count;
        }

        //Returns a list of nodes that can be travelled to reach a target destination
        public List<WorldTile> FindNodePath(int startX, int startY, int endX, int endY, bool allowDiagonals = false, Settlement settlement = null)
        {
            WorldTile startNode = worldMap.GetGridObject(startX, startY);
            //Debug.Log("Start: " + startNode.x + "," + startNode.y);
            WorldTile endNode = worldMap.GetGridObject(endX, endY);
            //Debug.Log("End: " + endNode.x + "," + endNode.y);

            openList = new List<WorldTile> { startNode };
            closedList = new List<WorldTile>();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    WorldTile pathNode = worldMap.GetGridObject(x, y);
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.cameFromTile = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                WorldTile currentNode = GetLowestFCostNode(openList);

                if (currentNode == endNode)
                {
                    //Reached final node
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (WorldTile neighbour in GetNeighbourList(currentNode, allowDiagonals))
                {
                    if (closedList.Contains(neighbour)) continue;

                    if (!neighbour.IsLand)// || neighbour.isOccupied)
                    {
                        //Debug.Log(startX + "," + startY +  ": Removing unwalkable/occupied tile " + neighbour.x + "," + neighbour.y);
                        closedList.Add(neighbour);
                        continue;
                    }

                    //Don't allow the passage of one settlement through another's territory
                    if (settlement != null)
                    {
                        var claimant = SettlementData.FindClaimedTerritory(neighbour.x, neighbour.y);
                        if (claimant != null && claimant != settlement)
                        {
                            closedList.Add(neighbour);
                            continue;
                        }
                    }

                    //Adding in movement cost here of the neighbor node to account for areas that are more difficult to move through
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbour) + neighbour.movementCost;

                    if (tentativeGCost < neighbour.gCost)
                    {
                        //If it's lower than the cost previously stored on the neightbor, update it
                        neighbour.cameFromTile = currentNode;
                        neighbour.gCost = tentativeGCost;
                        neighbour.hCost = CalculateDistanceCost(neighbour, endNode);
                        neighbour.CalculateFCost();

                        if (!openList.Contains(neighbour)) openList.Add(neighbour);
                    }
                }
            }

            //Out of nodes on the openList
            //Debug.Log("Path could not be found");
            return null;
        }

        private List<WorldTile> GetNeighbourList(WorldTile currentNode, bool allowDiagonals)
        {
            if (allowDiagonals) return currentNode.neighbors_all;
            return currentNode.neighbors_adj;
        }

        private int CalculateDistanceCost(WorldTile a, WorldTile b)
        {
            int xDistance = Mathf.Abs(a.x - b.x);
            int yDistance = Mathf.Abs(a.y - b.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private WorldTile GetLowestFCostNode(List<WorldTile> pathNodeList)
        {
            WorldTile lowestFCostNode = pathNodeList[0];

            for (int i = 0; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                    lowestFCostNode = pathNodeList[i];
            }

            return lowestFCostNode;
        }

        private List<WorldTile> CalculatePath(WorldTile endNode)
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