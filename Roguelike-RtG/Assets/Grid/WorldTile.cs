using JS.WorldMap.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldMap
{
    //Do Not Serialize this class, results in a recursive serialization issue
    public class WorldTile : IHeapItem<WorldTile>
    {
        private Grid<WorldTile> grid;
        public int x { get; private set; }
        public int y { get; private set; }

        //pathfinding
        public int gCost; //the movement cost to move from the start tile to this tile, following the existing path
        public int hCost; //the estimated movement cost to move from this tile to the end tile
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

        public int movementCost;
        public WorldTile cameFromTile;

        public int CompareTo(WorldTile other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0) compare = hCost.CompareTo(other.hCost);
            return -compare; //want to return 1 if it's lower, so return negative value
        }

        #region - Altitude -
        public bool isTectonicPoint { get; set; }
        public bool IsLand { get; private set; }
        public float Altitude { get; private set; }
        #endregion

        #region - Temperature -
        public float heatValue { get; private set; }
        public int TempZoneID { get; private set; }
        #endregion

        #region - Precipitation -
        public float precipitationValue { get; private set; }
        public int PrecipitationZoneID { get; private set; }
        #endregion

        public Compass windDirection;// { get; private set; }
        public float airPressure; // { get; private set; }

        public List<WorldTile> neighbors_all { get; private set; }
        public List<WorldTile> neighbors_adj { get; private set; }

        #region - Map Features -
        private MountainRange mountain;
        public MountainRange Mountain
        {
            get => mountain;
            set
            {
                mountain = value;
            }
        }

        public List<River> Rivers;

        public int BiomeID { get; private set; }
        public bool hasBiome { get; private set; } = false;

        private BiomeGroup biomeGroup;
        public BiomeGroup BiomeGroup
        {
            get => biomeGroup;
            set => biomeGroup = value;
        }
        #endregion

        public WorldTile(Grid<WorldTile> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;

            neighbors_all = new List<WorldTile>();
            neighbors_adj = new List<WorldTile>();

            Rivers = new List<River>();
        }

        public void SetNeighbors()
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

        public Compass NeighborDirection_Adjacent(WorldTile tile)
        {
            if (!neighbors_adj.Contains(tile)) throw new System.Exception(tile.x + "," + tile.y + 
                " is is not an adjacent tile to " + x + "," + y + "." + "");

            if (tile.x != x)
            {
                if (tile.x > x) return Compass.East;
                return Compass.West;
            }
            else
            {
                if (tile.y > y) return Compass.North;
                return Compass.South;
            }
        }

        #region - Tile Values -
        public void SetAltitude(float value, bool isLand)
        {
            Altitude = value;
            IsLand = isLand;
        }

        public void SetTemperatureValues(float value, int zoneID)
        {
            heatValue = value;
            TempZoneID = zoneID;
        }

        public void SetPrecipitationValues(float value, int zoneID)
        {
            precipitationValue = value;
            PrecipitationZoneID = zoneID;
        }
        
        public void AddMoisture(float value)
        {
            precipitationValue = Mathf.Clamp(precipitationValue + value, 0, 1);
        }
        #endregion

        #region - Mountains -
        public void CheckNeighborMountains()
        {
            for (int i = 0; i < neighbors_adj.Count; i++)
            {
                if (neighbors_adj[i].Mountain != null)
                {
                    SetMountainTile(neighbors_adj[i].Mountain);
                }
            }

            if (mountain != null) return;
            var newRange = new MountainRange();
            SetMountainTile(newRange);
        }

        public void SetMountainTile(MountainRange newMountain)
        {
            if (mountain != null)
            {
                if (newMountain == mountain) return;
                //Destroys the current mountain and adds tiles to new one
                else mountain.MergeMountain(newMountain);
            }

            mountain = newMountain;
            if (!mountain.Nodes.Contains(this))
                mountain.Nodes.Add(this);
        }
        #endregion

        #region - Rivers -
        public void AddRiver(River river)
        {
            SetRiverPath(river);
            Altitude = Mathf.Clamp(Altitude - 0.25f, 0, 1);
            precipitationValue = Mathf.Clamp(precipitationValue + 0.25f, 0.5f, 0.9f);

            for (int i = 0; i < neighbors_adj.Count; i++)
            {
                neighbors_adj[i].AddMoisture(0.15f);
            }
        }

        public void SetRiverPath(River river)
        {
            if (!IsLand) return;
            if (Rivers.Contains(river)) return;

            //Debug.Log("Adding River " + river.ID + " to tile " + x + "," + y);
            Rivers.Add(river);
        }
        #endregion

        #region - Biome -
        public void SetBiome(Biome biome)
        {
            BiomeID = biome.ID;
            hasBiome = true;
            CheckNeighborBiomes();
        }

        private void CheckNeighborBiomes()
        {
            for (int i = 0; i < neighbors_adj.Count; i++)
            {
                if (!neighbors_adj[i].hasBiome) continue;
                if (neighbors_adj[i].BiomeID != BiomeID) continue;
                if (neighbors_adj[i].biomeGroup != null)
                {
                    MergeBiomes(neighbors_adj[i].BiomeGroup);
                }
            }

            if (biomeGroup != null) return;
            var newBiome = new BiomeGroup(BiomeID);
            MergeBiomes(newBiome);
        }

        private void MergeBiomes(BiomeGroup newBiomeGroup)
        {
            if (biomeGroup != null)
            {
                if (newBiomeGroup == biomeGroup) return;
                else TerrainHelper.MergeBiomes(biomeGroup, newBiomeGroup);
            }

            biomeGroup = newBiomeGroup;
            if (!biomeGroup.Nodes.Contains(this))
                biomeGroup.Nodes.Add(this);
        }
        #endregion
    }
}