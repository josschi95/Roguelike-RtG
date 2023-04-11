using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldGeneration
{
    //[System.Serializable]
    public class TerrainNode
    {
        private Grid<TerrainNode> grid;
        public int x { get; private set; }
        public int y { get; private set; }

        #region - Altitude -
        public bool isTectonicPoint { get; set; }
        public bool isNotWater { get; private set; }
        public float altitude { get; private set; }
        public AltitudeZone altitudeZone { get; private set; }
        #endregion

        #region - Temperature -
        public float heatValue { get; private set; }
        public float avgAnnualTemperature_F => heatValue * 100;
        public float avgAnnualTemperature_C => Temperature.FarenheitToCelsius(heatValue * 100);
        public TemperatureZone temperatureZone { get; private set; }
        #endregion

        #region - Precipitation -
        public float precipitationValue { get; private set; }
        public float annualPrecipitation_cm => precipitationValue * 400;
        public PrecipitationZone precipitationZone { get; private set; }
        #endregion

        public SecondaryDirections windDirection;// { get; private set; }
        public float airPressure; // { get; private set; }

        public TerrainNode[] neighbors { get; private set; }

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

        public List<River> rivers;

        public Biome biome { get; private set; }

        private BiomeGroup biomeGroup;

        public BiomeGroup BiomeGroup
        {
            get => biomeGroup;
            set
            {
                biomeGroup = value;
            }
        }

        private Island island;

        public Island Island
        {
            get => island;
            set
            {
                island = value;
            }
        }

        private Lake lake;

        public Lake Lake
        {
            get => lake;
            set
            {
                lake = value;
            }
        }

        private Settlement settlement;
        public Settlement Settlement
        {
            get => settlement;
            set
            {
                settlement = value;
            }
        }
        #endregion

        public TerrainNode(Grid<TerrainNode> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;

            rivers = new List<River>();
        }

        public void SetNeighbors()
        {
            var neighbors = new List<TerrainNode>();

            if (x != 0) neighbors.Add(grid.GetGridObject(x - 1, y));
            if (x != grid.GetWidth() - 1) neighbors.Add(grid.GetGridObject(x + 1, y));
            if (y != 0) neighbors.Add(grid.GetGridObject(x, y - 1));
            if (y != grid.GetHeight() - 1) neighbors.Add(grid.GetGridObject(x, y + 1));
            this.neighbors = new TerrainNode[neighbors.Count];

            for (int i = 0; i < this.neighbors.Length; i++)
            {
                this.neighbors[i] = neighbors[i];
            }
        }

        #region - Node Values -
        public void SetAltitude(float value, AltitudeZone zone)
        {
            altitude = value;
            isNotWater = zone.isLand;
            altitudeZone = zone;
            if (zone.isMountain) CheckNeighborMountains();
            if (zone.isLand) CheckNeighborIslands();
            else CheckNeighborLakes();
        }

        public void SetTemperatureValues(float value, TemperatureZone zone)
        {
            heatValue = value;
            temperatureZone = zone;
        }

        public void SetPrecipitationValues(float value, PrecipitationZone zone)
        {
            precipitationValue = value;
            precipitationZone = zone;
        }
        
        public void AddMoisture(float value)
        {
            precipitationValue = Mathf.Clamp(precipitationValue + value, 0, 1);
        }
        #endregion

        #region - Mountains -
        private void CheckNeighborMountains()
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].Mountain != null)
                {
                    SetMountainNode(neighbors[i].Mountain);
                }
            }

            if (mountain != null) return;
            var newRange = new MountainRange();
            SetMountainNode(newRange);
        }

        public void SetMountainNode(MountainRange newMountain)
        {
            if (mountain != null)
            {
                if (newMountain == mountain) return;
                //Destroys the current mountain and adds nodes to new one
                else mountain.MergeMountain(newMountain);
            }

            mountain = newMountain;
            if (!mountain.Nodes.Contains(this))
                mountain.Nodes.Add(this);
        }
        #endregion

        #region - Islands -
        private void CheckNeighborIslands()
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].Island != null)
                {
                    SetIslandNode(neighbors[i].Island);
                }
            }

            if (island != null) return;
            var newIsland = new Island();
            SetIslandNode(newIsland);
        }

        private void SetIslandNode(Island newIsland)
        {
            if (island != null)
            {
                if (newIsland == island) return;
                else island.MergeIslands(newIsland);
            }

            island = newIsland;
            if (!island.Nodes.Contains(this))
                island.Nodes.Add(this);
        }
        #endregion

        #region - Lakes -
        private void CheckNeighborLakes()
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].Lake != null)
                {
                    SetLakeNode(neighbors[i].lake);
                }
            }

            if (lake != null) return;
            var newIsland = new Lake();
            SetLakeNode(newIsland);
        }

        private void SetLakeNode(Lake newLake)
        {
            if (lake != null)
            {
                if (newLake == lake) return;
                else lake.MergeLakes(newLake);
            }

            lake = newLake;
            lake.Add(this);
        }
        #endregion

        #region - Rivers -
        public int GetNeighborRiverCount(River river)
        {
            int count = 0;

            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].rivers.Count > 0 && neighbors[i].rivers.Contains(river)) count++;
            }

            return count;
        }

        public void SetRiverPath(River river)
        {
            if (!isNotWater) return;
            if (rivers.Contains(river)) return;

            //Debug.Log("Adding River " + river.ID + " to node " + x + "," + y);
            rivers.Add(river);
        }

        public void SetRiverNode(River river)
        {
            SetRiverPath(river);
            isNotWater = false;
            altitude = Mathf.Clamp(altitude - 0.25f, 0, 1);
            precipitationValue = 1;

            for (int i = 0; i < neighbors.Length; i++)
            {
                neighbors[i].AddMoisture(0.25f);
            }

            //Either some kind of callback or loop through everything again at the end
        }

        public void DigRiver(River river, int size, Biome riverBiome)
        {
            if (size < 1) return;

            SetRiverNode(river);
            SetBiome(riverBiome);

            if (size == 1) return;

            for (int i = 0; i < neighbors.Length; i++)
            {
                //neighbors[i].DigRiver(river, size - 1, riverBiome);
            }
        }
        #endregion

        #region - Biome -
        public void SetBiome(Biome biome)
        {
            this.biome = biome;
            CheckNeighborBiomes();
        }

        //Check if this node is surrounded by a biome of different types
        public void AdjustToNeighborBiomes()
        {
            if (!isNotWater) return; //water doesn't have a biome
            int differentNeighbors = 0; //number of neighbors with a different biome
            Biome neighborBiome = null; //the biome this node will switch to
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].biome == null) continue;
                if (neighbors[i].biome == biome) continue;
                neighborBiome = neighbors[i].biome;
                differentNeighbors++;
            }
            if (differentNeighbors >= 3) AdjustToBiome(neighborBiome);
        }

        public void AdjustToBiome(Biome biome)
        {
            SetBiome(biome);

            heatValue = Mathf.Clamp(heatValue,
                Temperature.CelsiusToFarenheit(biome.MinAvgTemp) / 100f,
                Temperature.CelsiusToFarenheit(biome.MaxAvgTemp) / 100f);

            precipitationValue = Mathf.Clamp(precipitationValue,
                biome.MinPrecipitation / 400f,
                biome.MaxPrecipitation / 400f
                );
        }

        private void CheckNeighborBiomes()
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].biome != biome) continue;
                if (neighbors[i].biomeGroup != null)
                {
                    MergeBiomes(neighbors[i].BiomeGroup);
                }
            }

            if (biomeGroup != null) return;
            var newBiome = new BiomeGroup(biome);
            MergeBiomes(newBiome);
        }

        private void MergeBiomes(BiomeGroup newBiomeGroup)
        {
            if (biomeGroup != null)
            {
                if (newBiomeGroup == biomeGroup) return;
                else biomeGroup.MergeBiomes(newBiomeGroup);
            }

            biomeGroup = newBiomeGroup;
            if (!biomeGroup.Nodes.Contains(this))
                biomeGroup.Nodes.Add(this);
        }
        #endregion
    }
}