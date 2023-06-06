using JS.Primitives;
using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldMap
{
    public class LocalMapGenerator : MonoBehaviour
    {
        [SerializeField] private Vector3IntVariable worldPos;
        private Vector2Int regionPos;

        [Space]

        [SerializeField] private LocalMapDisplay localMapDisplay;
        [SerializeField] private WorldData worldData;

        [SerializeField] private BiomeHelper biomeHelper;
        [SerializeField] private WorldGenerationParameters worldGenParams;
        [SerializeField] private SettlementData settlementData;
        //private System.Random PRNG;
        private int seed;

        #region - Perlin -
        [Header("Perlin Noise")]
        [SerializeField] private float noiseScale = 30;
        [Tooltip("The number of iterations of Perlin Noise over an area")]
        [SerializeField] private int octaves = 4;
        [Range(0, 1)]
        [Tooltip("Controls decrease in amplitude of subsequent octaves")]
        [SerializeField] private float persistence = 0.5f;
        [Tooltip("Controls increase in frequency of octaves")]
        [SerializeField] private float lacunarity = 2f;
        [SerializeField] private Vector2 offset;
        #endregion

        //Called from GameEventListeners on scene loaded or map change
        public void CheckForNode() => GenerateLocalMap();

        private void GenerateLocalMap()
        {
            var worldX = Mathf.FloorToInt(worldPos.Value.x / worldGenParams.LocalDimensions.x);
            var worldY = Mathf.FloorToInt(worldPos.Value.y / worldGenParams.LocalDimensions.y);
            regionPos.x = worldPos.Value.x % worldGenParams.LocalDimensions.x;
            regionPos.y = worldPos.Value.y % worldGenParams.LocalDimensions.y;
            //var node = worldData.GetNode(worldX, worldY);

            //Seed is equal to the world tile + the regional x + y
            seed = worldData.TerrainData.SeedMap[worldX, worldY] + regionPos.x + regionPos.y;
            //PRNG = new System.Random(seed);

            //var biome = biomeHelper.GetBiome(node.BiomeID);
            //Each biome should hold a list of ground and feature tiles
            //as well as unique features which may appear

            LocalMapInfo info = new LocalMapInfo();
            info.x = worldX; info.y = worldY;
            info.PerlinMap = PerlinNoise.GenerateHeightMap(worldGenParams.LocalDimensions.x, seed, noiseScale, octaves, persistence, lacunarity, offset);
            info.featurePoints = Poisson.GeneratePoints(seed, 10, worldGenParams.LocalDimensions);

            HandleRivers(worldX, worldY);

            localMapDisplay.DisplayMap(info);
        }

        #region - Rivers -
        private void HandleRivers(int worldX, int worldY)
        {
            var river = worldData.TerrainData.FindRiverAt(worldX, worldY, out var index);
            if (river == null) return; //There is no river

            if (!RiverIsInMap(river.Nodes[index], out int regionIndex)) return; //The river doesn't pass through this tile
            var direction = GetRiverDirection(river.Nodes[index], regionIndex);

            int startOffset = river.Nodes[index].Offset; //the offset at which the river enters the region map
                                                         //Debug.Log("Region Start: " + startOffset);
            int endOffset = river.Nodes[index].Offset; //the offset at which the river leaves the region map
            if (index < river.Nodes.Length - 1) endOffset = river.Nodes[index + 1].Offset;
            //Debug.Log("Region End: " + endOffset);

            float diff = endOffset - startOffset;
            var step = Mathf.RoundToInt(diff / worldGenParams.RegionDimensions.x); //the points between the two
                                                                                   //Debug.Log("Step: " + step);

            startOffset += regionIndex * step; //Increase by 1 step for each region tile the river has traveled
            if (regionIndex < worldGenParams.RegionDimensions.x - 1) endOffset = startOffset + step;

            var riverWidth = Mathf.RoundToInt(river.Nodes[index].Size * 0.5f);
            DigRiver(direction, riverWidth, startOffset, endOffset);
        }

        /// <summary>
        /// Returns true if the given river flows through this part of the map.
        /// </summary>
        /// <param name="river"></param>
        /// <param name="regionIndex">Regional position within river's flow.</param>
        /// <returns></returns>
        private bool RiverIsInMap(RiverNode river, out int regionIndex)
        {
            regionIndex = 0;
            Debug.Log("Flowing: " + river.Flow.ToString());

            switch (river.PathDirection)
            {
                case Compass.North:
                    {
                        if (river.Flow == Compass.North) regionIndex = regionPos.y;
                        else regionIndex = worldGenParams.RegionDimensions.y - regionPos.y - 1;

                        return regionPos.x == 1;
                    }
                case Compass.South:
                    {
                        if (river.Flow == Compass.North) regionIndex = regionPos.y;
                        else regionIndex = worldGenParams.RegionDimensions.y - regionPos.y - 1;

                        return regionPos.x == 1;
                    }
                case Compass.East:
                    {
                        if (river.Flow == Compass.East) regionIndex = regionPos.x;
                        else regionIndex = worldGenParams.RegionDimensions.x - regionPos.x - 1;
                        
                        return regionPos.y == 1;
                    }
                case Compass.West:
                    {
                        if (river.Flow == Compass.East) regionIndex = regionPos.x;
                        else regionIndex = worldGenParams.RegionDimensions.x - regionPos.x - 1;

                        return regionPos.y == 1;
                    }
                case Compass.NorthEast:
                    {
                        if (regionPos.x == 2 && regionPos.y == 2) return false;
                        if (regionPos.x == 0 || regionPos.y == 0) return false;

                        if (regionPos.x == 1 && regionPos.y == 1) regionIndex = 1;
                        else if (river.Flow == Compass.North && regionPos.y == 2) regionIndex = 2;
                        else if (river.Flow == Compass.East && regionPos.x == 2) regionIndex = 2;

                        return true;
                    }
                case Compass.SouthEast:
                    {
                        if (regionPos.x == 2 && regionPos.y == 0) return false;
                        if (regionPos.x == 0 || regionPos.y == 2) return false;

                        if (regionPos.x == 1 && regionPos.y == 1) regionIndex = 1;
                        else if (river.Flow == Compass.South && regionPos.y == 0) regionIndex = 2;
                        else if (river.Flow == Compass.East && regionPos.x == 2) regionIndex = 2;

                        return true;
                    }
                case Compass.NorthWest:
                    {
                        if (regionPos.x == 0 && regionPos.y == 2) return false;
                        if (regionPos.x == 2 || regionPos.y == 0) return false;

                        if (regionPos.x == 1 && regionPos.y == 1) regionIndex = 1;
                        else if (river.Flow == Compass.North && regionPos.y == 2) regionIndex = 2;
                        else if (river.Flow == Compass.West && regionPos.x == 0) regionIndex = 2;

                        return true;
                    }
                case Compass.SouthWest:
                    {
                        if (regionPos.x == 0 && regionPos.y == 0) return false;
                        if (regionPos.x == 2 || regionPos.y == 2) return false;

                        if (regionPos.x == 1 && regionPos.y == 1) regionIndex = 1;
                        else if (river.Flow == Compass.South && regionPos.y == 0) regionIndex = 2;
                        else if (river.Flow == Compass.West && regionPos.x == 0) regionIndex = 2;

                        return true;
                    }
                default: return false;
            }
        }

        private Compass GetRiverDirection(RiverNode river, int regionIndex)
        {
            switch(river.PathDirection)
            {
                case Compass.North: return Compass.North;
                case Compass.South: return Compass.South;
                case Compass.East: return Compass.East;
                case Compass.West: return Compass.West;
                case Compass.NorthEast:
                    if (regionIndex == 2) return river.Flow;
                    else if (regionIndex == 1) return Compass.NorthEast;
                    if (river.Flow == Compass.North) return Compass.East;
                    else return Compass.North;
                case Compass.NorthWest:
                    if (regionIndex == 2) return river.Flow;
                    else if (regionIndex == 1) return Compass.NorthWest;
                    if (river.Flow == Compass.North) return Compass.West;
                    else return Compass.North;
                case Compass.SouthEast:
                    if (regionIndex == 2) return river.Flow;
                    else if (regionIndex == 1) return Compass.SouthEast;
                    if (river.Flow == Compass.South) return Compass.East;
                    else return Compass.South;
                case Compass.SouthWest:
                    if (regionIndex == 2) return river.Flow;
                    else if (regionIndex == 1) return Compass.SouthWest;
                    if (river.Flow == Compass.South) return Compass.West;
                    else return Compass.South;

            }
            throw new System.Exception("Fuck off.");
        }

        private void DigRiver(Compass direction, int riverWidth, int startOffset, int endOffset)
        {
            var grid = GridManager.ActiveGrid.Grid;
            var centerLine = new List<GridNode>();

            var width = worldGenParams.LocalDimensions.x;
            var height = worldGenParams.LocalDimensions.y;
            int middleY = Mathf.RoundToInt(width / 2);
            int middleX = Mathf.RoundToInt(height / 2);
            


            Debug.Log(startOffset + ", " + endOffset);
            if (direction == Compass.North || direction == Compass.South)
            {
                var start = grid.GetGridObject(startOffset, 0);
                var end = grid.GetGridObject (endOffset, height - 1);
                centerLine = Pathfinding.instance.FindNodePath(start, end);
            }
            else if (direction == Compass.East || direction == Compass.West)
            {
                var start = grid.GetGridObject(0, startOffset);
                var end = grid.GetGridObject(width - 1, endOffset);
                centerLine = Pathfinding.instance.FindNodePath(start, end);
            }
            else if (direction == Compass.NorthEast)
            {
                var start = grid.GetGridObject(startOffset, height - 1);
                var mid = grid.GetGridObject(middleX, middleY);
                var end = grid.GetGridObject(width - 1, endOffset);

                centerLine = Pathfinding.instance.FindNodePath(start, mid);
                centerLine.AddRange(Pathfinding.instance.FindNodePath(mid, end));
            }
            else if (direction == Compass.SouthEast)
            {
                var start = grid.GetGridObject(startOffset, 0);
                var mid = grid.GetGridObject(middleX, middleY);
                var end = grid.GetGridObject(width - 1, endOffset);

                centerLine = Pathfinding.instance.FindNodePath(start, mid);
                centerLine.AddRange(Pathfinding.instance.FindNodePath(mid, end));
            }
            else if (direction == Compass.SouthWest)
            {
                var start = grid.GetGridObject(startOffset, 0);
                var mid = grid.GetGridObject(middleX, middleY);
                var end = grid.GetGridObject(0, endOffset);

                centerLine = Pathfinding.instance.FindNodePath(start, mid);
                centerLine.AddRange(Pathfinding.instance.FindNodePath(mid, end));
            }
            else if (direction == Compass.NorthWest)
            {
                var start = grid.GetGridObject(startOffset, height - 1);
                var mid = grid.GetGridObject(middleX, middleY);
                var end = grid.GetGridObject(0, endOffset);

                centerLine = Pathfinding.instance.FindNodePath(start, mid);
                centerLine.AddRange(Pathfinding.instance.FindNodePath(mid, end));
            }

            foreach (var point in centerLine)
            {
                var nodes = Pathfinding.instance.GetNodesInRange_Circle(point, riverWidth);
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].isWater = true;
                }
            }
        }


        #endregion
    }
}