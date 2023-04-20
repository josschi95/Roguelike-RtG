using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldGeneration
{
    //Do Not Serialize this class, results in a recursive serialization issue
    public class WorldTile
    {
        private Grid<WorldTile> grid;
        public int x { get; private set; }
        public int y { get; private set; }

        //pathfinding
        public int gCost; //the movement cost to move from the start tile to this tile, following the existing path
        public int hCost; //the estimated movement cost to move from this tile to the end tile
        public int fCost; //the current best guess as to the cost of the path
        public WorldTile cameFromTile;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        #region - Altitude -
        public bool isTectonicPoint { get; set; }
        public bool isNotWater { get; private set; }
        public float altitude { get; private set; }
        public AltitudeZone altitudeZone { get; private set; }
        #endregion

        #region - Temperature -
        public float heatValue { get; private set; }
        public float avgAnnualTemperature_F => Mathf.Clamp(heatValue - 0.1f, 0, 1) * 100;
        public float avgAnnualTemperature_C => Temperature.FarenheitToCelsius(avgAnnualTemperature_F);
        public TemperatureZone temperatureZone { get; private set; }
        #endregion

        #region - Precipitation -
        public float precipitationValue { get; private set; }
        public float annualPrecipitation_cm => precipitationValue * 400;
        public PrecipitationZone precipitationZone { get; private set; }
        #endregion

        public SecondaryDirections windDirection;// { get; private set; }
        public float airPressure; // { get; private set; }
        public bool isCoast { get; set; }

        public WorldTile[] neighbors { get; private set; }

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

        public Biome PrimaryBiome { get; private set; }
        public List<Biome> SecondaryBiomes { get; private set; }

        private BiomeGroup biomeGroup;

        public BiomeGroup BiomeGroup
        {
            get => biomeGroup;
            set
            {
                biomeGroup = value;
            }
        }

        private Island island = null;

        public Island Island
        {
            get => island;
            set
            {
                island = value;
            }
        }

        private Lake lake = null;

        public Lake Lake
        {
            get => lake;
            set
            {
                lake = value;
            }
        }

        private Settlement settlement = null;
        public Settlement Settlement
        {
            get => settlement;
            set
            {
                settlement = value;
            }
        }

        private Settlement territory = null;
        public Settlement Territory
        {
            get => territory;
            set
            {
                territory = value;
            }
        }
        #endregion

        public WorldTile(Grid<WorldTile> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;

            rivers = new List<River>();
            SecondaryBiomes = new List<Biome>();
        }

        public void SetNeighbors()
        {
            var neighbors = new List<WorldTile>();

            if (x != 0) neighbors.Add(grid.GetGridObject(x - 1, y));
            if (x != grid.GetWidth() - 1) neighbors.Add(grid.GetGridObject(x + 1, y));
            if (y != 0) neighbors.Add(grid.GetGridObject(x, y - 1));
            if (y != grid.GetHeight() - 1) neighbors.Add(grid.GetGridObject(x, y + 1));
            this.neighbors = new WorldTile[neighbors.Count];

            for (int i = 0; i < this.neighbors.Length; i++)
            {
                this.neighbors[i] = neighbors[i];
            }
        }

        public Direction NeighborDirection(WorldTile tile)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (tile == neighbors[i]) break;
                if (i == neighbors.Length - 1) throw new System.Exception("This is not a neightbor tile!");
            }

            if (tile.x != x)
            {
                if (tile.x > x) return Direction.East;
                return Direction.West;
            }
            else
            {
                if (tile.y > y) return Direction.North;
                return Direction.South;
            }
        }

        #region - Tile Values -
        public void SetAltitude(float value, AltitudeZone zone)
        {
            altitude = value;
            isNotWater = zone.isLand;
            altitudeZone = zone;
            AddSecondaryBiome(zone.SecondaryBiome);
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
        public void CheckNeighborMountains()
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].Mountain != null)
                {
                    SetMountainTile(neighbors[i].Mountain);
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
        public int GetNeighborRiverCount(River river)
        {
            int count = 0;

            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].rivers.Count > 0 && neighbors[i].rivers.Contains(river)) count++;
            }

            return count;
        }

        public void AddRiver(River river)
        {
            SetRiverPath(river);
            altitude = Mathf.Clamp(altitude - 0.25f, 0, 1);
            precipitationValue = Mathf.Clamp(precipitationValue + 0.25f, 0.5f, 0.9f);

            for (int i = 0; i < neighbors.Length; i++)
            {
                neighbors[i].AddMoisture(0.15f);
            }

            //Either some kind of callback or loop through everything again at the end
        }

        public void SetRiverPath(River river)
        {
            if (!isNotWater) return;
            if (rivers.Contains(river)) return;

            //Debug.Log("Adding River " + river.ID + " to tile " + x + "," + y);
            rivers.Add(river);
        }

        public void DigRiver(River river, int size, Biome riverBiome)
        {
            if (size < 1) return; //Will need to adjust this later, the size of the river will ultimately decide its width in Local Tiles

            AddRiver(river);
            AddSecondaryBiome(riverBiome);
        }
        #endregion

        #region - Biome -
        public void SetBiome(Biome biome)
        {
            PrimaryBiome = biome;
            CheckNeighborBiomes();
        }

        /// <summary>
        /// Adds a secondary biome to the tile such as a river, mountain (or possibly coast)
        /// </summary>
        private void AddSecondaryBiome(Biome biome)
        {
            if (biome == null) return;
            if (SecondaryBiomes.Contains(biome)) return;
            SecondaryBiomes.Add(biome);
        }

        //Check if this tile is surrounded by a biome of different types
        public void AdjustToNeighborBiomes()
        {
            if (!isNotWater) return; //water doesn't have a biome (yet)
            int differentNeighbors = 0; //number of neighbors with a different biome
            Biome neighborBiome = null; //the biome this tile will switch to
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i].PrimaryBiome == null) continue;
                if (!neighbors[i].isNotWater) continue; //don't adjust to water biomes
                if (neighbors[i].PrimaryBiome == PrimaryBiome) continue;
                neighborBiome = neighbors[i].PrimaryBiome;
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
                if (neighbors[i].PrimaryBiome != PrimaryBiome) continue;
                if (neighbors[i].biomeGroup != null)
                {
                    MergeBiomes(neighbors[i].BiomeGroup);
                }
            }

            if (biomeGroup != null) return;
            var newBiome = new BiomeGroup(PrimaryBiome);
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