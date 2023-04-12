using System.Collections.Generic;
using UnityEngine;
using JS.WorldGeneration;

[CreateAssetMenu(menuName = "Scriptable Objects/World Map Data")]
public class WorldMapData : ScriptableObject
{
    [SerializeField] private TerrainData terrainData;

    private Grid<TerrainNode> grid;

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
}
