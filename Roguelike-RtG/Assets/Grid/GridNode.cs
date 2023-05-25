using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public Grid<GridNode> grid {  get; private set; }
    public int x { get; private set; }
    public int y { get; private set; }

    public int gCost; //the movement cost to move from the start node to this node, following the existing path
    public int hCost; //the estimated movement cost to move from this node to the end node
    public int fCost; //the current best guess as to the cost of the path
    public GridNode cameFromNode;

    public bool isOccupied { get; private set; } //true if there is another creature occupying the node
    public bool isWalkable { get; private set; } //if this node can be traversed at all
    public bool blocksGas { get; private set; }
    public int movementPenalty { get; private set; } //additional cost to move into this tile

    public List<GridNode> neighbors_all { get; private set; }
    public List<GridNode> neighbors_adj { get; private set; }

    public GridNode(Grid<GridNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        isOccupied = false;
        isWalkable = true;
        movementPenalty = 0;

        neighbors_all = new List<GridNode>();
        neighbors_adj = new List<GridNode>();
    }

    public void GetNeighbors()
    {       
        if (y + 1 < grid.GetHeight())
        {
            neighbors_adj.Add(grid.GetGridObject(x, y + 1)); //N
            neighbors_all.Add(grid.GetGridObject(x, y + 1));
        }
        if (y - 1 >= 0)
        {
            neighbors_adj.Add(grid.GetGridObject(x, y - 1)); //S
            neighbors_all.Add(grid.GetGridObject(x, y - 1));
        }
        
        if (x + 1 < grid.GetWidth())
        {
            neighbors_adj.Add(grid.GetGridObject(x + 1, y)); //E
            neighbors_all.Add(grid.GetGridObject(x + 1, y));

            if (y - 1 >= 0) neighbors_all.Add(grid.GetGridObject(x + 1, y - 1)); //SW
            if (y + 1 < grid.GetHeight()) neighbors_all.Add(grid.GetGridObject(x + 1, y + 1)); //NW
        }
        if (x - 1 >= 0)
        {
            neighbors_adj.Add(grid.GetGridObject(x - 1, y)); //W
            neighbors_all.Add(grid.GetGridObject(x - 1, y));

            if (y - 1 >= 0) neighbors_all.Add(grid.GetGridObject(x - 1, y - 1)); //SW
            if (y + 1 < grid.GetHeight()) neighbors_all.Add(grid.GetGridObject(x - 1, y + 1)); //NW
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
        movementPenalty = Mathf.Clamp(cost, 0, int.MaxValue);
        grid.TriggerGridObjectChanged(x, y);
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}
