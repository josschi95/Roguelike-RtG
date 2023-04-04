using UnityEngine;

public static class DiamondSquare
{
    //if n == 7, mapSize = 128
    //if n == 8, mapSize = 256, ~minute map
    //if n == 9, mapSize = 512 ~ tiny map
    //if n == 10, mapSize = 1024 ~ small map
    //if n == 11, mapSize = 2024 ~ ~ large map

    public static float[,] GetHeightMap(int size, float roughness = 0.8f)
    {
        float[,] heightMap = new float[size, size];
        float average, range = 0.25f;
        int sideLength, halfSide, x, y;

        //Set the four corners to random values
        heightMap[0, 0] = Random.value;
        heightMap[0, size - 1] = Random.value;
        heightMap[size - 1, 0] = Random.value;
        heightMap[size - 1, size - 1] = Random.value;

        for (sideLength = size - 1; sideLength > 1; sideLength /= 2)
        {
            halfSide = sideLength / 2;

            // Run Diamond Step
            for (x = 0; x < size - 1; x += sideLength)
            {
                for (y = 0; y < size - 1; y += sideLength)
                {
                    // Get the average of the corners
                    average = heightMap[x, y];
                    average += heightMap[x + sideLength, y];
                    average += heightMap[x, y + sideLength];
                    average += heightMap[x + sideLength, y + sideLength];
                    average /= 4.0f;

                    // Offset by a random value
                    //average += (Random.value * (range * 2.0f)) - range;
                    average += Random.Range(-range, range);

                    heightMap[x + halfSide, y + halfSide] = Mathf.Clamp(average, 0, 1);
                    if (average < 0) Debug.LogWarning("Clamping height value to 0");
                    else if (average > 1) Debug.LogWarning("Clamping height value to 1");
                }
            }

            // Run Square Step
            for (x = 0; x < size - 1; x += halfSide)
            {
                for (y = (x + halfSide) % sideLength; y < size - 1; y += sideLength)
                {
                    // Get the average of the corners
                    average = heightMap[(x - halfSide + size - 1) % (size - 1), y];
                    average += heightMap[(x + halfSide) % (size - 1), y];
                    average += heightMap[x, (y + halfSide) % (size - 1)];
                    average += heightMap[x, (y - halfSide + size - 1) % (size - 1)];
                    average /= 4.0f;

                    // Offset by a random value
                    //average += (Random.value * (range * 2.0f)) - range;
                    average += Random.Range(-range, range);


                    // Set the height value to be the calculated average
                    if (average < 0) Debug.LogWarning("Clamping height value to 0");
                    else if (average > 1) Debug.LogWarning("Clamping height value to 1");
                    heightMap[x, y] = Mathf.Clamp(average, 0, 1);

                    // Set the height on the opposite edge if this is
                    // an edge piece
                    if (x == 0) heightMap[size - 1, y] = average;

                    if (y == 0) heightMap[x, size - 1] = average;
                }
            }

            // Lower the random value range
            range -= range * 0.5f * roughness;
        }

        return heightMap;
    }
}