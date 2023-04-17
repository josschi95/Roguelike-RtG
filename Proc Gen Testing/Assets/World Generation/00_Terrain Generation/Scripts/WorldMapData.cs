using System.Collections.Generic;
using UnityEngine;
using JS.WorldGeneration;

[CreateAssetMenu(menuName = "Scriptable Objects/World Map Data")]
public class WorldMapData : ScriptableObject
{
    [SerializeField] private TerrainData terrainData;

    private Grid<TerrainNode> grid;


    public int Height => grid.GetHeight();
    public int Width => grid.GetWidth();

    public void CreateGrid(int width, int height)
    {
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        Vector3 origin = new Vector3(-halfWidth, -halfHeight);

        grid = new Grid<TerrainNode>(width, height, 1, origin, (Grid<TerrainNode> g, int x, int y) => new TerrainNode(g, x, y));

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
        int size = terrainData.mapSize;
        float halfWidth = size / 2f;
        float halfHeight = size / 2f;
        Vector3 origin = new Vector3(-halfWidth, -halfHeight);

        grid = new Grid<TerrainNode>(size, size, 1, origin, (Grid<TerrainNode> g, int x, int y) => new TerrainNode(g, x, y));

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                var node = grid.GetGridObject(x, y);
                node.SetNeighbors();
            }
        }
    }

    public void CreateGridFromData(MapData data)
    {
        int size = data.mapSize;
        float halfWidth = size / 2f;
        float halfHeight = size / 2f;
        Vector3 origin = new Vector3(-halfWidth, -halfHeight);

        grid = new Grid<TerrainNode>(size, size, 1, origin, (Grid<TerrainNode> g, int x, int y) => new TerrainNode(g, x, y));

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

    public TerrainNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public TerrainNode GetNode(Vector3 worldPosition)
    {
        return grid.GetGridObject(worldPosition);
    }

    public Vector3 GetPosition(TerrainNode node)
    {
        return grid.GetWorldPosition(node.x, node.y);
    }

    public int GetNodeVerticalDistance(TerrainNode fromNode, TerrainNode toNode)
    {
        return Mathf.Abs(fromNode.y - toNode.y);
    }

    public int GetNodeHorizontalDistance(TerrainNode fromNode, TerrainNode toNode)
    {
        return Mathf.Abs(fromNode.x - toNode.x);
    }

    public int GetNodeDistance_Path(TerrainNode fromNode, TerrainNode toNode)
    {
        int x = Mathf.Abs(fromNode.x - toNode.x);
        int y = Mathf.Abs(fromNode.y - toNode.y);
        return x + y;
    }

    public float GetNodeDistance_Straight(TerrainNode fromNode, TerrainNode toNode)
    {

        return Mathf.Sqrt(Mathf.Pow(fromNode.x - toNode.x, 2) + Mathf.Pow(fromNode.y - toNode.y, 2));
    }

    public List<TerrainNode> GetNodesInRange_Circle(TerrainNode fromNode, int range)
    {
        var nodes = new List<TerrainNode>();

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

    public List<TerrainNode> GetNodesInRange_Diamond(TerrainNode fromNode, int range)
    {
        var nodes = new List<TerrainNode>();

        for (int x = fromNode.x - range; x < fromNode.x + range + 1; x++)
        {
            for (int y = fromNode.y - range; y < fromNode.y + range + 1; y++)
            {
                if (x < 0 || x > grid.GetWidth() - 1) continue;
                if (y < 0 || y > grid.GetHeight() - 1) continue;

                var node = grid.GetGridObject(x, y);
                if (GetNodeDistance_Path(fromNode, node) <= range) nodes.Add(node);
            }
        }
        return nodes;
    }

    public List<TerrainNode> GetNodesInRange_Square(TerrainNode fromNode, int range)
    {
        var nodes = new List<TerrainNode>();

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
    private List<TerrainNode> openList; //nodes to search
    private List<TerrainNode> closedList; //already searched
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    
    public int GetPathCount(TerrainNode startNode, TerrainNode endNode, Settlement settlement = null)
    {
        return (FindNodePath(startNode.x, startNode.y, endNode.x, endNode.y, settlement)).Count;
    }

    //Returns a list of nodes that can be travelled to reach a target destination
    public List<TerrainNode> FindNodePath(int startX, int startY, int endX, int endY, Settlement settlement = null)
    {
        TerrainNode startNode = grid.GetGridObject(startX, startY);
        //Debug.Log("Start: " + startNode.x + "," + startNode.y);
        TerrainNode endNode = grid.GetGridObject(endX, endY);
        //Debug.Log("End: " + endNode.x + "," + endNode.y);

        openList = new List<TerrainNode> { startNode };
        closedList = new List<TerrainNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TerrainNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            TerrainNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                //Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (TerrainNode neighbour in GetNeighbourList(currentNode))
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
                    neighbour.cameFromNode = currentNode;
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

    private int CalculateDistanceCost(TerrainNode a, TerrainNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        //return MOVE_STRAIGHT_COST * remaining;
    }

    private TerrainNode GetLowestFCostNode(List<TerrainNode> pathNodeList)
    {
        TerrainNode lowestFCostNode = pathNodeList[0];

        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = pathNodeList[i];
        }

        return lowestFCostNode;
    }

    private List<TerrainNode> CalculatePath(TerrainNode endNode)
    {
        List<TerrainNode> path = new List<TerrainNode>();
        path.Add(endNode);
        TerrainNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            //Start at the end and work backwards
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    //Return a list of all neighbors, up/down/left/right
    private List<TerrainNode> GetNeighbourList(TerrainNode currentNode)
    {
        List<TerrainNode> neighborList = new List<TerrainNode>();

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
