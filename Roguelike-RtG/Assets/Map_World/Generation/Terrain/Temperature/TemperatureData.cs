using UnityEngine;

namespace JS.World.Map.Features
{
    public static class TemperatureData
    {
        public static float[,] GenerateHeatMap(float[,] heightMap, float seaLevel, System.Random rng)
        {
            float[,] heatMap = new float[heightMap.GetLength(0), heightMap.GetLength(1)];

            float equator = heatMap.GetLength(1) / 2f;

            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    float distanceFromEquator = Mathf.Abs(equator - y) / equator;

                    //var heatValue = -Mathf.Pow(distanceFromEquator, 3) + 1;
                    var heatValue = 1 - Mathf.Sin(distanceFromEquator) * distanceFromEquator * 1.4f;

                    if (heightMap[x, y] > seaLevel) heatValue -= Mathf.Abs(heightMap[x, y] - seaLevel);
                    heatValue += rng.Next(-5, 5) * 0.01f;

                    heatMap[x, y] = Mathf.Clamp(heatValue, 0, 1);
                }
            }
            return heatMap;
        }
    }
}