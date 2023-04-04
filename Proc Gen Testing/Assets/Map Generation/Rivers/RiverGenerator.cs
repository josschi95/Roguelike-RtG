using System.Collections.Generic;
using UnityEngine;

//https://www.gamedeveloper.com/programming/procedurally-generating-wrapping-world-maps-in-unity-c-part-3

public class RiverGenerator : MonoBehaviour
{
    private WorldMap worldMap;

    [SerializeField] private float MinRiverHeight = 0.6f;

    [SerializeField] private int MaxRiverAttempts = 1000;

    [SerializeField] private int MinRiverTurns = 10;

    [SerializeField] private int MinRiverLength = 15;

    [SerializeField] private int MaxRiverIntersections = 2;

    [SerializeField] private TerrainType riverTerrain;

    private List<River> rivers;
    private List<RiverGroup> riverGroups;

    public List<River> GenerateRivers(int mapSize, List<MountainRange> mountains, int count)
    {
        //Debug.Log("Digging Rivers. " + Time.realtimeSinceStartup);

        worldMap = WorldMap.instance;

        int attempts = 0;
        rivers = new List<River>();
        riverGroups = new List<RiverGroup>();

        while (rivers.Count < count && attempts < MaxRiverAttempts)
        {
            attempts++;

            //Grab a random node from the foot of a random mountain
            //If attempts are at less than half of what is allowed, try to find an unoccupied mountain
            var mountain = FindRiverSource(mountains, attempts < MaxRiverAttempts / 2);
            TerrainNode node;
            if (mountain != null) node = mountain.Nodes[Random.Range(0, mountain.Nodes.Count)];
            else node = worldMap.GetNode(Random.Range(0, mapSize - 1), Random.Range(0, mapSize - 1));

            if (node.altitude < MinRiverHeight) continue;
            //Maybe add an extra property for if Land/Water

            //Can start a river here
            River river = new River();

            //Find river initial direction
            river.CurrentDirection = FindLowestNeighborDirection(node);

            //Recursively find a path to water
            FindPathToWater(node, river.CurrentDirection, river);

            //Ensure the generated river meets all requirements
            if (river.TurnCount < MinRiverTurns)
            {
                //Debug.Log("Not Enough Turns");
                continue;
            }
            else if (river.Nodes.Count < MinRiverLength)
            {
                //Debug.Log("Not Enough Nodes");
                continue;
            }
            else if (river.Intersections > MaxRiverIntersections)
            {
                //Debug.Log("Too many intersections");
                continue;
            }

            river.Register(rivers.Count);
            rivers.Add(river);
        }

        BuildRiverGroups(mapSize);
        DigRiverGroups();
        DigRemainingRivers();

        AdjustRiverNodes(mapSize, mapSize);

        return rivers;
    }

    private MountainRange FindRiverSource(List<MountainRange> mountains, bool riverLessMountain)
    {
        if (mountains.Count == 0) return null;
        if (!riverLessMountain) return mountains[Random.Range(0, mountains.Count)];

        var shuffledList = new List<MountainRange>(mountains);
        for (int i = 0; i < shuffledList.Count; i++)
        {
            var temp = shuffledList[i];
            int randomIndex = Random.Range(i, shuffledList.Count);
            shuffledList[i] = shuffledList[randomIndex];
            shuffledList[randomIndex] = temp;
        }

        for (int i = 0; i < shuffledList.Count; i++)
        {
            if (shuffledList[i].Rivers.Count == 0) return shuffledList[i];
        }

        return mountains[Random.Range(0, mountains.Count)];
    }

    private TerrainNode FindFootOfMountain(MountainRange mountain)
    {
        for (int i = 0; i < mountain.Nodes.Count; i++)
        {
            for (int n = 0; n < mountain.Nodes[i].neighbors.Length; n++)
            {
                //The neighbor is part of the mountain
                if (mountain.Nodes.Contains(mountain.Nodes[i].neighbors[n])) continue;
                //The neighbor is not part of the mountain but is adjacen to it
                return mountain.Nodes[i];//.neighbors[n];
            }
        }
        return mountain.Nodes[0];
    }

