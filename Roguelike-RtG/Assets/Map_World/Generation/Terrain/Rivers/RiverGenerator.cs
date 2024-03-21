using System.Collections.Generic;
using UnityEngine;
using JS.World.Map.Features;

namespace JS.World.Map.Generation
{
    public class RiverGenerator : MonoBehaviour
    {
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private BiomeHelper biomeHelper;

        [SerializeField] private float MinRiverHeight = 0.6f;
        [SerializeField] private int MaxRiverAttempts = 1000;
        [SerializeField] private int MinRiverTurns = 6;
        [SerializeField] private int MaxRiverTurns = 15;
        [SerializeField] private int MinRiverLength = 15;

        private List<River> rivers;
        private List<RiverGroup> riverGroups;

        private int localMapDimensions;
        private float headWaterHeight;

        public void GenerateRivers(int riverCount)
        {
            localMapDimensions = WorldParameters.LOCAL_WIDTH;
            int attempts = 0;
            rivers = new List<River>();

            while (rivers.Count < riverCount && attempts < MaxRiverAttempts)
            {
                attempts++;
                bool findEmpty = attempts < MaxRiverAttempts / 2;
                if (TryGenerateRiver(findEmpty, out River river, out List<WorldTile> tiles))
                {
                    FinalizeRiver(river, tiles);
                }
            }

            BuildRiverGroups();
            DigRiverGroups();
            DigRemainingRivers();

            Features.TerrainData.Rivers = rivers.ToArray();
        }

        private bool TryGenerateRiver(bool findEmptyMountain, out River river, out List<WorldTile> tiles)
        {
            river = new River();
            tiles = new List<WorldTile>();
            var mountain = FindRiverSource(findEmptyMountain);
            WorldTile node;

            if (mountain != null) node = mountain.Nodes[worldGenerator.PRNG.Next(0, mountain.Nodes.Count)];
            else node = WorldMap.GetNode(worldGenerator.PRNG.Next(0, WorldMap.Width - 1), worldGenerator.PRNG.Next(0, WorldMap.Height - 1));

            if (Features.TerrainData.HeightMap[node.x, node.y] < MinRiverHeight) return false;

            //Find river initial direction
            river.CurrentDirection = node.NeighborDirection_Adjacent(FindLowestNeighborNode(node));
            headWaterHeight = node.Altitude; //set initial height

            //Recursively find a path to water
            FindPathToWater(node, river.CurrentDirection, river, tiles);

            //Ensure the generated river meets all requirements
            if (river.TurnCount < MinRiverTurns) return false;
            if (river.TurnCount > MaxRiverTurns) return false;
            if (tiles.Count < MinRiverLength) return false;
            //if (river.Intersections > MaxRiverIntersections) return false;

            if (FindWaterTile(tiles[tiles.Count - 1]) == null)
            {
                var n = tiles[tiles.Count - 1];
                //Debug.Log("Check for lake at " +  n.x + "," + n.y);
            }
            return true;
        }

        private MountainRange FindRiverSource(bool riverLessMountain)
        {
            var mountains = Features.TerrainData.Mountains;
            if (mountains.Length == 0) return null;
            if (!riverLessMountain) return Features.TerrainData.Mountains[worldGenerator.PRNG.Next(0, mountains.Length)];

            var shuffledList = new List<MountainRange>(mountains);
            MathsUtil.ShuffleList(shuffledList, worldGenerator.PRNG);

            for (int i = 0; i < shuffledList.Count; i++)
            {
                if (shuffledList[i].MountainRivers.Count == 0) return shuffledList[i];
            }

            return mountains[worldGenerator.PRNG.Next(0, mountains.Length)];
        }

        private WorldTile FindLowestNeighborNode(WorldTile node)
        {
            var lowestNeighbor = node.neighbors_adj[0];
            for (int i = 0; i < node.neighbors_adj.Count; i++)
            {
                if (Features.TerrainData.HeightMap[node.neighbors_adj[i].x, node.neighbors_adj[i].y] < Features.TerrainData.HeightMap[lowestNeighbor.x, lowestNeighbor.y])
                {
                    lowestNeighbor = node.neighbors_adj[i];
                }
            }
            return lowestNeighbor;
        }

