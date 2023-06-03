using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private List<GridNode> openList; //nodes to search
    private List<GridNode> closedList; //already searched

    //[SerializeField] private bool allowDiagonals = false;

    private void Awake()
    {
        if (instance !=  null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    //This is new, nodes will now hold reference to their grids
    public List<Vector2Int> FindVectorPath(GridNode start, GridNode end, bool ignoreEndNode = false)
    {
        List<GridNode> path = FindNodePath(start, end, ignoreEndNode);
        if (path == null) return null;

        List<Vector2Int> vectorPath = new List<Vector2Int>();
        foreach (GridNode node in path)
        {
            vectorPath.Add(new Vector2Int(node.x, node.y));
        }
        return vectorPath;
    }

    public List<GridNode> FindNodePath(GridNode start, GridNode end, bool ignoreEndNode = false)
    {
        var grid = start.grid;
        if (grid != end.grid)
        {

            return null;
        }
        openList = new List<GridNode> { start };
        closedList = new List<GridNode>();

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                GridNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        start.gCost = 0;
        start.hCost = CalculateDistanceCost(start, end);
        start.CalculateFCost();

        while (openList.Count > 0)
        {
            GridNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == end)
            {
                //Reached final node
                return CalculatePath(end);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //foreach (GridNode neighbour in GetNeighbourList(currentNode))
            foreach (GridNode neighbour in currentNode.neighbors_all)
            {
                if (closedList.Contains(neighbour)) continue;

                //if the neighbor is the endNode and choosing to ignore whether it is walkable, add it to the closed list
                if (neighbour == end && ignoreEndNode)
                {
                    //Do nothing here, bypass the next if statement
                    //Debug.Log("Ignoring End Node");
                }
                else if (!neighbour.isWalkable || neighbour.isOccupied)
                {
                    //Debug.Log("Removing unwalkable/occupied tile " + neighbour.x + "," + neighbour.y);
                    closedList.Add(neighbour);
                    continue;
                }

                //Adding in movement cost here of the neighbor node to account for areas that are more difficult to move through
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbour) + neighbour.movementPenalty;

                if (tentativeGCost < neighbour.gCost)
                {
                    //If it's lower than the cost previously stored on the neightbor, update it
                    neighbour.cameFromNode = currentNode;
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = CalculateDistanceCost(neighbour, end);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour)) openList.Add(neighbour);
                }
            }
        }

        //Out of nodes on the openList
        Debug.Log("Path could not be found");
        return null;
    }

    //Return a list of nodes which can be reached given the available number of moves
    public List<Vector3> FindReachableNodes(GridNode startNode, int moves)
    {
        List<GridNode> nodes = new List<GridNode>();
        var grid = startNode.grid;

        //Get all nodes in the grid
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                GridNode pathNode = grid.GetGridObject(x, y);
                //The node can be walked through
                //Will likely have to change this in the future to allow allies to pass through each other's spaces
                //Which means returning a list of pathnodes instead, and then finding which ones are occupied,
                //and checking if the character there is an ally
                if (pathNode.isWalkable && !pathNode.isOccupied)
                {
                    if (Vector2.Distance(new Vector2(pathNode.x, pathNode.y), new Vector2(startNode.x, startNode.y)) <= moves)
                    {
                        //Note that this doesn't eliminate diagonals
                        nodes.Add(pathNode);
                        //Debug.Log(pathNode.x + "," + pathNode.y);
                    }
                }
            }
        }

        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            var temp = FindNodePath(startNode, nodes[i]);

            //There is no available path to that node
            if (temp == null) nodes.RemoveAt(i);
            //The path requires more moves than are available
            else if (CalculatePathMoveCost(temp) > moves) nodes.RemoveAt(i);
        }

        List<Vector3> vectorPath = new List<Vector3>();
        foreach (GridNode node in nodes)
        {
            vectorPath.Add(new Vector3(node.x, node.y));
        }
        return vectorPath;
    }

    //Return a list of nodes which can be targeted given the range, used for attacks
    public List<GridNode> FindTargetableNodes(GridNode startNode, int range)
    {
        List<GridNode> nodes = new List<GridNode>();
        var grid = startNode.grid;

        //Get all nodes in the grid
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                GridNode pathNode = grid.GetGridObject(x, y);
                //Add all walkable nodes which can be accessed via a straight line path
                if (pathNode.isWalkable && Vector2.Distance(new Vector2(pathNode.x, pathNode.y), new Vector2(startNode.x, startNode.y)) <= range)
                {
                    //Note that this doesn't eliminate diagonals
                    nodes.Add(pathNode);
                    //Debug.Log(pathNode.x + "," + pathNode.y);
                }
            }
        }

        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            var temp = FindNodePath(startNode, nodes[i]);

            //There is no available path to that node
            if (temp == null) nodes.RemoveAt(i);
            //The path requires more moves than are available
            else if (CalculatePathMoveCost(temp) > range) nodes.RemoveAt(i);
        }

        return nodes;
    }

    //Return a list of all neighbors, up/down/left/right
    private List<GridNode> GetNeighbourList(GridNode currentNode)
    {
        return currentNode.neighbors_all;
        //if (allowDiagonals) return currentNode.neighbors_all;
        //return currentNode.neighbors_adj;
    }

    private List<GridNode> CalculatePath(GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        path.Add(endNode);
        GridNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            //Start at the end and work backwards
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculatePathMoveCost(List<GridNode> nodes)
    {
        int cost = 0;
        //Skip the first node in the list, this is where the character is
        for (int i = 1; i < nodes.Count; i++)
        {
            cost += nodes[i].movementPenalty + 1;
        }
        return cost;
    }

    private int CalculateDistanceCost(GridNode a, GridNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        //if (allowDiagonals) return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        //return MOVE_STRAIGHT_COST * remaining;
    }

    private GridNode GetLowestFCostNode(List<GridNode> pathNodeList)
    {
        GridNode lowestFCostNode = pathNodeList[0];

        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = pathNodeList[i];
        }

        return lowestFCostNode;
    }
}