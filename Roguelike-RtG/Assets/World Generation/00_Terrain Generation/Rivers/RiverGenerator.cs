using System;
using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldMap.Generation
{
    public class RiverGenerator : MonoBehaviour
    {
        [SerializeField] private WorldData worldMap;
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private TerrainData terrainData;
        //private WorldMap worldMap;

        [SerializeField] private float MinRiverHeight = 0.6f;

        [SerializeField] private int MaxRiverAttempts = 1000;

        [SerializeField] private int MinRiverTurns = 10;

        [SerializeField] private int MinRiverLength = 15;

        [SerializeField] private int MaxRiverIntersections = 2;

        [SerializeField] private Biome riverBiome;

        private List<River> rivers;
        private List<RiverGroup> riverGroups;

        public List<River> GenerateRivers(int mapSize, int count)
        {
            int attempts = 0;
            rivers = new List<River>();

            while (rivers.Count < count && attempts < MaxRiverAttempts)
            {
                attempts++;
                bool findEmpty = attempts < MaxRiverAttempts / 2;
                if (TryGenerateRiver(findEmpty, out River river, out List<WorldTile> tiles))
                {
                    FinalizeRiver(river, tiles);
                }
            }

            BuildRiverGroups(mapSize);
            DigRiverGroups();
            DigRemainingRivers();

            return rivers;
        }

        private bool TryGenerateRiver(bool findEmptyMountain, out River river, out List<WorldTile> tiles)
        {
            river = new River();
            tiles = new List<WorldTile>();
            var mountain = FindRiverSource(findEmptyMountain);
            WorldTile node;

            if (mountain != null) node = mountain.Nodes[worldGenerator.rng.Next(0, mountain.Nodes.Count)];
            else node = worldMap.GetNode(worldGenerator.rng.Next(0, worldMap.Width - 1), worldGenerator.rng.Next(0, worldMap.Height - 1));

            if (terrainData.HeightMap[node.x, node.y] < MinRiverHeight) return false;

            //Find river initial direction
            river.CurrentDirection = node.NeighborDirection_Adjacent(FindLowestNeighborNode(node));

            //Recursively find a path to water
            FindPathToWater(node, river.CurrentDirection, river, tiles);

            //Ensure the generated river meets all requirements
            if (river.TurnCount < MinRiverTurns) return false;
            else if (tiles.Count < MinRiverLength) return false;
            else if (river.Intersections > MaxRiverIntersections) return false;

            return true;
        }

        private MountainRange FindRiverSource(bool riverLessMountain)
        {
            var mountains = worldMap.TerrainData.Mountains;
            if (mountains.Length == 0) return null;
            if (!riverLessMountain) return worldMap.TerrainData.Mountains[worldGenerator.rng.Next(0, mountains.Length)];

            var shuffledList = new List<MountainRange>(mountains);
            for (int i = 0; i < shuffledList.Count; i++)
            {
                var temp = shuffledList[i];
                int randomIndex = worldGenerator.rng.Next(i, shuffledList.Count);
                shuffledList[i] = shuffledList[randomIndex];
                shuffledList[randomIndex] = temp;
            }

            for (int i = 0; i < shuffledList.Count; i++)
            {
                if (shuffledList[i].Rivers.Count == 0) return shuffledList[i];
            }

            return mountains[worldGenerator.rng.Next(0, mountains.Length)];
        }

        private WorldTile FindLowestNeighborNode(WorldTile node)
        {
            var lowestNeighbor = node.neighbors_adj[0];
            for (int i = 0; i < node.neighbors_adj.Count; i++)
            {
                if (terrainData.HeightMap[node.neighbors_adj[i].x, node.neighbors_adj[i].y] < terrainData.HeightMap[lowestNeighbor.x, lowestNeighbor.y])
                {
                    lowestNeighbor = node.neighbors_adj[i];
                }
            }
            return lowestNeighbor;
        }

        private void FindPathToWater(WorldTile node, Direction direction, River river, List<WorldTile> tiles)
        {
            //may need to do some additional checks...
            if (node.rivers.Contains(river))
            {
                Debug.Log("Node already contains river");
                return;
            }
            else if (tiles.Contains(node)) //this caused a stack overflow x2!
            {
                Debug.LogWarning("River already contains unregistered node");
                return;
            }

            if (node.rivers.Count > 0) river.Intersections++;

            tiles.Add(node);

            if (node.x == 0 || node.y == 0 || node.x == worldMap.Width - 1 || node.y == worldMap.Height - 1) return;

            // get neighbors
            WorldTile north = worldMap.GetNode(node.x, node.y + 1);
            WorldTile south = worldMap.GetNode(node.x, node.y - 1);
            WorldTile east = worldMap.GetNode(node.x + 1, node.y);
            WorldTile west = worldMap.GetNode(node.x - 1, node.y);

            float northValue = GetNodeValue(north, river, tiles);
            float southValue = GetNodeValue(south, river, tiles);
            float eastValue = GetNodeValue(east, river, tiles);
            float westValue = GetNodeValue(west, river, tiles);

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
                if (north.IsLand)
                {
                    if (river.CurrentDirection != Direction.North)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.North;
                    }
                    FindPathToWater(north, direction, river, tiles);
                }
            }
            else if (min == southValue)
            {
                if (south.IsLand)
                {
                    if (river.CurrentDirection != Direction.South)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.South;
                    }
                    FindPathToWater(south, direction, river, tiles);
                }
            }
            else if (min == eastValue)
            {
                if (east.IsLand)
                {
                    if (river.CurrentDirection != Direction.East)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.East;
                    }
                    FindPathToWater(east, direction, river, tiles);
                }
            }
            else if (min == westValue)
            {
                if (west.IsLand)
                {
                    if (river.CurrentDirection != Direction.West)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.West;
                    }
                    FindPathToWater(west, direction, river, tiles);
                }
            }
        }

        private float GetNodeValue(WorldTile node, River river, List<WorldTile> tiles)
        {
            if (node == null) return int.MaxValue;
            float value = int.MaxValue;

            // query height values of neighbor
            if (GetNeighborRiverCount(node, river) < 2 && !tiles.Contains(node))
            {
                value = terrainData.HeightMap[node.x, node.y];
            }

            // if neighbor is existing river that is not this one, flow into it
            if (node.rivers.Count == 0 && !node.IsLand) value = 0;

            return value;
        }

        private int GetNeighborRiverCount(WorldTile node, River river)
        {
            int count = 0;

            for (int i = 0; i < node.neighbors_adj.Count; i++)
            {
                if (node.neighbors_adj[i].rivers.Count > 0 && node.neighbors_adj[i].rivers.Contains(river)) count++;
            }

            return count;
        }

        #region - River Groups -
        private void BuildRiverGroups(int mapSize)
        {
            riverGroups = new List<RiverGroup>();

            //loop each tile, checking if it belongs to multiple rivers
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);

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

        private RiverGroup FindExistingRiverGroup(WorldTile node)
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
        #endregion

        #region - Finalize River -
        private void FinalizeRiver(River river, List<WorldTile> tiles)
        {
            river.ID = rivers.Count;
            river.Coordinates = new GridCoordinates[tiles.Count];
            river.Flow = new Compass[tiles.Count];
            for (int i = 0; i < tiles.Count; i++)
            {
                river.Coordinates[i] = new GridCoordinates(tiles[i].x, tiles[i].y);
                tiles[i].SetRiverPath(river);

                if (i == 0) //Direction is only dependent on following node
                {
                    river.Flow[i] = ConvertDirection(tiles[i].NeighborDirection_Adjacent(tiles[i + 1]));
                }
                else if (i == tiles.Count - 1) //Direction is only dependent on previous node
                {
                    river.Flow[i] = ConvertDirection(tiles[i].NeighborDirection_Adjacent(tiles[i - 1]));
                }
                else
                {
                    var from = tiles[i].NeighborDirection_Adjacent(tiles[i - 1]);
                    var to = tiles[i].NeighborDirection_Adjacent(tiles[i + 1]);
                    river.Flow[i] = CombineDirections(from, to);
                }
            }
            rivers.Add(river);
        }

        private Compass ConvertDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return Compass.North;
                case Direction.South: return Compass.South;
                case Direction.East: return Compass.East;
                case Direction.West: return Compass.West;
                default: throw new Exception("Direction outside parameters");
            }
        }

        private Compass CombineDirections(Direction fromDirection, Direction toDirection)
        {
            switch (fromDirection)
            {
                case Direction.North:
                    if (toDirection == Direction.East) return Compass.NorthEast;
                    else if (toDirection == Direction.West) return Compass.NorthWest;
                    else return Compass.North;
                case Direction.South:
                    if (toDirection == Direction.East) return Compass.SouthEast;
                    else if (toDirection == Direction.West) return Compass.SouthWest;
                    else return Compass.North;
                case Direction.East:
                    if (toDirection == Direction.North) return Compass.NorthEast;
                    else if (toDirection == Direction.South) return Compass.SouthEast;
                    else return Compass.East;
                case Direction.West:
                    if (toDirection == Direction.North) return Compass.NorthWest;
                    else if (toDirection == Direction.South) return Compass.SouthWest;
                    else return Compass.East;
            }
            throw new System.Exception("Compass Direction outside parameters");
        }
        #endregion

        #region - Digging Rivers -
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

        private River FindLongestRiverInGroup(RiverGroup group)
        {
            River longest = group.Rivers[0];
            for (int i = 0; i < group.Rivers.Count; i++)
            {
                if (group.Rivers[i].Coordinates.Length > longest.Coordinates.Length)
                {
                    longest = group.Rivers[i];
                }
            }
            return longest;
        }

        private void DigRemainingRivers()
        {
            for (int i = 0; i < rivers.Count; i++)
            {
                if (rivers[i].hasBeenDug) continue;
                DigRiver(rivers[i]);
            }
        }

        private void DigRiver(River river)
        {
            int counter = 0;

            // How wide are we digging this river?
            int size = worldGenerator.rng.Next(1, 5);
            river.Length = river.Coordinates.Length;

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
            int count1 = worldGenerator.rng.Next(fivemin, five);
            if (size < 4)
            {
                count1 = 0;
            }
            int count2 = count1 + worldGenerator.rng.Next(fourmin, four);
            if (size < 3)
            {
                count2 = 0;
                count1 = 0;
            }
            int count3 = count2 + worldGenerator.rng.Next(threemin, three);
            if (size < 2)
            {
                count3 = 0;
                count2 = 0;
                count1 = 0;
            }
            int count4 = count3 + worldGenerator.rng.Next(twomin, two);

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
            for (int i = river.Coordinates.Length - 1; i >= 0; i--)
            {
                WorldTile node = worldMap.GetNode(river.Coordinates[i].x, river.Coordinates[i].y);

                if (counter < count1) node.DigRiver(river, 4, riverBiome);
                else if (counter < count2) node.DigRiver(river, 3, riverBiome);
                else if (counter < count3) node.DigRiver(river, 2, riverBiome);
                else if (counter < count4) node.DigRiver(river, 1, riverBiome);
                else node.DigRiver(river, 0, riverBiome);
                counter++;
            }
            river.hasBeenDug = true;
        }
        #endregion
    }
}