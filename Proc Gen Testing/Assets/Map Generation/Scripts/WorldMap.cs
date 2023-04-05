using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    public static WorldMap instance { get; private set; }
    private Grid<TerrainNode> grid;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void CreateGrid(int width, int height)
    {
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        Vector3 origin = new Vector3(transform.position.x - halfWidth, transform.position.y - halfHeight);

        grid = new Grid<TerrainNode>(width, height, 1, origin, (Grid<TerrainNode> g, int x, int y) => new TerrainNode(g, x, y));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid.GetGridObject(x, y).SetNeighbors();
            }
        }
    }

    public void SetGridValues()
    {

    }

    public void CreateGridFromData(MapData data)
    {
        int size = data.mapSize;
        float halfWidth = size / 2f;
        float halfHeight = size / 2f;
        Vector3 origin = new Vector3(transform.position.x - halfWidth, transform.position.y - halfHeight);

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

    public int GetNodeDistance(TerrainNode fromNode, TerrainNode toNode)
    {
        int x = Mathf.Abs(fromNode.x - toNode.x);
        int y = Mathf.Abs(fromNode.y - toNode.y);
        return x + y;
    }

    public List<TerrainNode> GetNodesInRange(TerrainNode fromNode, int range)
    {
        var nodes = new List<TerrainNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0;  y < grid.GetHeight();  y++)
            {
                var node = grid.GetGridObject(x, y);
                if (GetNodeDistance(fromNode, node) <= range) nodes.Add(node);
            }
        }

        return nodes;
    }
}
