using UnityEngine;

public static class DampedCosine
{
    public static float[,] GetMoistureMap(float[,] heightMap, float seaLevel)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        var equator = height / 2f;
        float baseValue = 0.5f;

        float[,] moistureMap = new float[width, height];

        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for (int y = 0; y < heightMap.GetLength(1); y++)
            {
                float distanceFromEquator = Mathf.Abs(equator - y);

                float f = distanceFromEquator / equator;
                var moistureValue = baseValue + Mathf.Exp(-f) * 0.5f * Mathf.Cos(3 * Mathf.PI * f);

                if (heightMap[x,y] <= seaLevel) moistureValue = 1;
                else if (heightMap[x, y] <= seaLevel + 0.15f) //water and shore
                {
                    moistureValue += (seaLevel * 2f) - heightMap[x, y];
                }
                moistureValue += Random.Range(0, 0.1f);

                moistureMap[x,y] = Mathf.Clamp(moistureValue, 0, 1);
            }
        }
        return moistureMap;
    }
}