using UnityEngine;

/// <summary>
/// A class that handles the generations of a falloff map to decrease altitude closer to the map's edge.
/// </summary>
public static class FalloffGenerator
{
    private static float a = 3.5f; // determines the slope of the line

    private static float b = 8.5f; // determines the distance from 0 that the line is placed

    private static float minB = 8.5f; // Lower values will drastically diminish landmass
    private static float maxB = 14.0f; // Higher values will have no effect

    public static bool HorizontalFalloff = false; // Apply falloff to left/right sides of map
    public static bool VerticalFalloff = false; // Apply falloff to top/bottom sides of map

    public static float[,] GenerateFalloffMap(int size)
    {
        b = Mathf.Clamp(b, minB, maxB);

        float[,] map = new float[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float i = 0; float j = 0;
                if (HorizontalFalloff) i = x / (float)size * 2 - 1;
                if (HorizontalFalloff) j = y / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));
                map[x, y] = Evaluate(value);
            }
        }
        return map;
    }

    private static float Evaluate(float value)
    {
        // y = x^a / x^a + (b - b * x)^a 
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}