        private void FindPathToWater(WorldTile node, Compass direction, River river, List<WorldTile> tiles)
        {
            //may need to do some additional checks...
            if (node.Rivers.Contains(river))
            {
                Debug.Log("Node already contains river");
                return;
            }
            else if (tiles.Contains(node)) //this caused a stack overflow x2!
            {
                Debug.LogWarning("River already contains unregistered node");
                return;
            }
            //Rivers can't flow up higher than they start
            if (node.Altitude > headWaterHeight) return;
            if (tiles.Count > 1 && node.Altitude > tiles[tiles.Count - 1].Altitude + 0.015f) return;

            if (tiles.Count > 1)
            {
                //if (!node.hasBiome) Debug.LogWarning("Biome not assigned");
                if (node.hasBiome && node.BiomeID == biomeHelper.Mountain.ID)
                {
                    //Debug.Log("River is in mountains at " + node.x + "," + node.y);
                    if (tiles[tiles.Count - 1].BiomeID != biomeHelper.Mountain.ID)
                    {
                        //Debug.Log("Cannot flow up mountain at " + node.x + "," + node.y);
                        return; //Don't flow from a non-mountain tile to a mountain tile
                    }
                }
            }

            tiles.Add(node);

            if (node.Rivers.Count > 0) return;

            if (node.x == 0 || node.y == 0 || node.x == WorldMap.Width - 1 || node.y == WorldMap.Height - 1) return;

            // get neighbors
            WorldTile north = WorldMap.GetNode(node.x, node.y + 1);
            WorldTile south = WorldMap.GetNode(node.x, node.y - 1);
            WorldTile east = WorldMap.GetNode(node.x + 1, node.y);
            WorldTile west = WorldMap.GetNode(node.x - 1, node.y);

            float northValue = GetNodeValue(north, river, tiles);
            float southValue = GetNodeValue(south, river, tiles);
            float eastValue = GetNodeValue(east, river, tiles);
            float westValue = GetNodeValue(west, river, tiles);

            // override flow direction if a tile is significantly lower
            switch (direction)
            {
                case Compass.North:
                    if (Mathf.Abs(northValue - southValue) < 0.1f)
                        southValue = int.MaxValue;
                    break;
                case Compass.South:
                    if (Mathf.Abs(northValue - southValue) < 0.1f)
                        northValue = int.MaxValue;
                    break;
                case Compass.East:
                    if (Mathf.Abs(eastValue - westValue) < 0.1f)
                        westValue = int.MaxValue;
                    break;
                case Compass.West:
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
                    if (river.CurrentDirection != Compass.North)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Compass.North;
                    }
                    FindPathToWater(north, direction, river, tiles);
                }
            }
            else if (min == southValue)
            {
                if (south.IsLand)
                {
                    if (river.CurrentDirection != Compass.South)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Compass.South;
                    }
                    FindPathToWater(south, direction, river, tiles);
                }
            }
            else if (min == eastValue)
            {
                if (east.IsLand)
                {
                    if (river.CurrentDirection != Compass.East)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Compass.East;
                    }
                    FindPathToWater(east, direction, river, tiles);
                }
            }
            else if (min == westValue)
            {
                if (west.IsLand)
                {
                    if (river.CurrentDirection != Compass.West)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Compass.West;
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
                value = Features.TerrainData.HeightMap[node.x, node.y];
            }

            // if neighbor is existing river that is not this one, flow into it
            if (node.Rivers.Count == 0 && !node.IsLand) value = 0;

            return value;
        }

        private int GetNeighborRiverCount(WorldTile node, River river)
        {
            int count = 0;

            for (int i = 0; i < node.neighbors_adj.Count; i++)
            {
                if (node.neighbors_adj[i].Rivers.Count > 0 && node.neighbors_adj[i].Rivers.Contains(river)) count++;
            }

            return count;
        }

        #region - River Groups -
        private void BuildRiverGroups()
        {
            riverGroups = new List<RiverGroup>();

            //loop each tile, checking if it belongs to multiple rivers
            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    WorldTile node = WorldMap.GetNode(x, y);

                    //node only has one or no river(s) running through it
                    if (node.Rivers.Count <= 1) continue;
                    // multiple rivers == intersection

                    RiverGroup group = FindExistingRiverGroup(node);

                    // existing group found -- add to it
                    if (group != null)
                    {
                        for (int n = 0; n < node.Rivers.Count; n++)
                        {
                            if (!group.Rivers.Contains(node.Rivers[n]))
                                group.Rivers.Add(node.Rivers[n]);
                        }
                    }
                    else   //No existing group found - create a new one
                    {
                        //Debug.Log("New River Group Found at " + node.x + "," + node.y);

                        group = new RiverGroup();
                        for (int n = 0; n < node.Rivers.Count; n++)
                        {
                            group.Rivers.Add(node.Rivers[n]);
                        }
                        riverGroups.Add(group);
                    }
                }
            }
        }

        private RiverGroup FindExistingRiverGroup(WorldTile node)
        {
            RiverGroup group = null;
            for (int n = 0; n < node.Rivers.Count; n++)
            {
                River tileriver = node.Rivers[n];
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
            river.Nodes = new RiverNode[tiles.Count];

            for (int i = 0; i < tiles.Count; i++)
            {
                //Mark tile as having a river
                tiles[i].SetRiverPath(river);

                //Coordinates
                river.Nodes[i] = new RiverNode(tiles[i].x, tiles[i].y);

                //Size
                if (i == 0) river.Nodes[i].Size = 1;
                else if (worldGenerator.PRNG.Next(1, 100) > 50) river.Nodes[i].Size = river.Nodes[i - 1].Size + 1;
                else river.Nodes[i].Size = river.Nodes[i - 1].Size;
                river.Nodes[i].Size = Mathf.Clamp(river.Nodes[i].Size, 0, 40);
                //Offset - Set it anywhere between local map size with a border of 10 + river size

                river.Nodes[i].Offset = worldGenerator.PRNG.Next(10 + river.Nodes[i].Size, localMapDimensions - 10 - river.Nodes[i].Size);

                if (i == 0) //Direction is only dependent on following node
                {
                    river.Nodes[i].PathDirection = tiles[i].NeighborDirection_Adjacent(tiles[i + 1]);
                    river.Nodes[i].Flow = tiles[i].NeighborDirection_Adjacent(tiles[i + 1]);
                }
                else if (i == tiles.Count - 1) //Direction is only dependent on previous node
                {
                    var from = tiles[i].NeighborDirection_Adjacent(tiles[i - 1]);
                    var water = FindWaterTile(tiles[i]);
                    if (water == null) //likely the edge of the map
                    {
                        river.Nodes[i].PathDirection = tiles[i].NeighborDirection_Adjacent(tiles[i - 1]);
                    }
                    else
                    {
                        var to = tiles[i].NeighborDirection_Adjacent(water);
                        river.Nodes[i].PathDirection = DirectionHelper.CombineDirections(from, to);
                    }

                    river.Nodes[i].Flow = DirectionHelper.ReflectDirection(tiles[i].NeighborDirection_Adjacent(tiles[i - 1]));
                }
                else
                {
                    var from = tiles[i].NeighborDirection_Adjacent(tiles[i - 1]);
                    var to = tiles[i].NeighborDirection_Adjacent(tiles[i + 1]);
                    river.Nodes[i].PathDirection = DirectionHelper.CombineDirections(from, to);

                    river.Nodes[i].Flow = tiles[i].NeighborDirection_Adjacent(tiles[i + 1]);
                }
            }
            rivers.Add(river);
        }

        private WorldTile FindWaterTile(WorldTile fromTile)
        {
            for (int i = 0; i < fromTile.neighbors_adj.Count; i++)
            {
                if (!fromTile.neighbors_adj[i].IsLand) return fromTile.neighbors_adj[i];
            }
            return null;
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
                if (group.Rivers[i].Nodes.Length > longest.Nodes.Length)
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
                if (!rivers[i].hasBeenDug) DigRiver(rivers[i]);
            }
        }

        private void DigRiver(River river)
        {
            river.Length = river.Nodes.Length;
            for (int i = river.Nodes.Length - 1; i >= 0; i--)
            {
                WorldTile node = WorldMap.GetNode(river.Nodes[i].x, river.Nodes[i].y);
                node.AddRiver(river);
            }
            river.hasBeenDug = true;
        }
        #endregion
    }
}