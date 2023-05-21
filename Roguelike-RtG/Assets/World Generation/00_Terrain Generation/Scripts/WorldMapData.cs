using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(menuName = "Scriptable Objects/World Map Data")]
    public class WorldMapData : ScriptableObject
    {
        private int seed;
        public int Seed
        {
            get => seed; set => seed = value;
        }

        [field: SerializeField] public TerrainData TerrainData { get; private set; }
        [field: SerializeField] public SettlementData SettlementData { get; private set; }
        private Grid<WorldTile> grid;

        public int Height => grid.GetHeight();
        public int Width => grid.GetWidth();

        public void CreateGrid(int width, int height)
        {
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;
            Vector3 origin = new Vector3(-halfWidth, -halfHeight);

            grid = new Grid<WorldTile>(width, height, 1, origin, (Grid<WorldTile> g, int x, int y) => new WorldTile(g, x, y));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid.GetGridObject(x, y).SetNeighbors();
                }
            }
        }

        public void CreateGridFromData()
        {
            int size = TerrainData.MapSize;
            float halfWidth = size / 2f;
            float halfHeight = size / 2f;
            Vector3 origin = new Vector3(-halfWidth, -halfHeight);

            grid = new Grid<WorldTile>(size, size, 1, origin, (Grid<WorldTile> g, int x, int y) => new WorldTile(g, x, y));

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var node = grid.GetGridObject(x, y);
                    node.SetNeighbors();
                }
            }
        }

        public void CreateGridFromData(WorldData data)
        {
            int size = data.mapWidth;
            float halfWidth = size / 2f;
            float halfHeight = size / 2f;
            Vector3 origin = new Vector3(-halfWidth, -halfHeight);

            grid = new Grid<WorldTile>(size, size, 1, origin, (Grid<WorldTile> g, int x, int y) => new WorldTile(g, x, y));

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var node = grid.GetGridObject(x, y);
                    node.SetNeighbors();

                    //will also need reference to scriptable objects for abstract values
                }
            }
        }

        public WorldTile GetNode(int x, int y)
        {
            return grid.GetGridObject(x, y);
        }

        public WorldTile GetNode(Vector3 worldPosition)
        {
            return grid.GetGridObject(worldPosition);
        }

        public Vector3 GetPosition(WorldTile node)
        {
            return grid.GetWorldPosition(node.x, node.y);
        }

        public int GetNodeDistance_Path(WorldTile fromNode, WorldTile toNode)
        {
            int x = Mathf.Abs(fromNode.x - toNode.x);
            int y = Mathf.Abs(fromNode.y - toNode.y);
            return x + y;
        }

        public float GetNodeDistance_Straight(WorldTile fromNode, WorldTile toNode)
        {

            return Mathf.Sqrt(Mathf.Pow(fromNode.x - toNode.x, 2) + Mathf.Pow(fromNode.y - toNode.y, 2));
        }

        public List<WorldTile> GetNodesInRange_Circle(WorldTile fromNode, int range)
        {
            var nodes = new List<WorldTile>();

            for (int x = fromNode.x - range; x < fromNode.x + range + 1; x++)
            {
                for (int y = fromNode.y - range; y < fromNode.y + range + 1; y++)
                {
                    if (x < 0 || x > grid.GetWidth() - 1) continue;
                    if (y < 0 || y > grid.GetHeight() - 1) continue;

                    var toNode = grid.GetGridObject(x, y);
                    if (GetNodeDistance_Straight(fromNode, toNode) <= range) nodes.Add(toNode);
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
                    if (x < 0 || x > grid.GetWidth() - 1) continue;
                    if (y < 0 || y > grid.GetHeight() - 1) continue;

                    var toNode = grid.GetGridObject(x, y);
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

        public int GetPathCount(WorldTile startNode, WorldTile endNode, Settlement settlement = null)
        {
            return (FindNodePath(startNode.x, startNode.y, endNode.x, endNode.y, settlement)).Count;
        }

        //Returns a list of nodes that can be travelled to reach a target destination
        public List<WorldTile> FindNodePath(int startX, int startY, int endX, int endY, Settlement settlement = null)
        {
            WorldTile startNode = grid.GetGridObject(startX, startY);
            //Debug.Log("Start: " + startNode.x + "," + startNode.y);
            WorldTile endNode = grid.GetGridObject(endX, endY);
            //Debug.Log("End: " + endNode.x + "," + endNode.y);

            openList = new List<WorldTile> { startNode };
            closedList = new List<WorldTile>();

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    WorldTile pathNode = grid.GetGridObject(x, y);
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

                foreach (WorldTile neighbour in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighbour)) continue;

                    if (!neighbour.isNotWater)// || neighbour.isOccupied)
                    {
                        //Debug.Log("Removing unwalkable/occupied tile " + neighbour.x + "," + neighbour.y);
                        closedList.Add(neighbour);
                        continue;
                    }
                    //Don't allow the passage of one settlement through another's territory
                    if (settlement != null && neighbour.Territory != null && neighbour.Territory != settlement)
                    {
                        closedList.Add(neighbour);
                        continue;
                    }

                    //Adding in movement cost here of the neighbor node to account for areas that are more difficult to move through
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbour);

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
            Debug.Log("Path could not be found");
            return null;
        }

        private int CalculateDistanceCost(WorldTile a, WorldTile b)
        {
            int xDistance = Mathf.Abs(a.x - b.x);
            int yDistance = Mathf.Abs(a.y - b.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
            //return MOVE_STRAIGHT_COST * remaining;
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

        //Return a list of all neighbors, up/down/left/right
        private List<WorldTile> GetNeighbourList(WorldTile currentNode)
        {
            List<WorldTile> neighborList = new List<WorldTile>();

            //Up
            if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x, currentNode.y + 1));
            //Down
            if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x, currentNode.y - 1));
            //Left
            if (currentNode.x - 1 >= 0) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y));
            //Right
            if (currentNode.x + 1 < grid.GetWidth()) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y));

            /*if (allowDiagonals)
            {
                if (currentNode.x - 1 >= 0)
                {
                    //Left Down
                    if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
                    //Left Up
                    if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
                }
                if (currentNode.x + 1 < grid.GetWidth())
                {
                    //Right Down
                    if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
                    //Right Up
                    if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
                }
            }*/

            return neighborList;
        }

        #endregion
    }
}