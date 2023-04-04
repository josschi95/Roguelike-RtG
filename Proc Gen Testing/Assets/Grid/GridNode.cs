using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    private Grid<GridNode> grid;
    public int x { get; private set; }
    public int y { get; private set; }

    public int gCost; //the movement cost to move from the start node to this node, following the existing path
    public int hCost; //the estimated movement cost to move from this node to the end node
    public int fCost; //the current best guess as to the cost of the path
    public GridNode cameFromNode;

    public bool isOccupied { get; private set; } //true if there is another creature occupying the node
    public bool isWalkable { get; private set; } //if this node can be traversed at all
    public int movementPenalty { get; private set; } //additional cost to move into this tile

    public GridNode[] neighbors { get; private set; }

    public GridNode(Grid<GridNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        isOccupied = false;
        isWalkable = true;
        movementPenalty = 0;
    }

    public void GetNeighbors()
    {
        var neighbors = new List<GridNode>();

        if (x != 0) neighbors.Add(grid.GetGridObject(x - 1, y));
        if (x != grid.GetWidth() - 1) neighbors.Add(grid.GetGridObject(x + 1, y));
        if (y != 0) neighbors.Add(grid.GetGridObject(x, y - 1));
        if (y != grid.GetHeight() - 1) neighbors.Add(grid.GetGridObject(x, y + 1));
        this.neighbors = new GridNode[neighbors.Count];

        for (int i = 0; i < this.neighbors.Length; i++)
        {
            this.neighbors[i] = neighbors[i];
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