    private TerrainNode FindLowestNeighborNode(TerrainNode node)
    {
        var lowestNeighbor = node.neighbors[0];
        for (int i = 0; i < node.neighbors.Length; i++)
        {
            if (node.neighbors[i].altitude < lowestNeighbor.altitude)
            {
                lowestNeighbor = node.neighbors[i];
            }
        }
        return lowestNeighbor;
    }

    private Direction FindLowestNeighborDirection(TerrainNode startNode)
    {
        var lowestNeighbor = FindLowestNeighborNode(startNode);
        if (lowestNeighbor.y > startNode.y) return Direction.North;
        else if (lowestNeighbor.y < startNode.y) return Direction.South;
        else if (lowestNeighbor.x > startNode.x) return Direction.East;
        else if (lowestNeighbor.x < startNode.x) return Direction.West;
        return Direction.South;
    }

    private TerrainNode GetNeighbor(TerrainNode startNode, Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return worldMap.GetNode(startNode.x, startNode.y + 1);
            case Direction.South:
                return worldMap.GetNode(startNode.x, startNode.y - 1);
            case Direction.East:
                return worldMap.GetNode(startNode.x + 1, startNode.y);
            case Direction.West:
                return worldMap.GetNode(startNode.x - 1, startNode.y);
        }
        return null;
    }

    private void FindPathToWater(TerrainNode node, Direction direction, River river)
    {
        //may need to do some additional checks...
        if (node.rivers.Contains(river))
        {
            Debug.Log("Node already contains river");
            return;
        }
        else if (river.Nodes.Contains(node)) //this caused a stack overflow x2!
        {
            Debug.LogWarning("River already contains unregistered node");
            return;
        }

        if (node.rivers.Count > 0) river.Intersections++;

        river.AddNode(node);

        // get neighbors
        TerrainNode north = GetNeighbor(node, Direction.North);
        TerrainNode south = GetNeighbor(node, Direction.South);
        TerrainNode east = GetNeighbor(node, Direction.East);
        TerrainNode west = GetNeighbor(node, Direction.West);

        float northValue = GetNodeValue(north, river);
        float southValue = GetNodeValue(south, river);
        float eastValue = GetNodeValue(east, river);
        float westValue = GetNodeValue(west, river);

        // override flow direction if a tile is significantly lower
        switch (direction)
        {
            case Direction.North:
                if (Mathf.Abs(northValue - southValue) < 0.1f)
                    southValue = int.MaxValue;
                break;
            case Direction.South:
                if (Mathf.Abs(northValue - southValue) < 0.1f)
                    northValue = int.MaxValue;
                break;
            case Direction.East:
                if (Mathf.Abs(eastValue - westValue) < 0.1f)
                    westValue = int.MaxValue;
                break;
            case Direction.West:
                if (Mathf.Abs(eastValue - westValue) < 0.1f)
                    eastValue = int.MaxValue;
                break;
        }

        float min = Mathf.Min(Mathf.Min(Mathf.Min(northValue, southValue), eastValue), westValue);
        if (min == int.MaxValue) return; //exit if no minimum is found

        if (min == northValue)
        {
            if (north.isLand)
            {
                if (river.CurrentDirection != Direction.North)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.North;
                }
                FindPathToWater(north, direction, river);
            }
        }
        else if (min == southValue)
        {
            if (south.isLand)
            {
                if (river.CurrentDirection != Direction.South)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.South;
                }
                FindPathToWater(south, direction, river);
            }
        }
        else if (min == eastValue)
        {
            if (east.isLand)
            {
                if (river.CurrentDirection != Direction.East)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.East;
                }
                FindPathToWater(east, direction, river);
            }
        }
        else if (min == westValue)
        {
            if (west.isLand)
            {
                if (river.CurrentDirection != Direction.West)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.West;
                }
                FindPathToWater(west, direction, river);
            }
        }
    }

    private float GetNodeValue(TerrainNode node, River river)
    {
        if (node == null) return int.MaxValue;
        float value = int.MaxValue;

        // query height values of neighbor
        if (node.GetNeighborRiverCount(river) < 2 && !river.Nodes.Contains(node)) value = node.altitude;

        // if neighbor is existing river that is not this one, flow into it
        if (node.rivers.Count == 0 && !node.isLand) value = 0;

        return value;
    }

    private void BuildRiverGroups(int mapSize)
    {
        //loop each tile, checking if it belongs to multiple rivers
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                TerrainNode node = worldMap.GetNode(x, y);

                //node only has one or no river(s) running through it
                if (node.rivers.Count <= 1) continue;
                // multiple rivers == intersection

                RiverGroup group = FindExistingRiverGroup(node);

                // existing group found -- add to it
                if (group != null)
                {
                    for (int n = 0; n < node.rivers.Count; n++)
                    {
                        if (!group.Rivers.Contains(node.rivers[n]))
                            group.Rivers.Add(node.rivers[n]);
                    }
                }
                else   //No existing group found - create a new one
                {
                    //Debug.Log("New River Group Found at " + node.x + "," + node.y);

                    group = new RiverGroup();
                    for (int n = 0; n < node.rivers.Count; n++)
                    {
                        group.Rivers.Add(node.rivers[n]);
                    }
                    riverGroups.Add(group);
                }
            }
        }
    }

    private RiverGroup FindExistingRiverGroup(TerrainNode node)
    {
        RiverGroup group = null;
        for (int n = 0; n < node.rivers.Count; n++)
        {
            River tileriver = node.rivers[n];
            for (int i = 0; i < riverGroups.Count; i++)
            {
                for (int j = 0; j < riverGroups[i].Rivers.Count; j++)
                {
                    River river = riverGroups[i].Rivers[j];
                    if (river.ID == tileriver.ID)
                    {
                        group = riverGroups[i];
                    }
                    if (group != null) break;
                }
                if (group != null) break;
            }
            if (group != null) break;
        }
        return group;
    }

    private void DigRiverGroups()
    {
        for (int i = 0; i < riverGroups.Count; i++)
        {
            RiverGroup group = riverGroups[i];
            River longest = FindLongestRiverInGroup(group);

            //Dig out longest path first
            DigRiver(longest);

            for (int r = 0; r < group.Rivers.Count; r++)
            {
                if (group.Rivers[r] != longest)
                {
                    DigRiver(group.Rivers[r]);
                }
            }
        }
    }

    private void DigRemainingRivers()
    {
        for (int i = 0; i < rivers.Count; i++)
        {
            if (rivers[i].hasBeenDug) continue;
            DigRiver(rivers[i]);
        }
    }

    private River FindLongestRiverInGroup(RiverGroup group)
    {
        River longest = group.Rivers[0];
        for (int i = 0; i < group.Rivers.Count; i++)
        {
            if (group.Rivers[i].Nodes.Count > longest.Nodes.Count)
            {
                longest = group.Rivers[i];
            }
        }
        return longest;
    }

    private void DigRiver(River river)
    {
        int counter = 0;

        // How wide are we digging this river?
        int size = Random.Range(1, 5);
        river.Length = river.Nodes.Count;

        // randomize size change
        int two = river.Length / 2;
        int three = two / 2;
        int four = three / 2;
        int five = four / 2;

        int twomin = two / 3;
        int threemin = three / 3;
        int fourmin = four / 3;
        int fivemin = five / 3;

        // randomize lenght of each size
        int count1 = Random.Range(fivemin, five);
        if (size < 4)
        {
            count1 = 0;
        }
        int count2 = count1 + Random.Range(fourmin, four);
        if (size < 3)
        {
            count2 = 0;
            count1 = 0;
        }
        int count3 = count2 + Random.Range(threemin, three);
        if (size < 2)
        {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        int count4 = count3 + Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length)
        {
            int extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                else if (count3 > 0) { count3--; count4--; extra--; }
                else if (count4 > 0) { count4--; extra--; }
            }
        }

        // Dig it out
        for (int i = river.Nodes.Count - 1; i >= 0; i--)
        {
            TerrainNode node = river.Nodes[i];

            if (counter < count1) node.DigRiver(river, 4);
            else if (counter < count2) node.DigRiver(river, 3); 
            else if (counter < count3) node.DigRiver(river, 2);
            else if (counter < count4) node.DigRiver(river, 1);
            else node.DigRiver(river, 0);
            counter++;
        }
        river.hasBeenDug = true;
    }

    private void AdjustRiverNodes(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var node = worldMap.GetNode(x, y);

                if (node.rivers.Count <= 0) continue;

                node.SetHeightValues(node.altitude, riverTerrain);
            }
        }
    }
}