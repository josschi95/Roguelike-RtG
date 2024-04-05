using UnityEngine;

namespace JS.World.Map.Features
{
    public static class ClimateMath // ClimateMath
    {
        public static float[] TemperatureZones; // Default [0.05, 0.18, 0.4, 0.6, 0.8, 1.0]
        public static float[] PrecipitationZones; // Default [0.2, 0.4, 0.6, 0.8, 0.9, 1.0]

        /// <summary>
        /// Creates a heat map based on a nodes distance from the equator.
        /// By default, the equator is located at the center of the map.
        /// </summary>
        public static float[,] GenerateHeatMap(float[,] heightMap, System.Random rng)
        {
            float[,] heatMap = new float[heightMap.GetLength(0), heightMap.GetLength(1)];

            //float equator = heatMap.GetLength(1) / 2f;

            // The difference in latitude between the northern edge of the map and the southern edge
            int latitudeDiff = Mathf.Abs(TerrainData.NorthLatitude - TerrainData.SouthLatitude);

            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    float relativeLatitude = (float)y / heightMap.GetLength(1);
                    float netLatitude = TerrainData.SouthLatitude + (relativeLatitude * latitudeDiff);
                    float distanceFromEquator = Mathf.Abs(netLatitude) / 90.0f;

                    // Initial value is flipped parabola to replicate temperature dropoff in temp as dist from equator increases
                    var heatValue = 1 - Mathf.Pow(distanceFromEquator, 2); // However this is not very accurate to the real-life gradient

                    // Heat value is then decreased by the difference between sea level and height, as temperature decreases with higher altitudes
                    if (heightMap[x, y] > WorldParameters.SEA_LEVEL) heatValue -= Mathf.Abs(heightMap[x, y] - WorldParameters.SEA_LEVEL);

                    // Heat value is then slightly randomized by +/- 0.05f
                    heatValue += rng.Next(-5, 5) * 0.01f;

                    heatMap[x, y] = Mathf.Clamp(heatValue, 0, 1); // clamp between 0 and 1
                }
            }
            return heatMap;
        }

        public static float[,] GetAirPressureMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            float[,] pressureMap = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float altitude = Mathf.Clamp(heightMap[x, y], WorldParameters.SEA_LEVEL, 1);
                    pressureMap[x, y] = 1 - Mathf.Sqrt(0.25f * altitude);
                }
            }
            return pressureMap;
        }

        public static Compass[,] GenerateWindMap(float[,] map)
        {
            Debug.Log("Not accounting for the doldrums.");

            int width = map.GetLength(0);
            int height = map.GetLength(1);
            //var equator = height / 2f;
            //float windZoneLength = equator / 3f;
            Compass[,] windMap = new Compass[width, height];

            // The difference in latitude between the northern edge of the map and the southern edge
            int latitudeDiff = Mathf.Abs(TerrainData.NorthLatitude - TerrainData.SouthLatitude);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float relativeLatitude = (float)y / map.GetLength(1);
                    float netLatitude = TerrainData.SouthLatitude + (relativeLatitude * latitudeDiff);

                    switch (netLatitude)
                    {
                        case < -60: // Polar Easterlies
                            windMap[x, y] = Compass.NorthEast;
                            break;
                        case < -30: // Prevailing Westerlies
                            windMap[x, y] = Compass.SouthWest;
                            break;
                        case <= 0: // Tropical Easterlies / Southeast Trade Winds
                            windMap[x, y] = Compass.NorthEast;
                            break;

                        // The doldrums at 0 

                        case < 30: // Tropical Easterlies / Northeast Trade Winds
                            windMap[x, y] = Compass.SouthEast;
                            break;
                        case < 60: // Prevailing Westerlies
                            windMap[x, y] = Compass.NorthWest;
                            break;
                        default: // Polar Easterlies
                            windMap[x, y] = Compass.SouthEast;
                            break;
                    }

                    /*float distanceFromEquator = Mathf.Abs(equator - y);
                    int windZone = Mathf.Clamp(Mathf.FloorToInt(distanceFromEquator / windZoneLength), 0, 2);

                    if (windZone == 1) //Secondary Direction is East
                    {
                        if (y > equator) windMap[x, y] = Compass.NorthEast;
                        else windMap[x, y] = Compass.SouthEast;
                    }
                    else //Secondary direction is West
                    {
                        if (y > equator) windMap[x, y] = Compass.SouthWest;
                        else windMap[x, y] = Compass.NorthWest;
                    }*/
                }
            }

            return windMap;
        }

        public static int GetHeatIndex(float heatValue)
        {
            for (int i = 0; i < TemperatureZones.Length; i++)
            {
                if (heatValue <= TemperatureZones[i]) return i;
            }
            throw new System.Exception($"Node temperature {heatValue} is outside bounds of designated zones.");
        }

        public static int GetPrecipitationZone(float moistureValue)
        {
            for (int i = 0; i < PrecipitationZones.Length; i++)
            {
                if (moistureValue <= PrecipitationZones[i]) return i;
            }
            throw new System.Exception($"Node precipitation {moistureValue} is outside bounds of designated zones.");
        }
    }
}