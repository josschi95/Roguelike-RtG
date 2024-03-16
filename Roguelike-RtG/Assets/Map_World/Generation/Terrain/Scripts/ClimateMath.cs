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

            float equator = heatMap.GetLength(1) / 2f;

            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    float distanceFromEquator = Mathf.Abs(equator - y) / equator; // I could swap x and y so I only need to calculate this once per column
                    
                    // Initial value is flipped parabola to replicate temperature dropoff in temp as dist from equator increases
                    var heatValue = 1 - Mathf.Pow(distanceFromEquator, 2);

                    // Heat value is then decreased by the difference between sea level and height, as temperature decreases with higher altitudes
                    if (heightMap[x, y] > WorldParameters.SEA_LEVEL) heatValue -= Mathf.Abs(heightMap[x, y] - WorldParameters.SEA_LEVEL);

                    // Heat value is then slightly randomized by +/- 0.05f
                    heatValue += rng.Next(-5, 5) * 0.01f;

                    heatMap[x, y] = Mathf.Clamp(heatValue, 0, 1); // clamp between 0 and 1
                }
            }
            return heatMap;
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