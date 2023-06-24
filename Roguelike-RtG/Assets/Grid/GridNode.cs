using JS.ECS;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : IHeapItem<GridNode>
{
    public Grid<GridNode> grid {  get; private set; }
    public int x { get; private set; }
    public int y { get; private set; }

    //A* Pathfinding
    public int gCost; //the movement cost to move from the start node to this node, following the existing path
    public int hCost; //the estimated movement cost to move from this node to the end node
    public int fCost  //the current best guess as to the cost of the path
    {
        get
        {
            return gCost + hCost;
        }
    }
   
    private int heapIndex;
    public int HeapIndex
    {
        get => heapIndex;
        set
        {
            heapIndex = value;
        }
    }
    public GridNode cameFromNode;

    public int CompareTo(GridNode other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0) compare = hCost.CompareTo(other.hCost);
        return -compare; //want to return 1 if it's lower, so return negative value
    }

    public bool isWater = false; //set to true when a water block is placed here
    public bool stairsUp = false;
    public bool stairsDown = false;
    public bool isOccupied { get; private set; } //true if there is another creature occupying the node
    public bool isWalkable { get; private set; } //if this node can be traversed at all
    public bool blocksGas { get; private set; }

    public int movementCost { get; private set; } //Increased cost to Move Action for moving into this node, affected by difficult terrain. 
    public int pathfindingCost { get; private set; } //Additional cost to move into this node for pathfinding calculations, to avoid hazards

    public List<GridNode> neighbors_all { get; private set; }
    public List<GridNode> neighbors_adj { get; private set; }
    public List<Entity> Entities { get; private set; }
    public GridNode(Grid<GridNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        isOccupied = false;
        isWalkable = true;
        movementCost = 0;

        neighbors_all = new List<GridNode>();
        neighbors_adj = new List<GridNode>();
        Entities = new List<Entity>();
    }

    public void GetNeighbors()
    {       
        if (y + 1 < grid.Height)
        {
            neighbors_adj.Add(grid.GetGridObject(x, y + 1)); //N
            neighbors_all.Add(grid.GetGridObject(x, y + 1));
        }
        if (y - 1 >= 0)
        {
            neighbors_adj.Add(grid.GetGridObject(x, y - 1)); //S
            neighbors_all.Add(grid.GetGridObject(x, y - 1));
        }
        
        if (x + 1 < grid.Width)
        {
            neighbors_adj.Add(grid.GetGridObject(x + 1, y)); //E
            neighbors_all.Add(grid.GetGridObject(x + 1, y));

            if (y - 1 >= 0) neighbors_all.Add(grid.GetGridObject(x + 1, y - 1)); //SW
            if (y + 1 < grid.Height) neighbors_all.Add(grid.GetGridObject(x + 1, y + 1)); //NW
        }
        if (x - 1 >= 0)
        {
            neighbors_adj.Add(grid.GetGridObject(x - 1, y)); //W
            neighbors_all.Add(grid.GetGridObject(x - 1, y));

            if (y - 1 >= 0) neighbors_all.Add(grid.GetGridObject(x - 1, y - 1)); //SW
            if (y + 1 < grid.Height) neighbors_all.Add(grid.GetGridObject(x - 1, y + 1)); //NW
        }
    }

    public void SetOccupied(bool isOccupied)
    {
        this.isOccupied = isOccupied;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetMoveCost(int cost)
    {
        movementCost = Mathf.Clamp(cost, 0, int.MaxValue);
        grid.TriggerGridObjectChanged(x, y);
    }
}
