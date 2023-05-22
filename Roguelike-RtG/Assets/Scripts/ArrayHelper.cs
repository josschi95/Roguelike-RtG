
public static class ArrayHelper
{
    public static int Convert2DCoordinateTo1DCoordinate(int width, int height, int x, int y)
    {
        int index = x * width + y;
        return x * width + y;
    }

    public static void Convert1DCoordinateTo2DCoordinate(int width, int height, int index, out int x, out int y)
    {
        x = index / width;
        y = index % width;
    }

    public static float[] Convert2DFloatArrayTo1D(float[,] squareArray)
    {
        int width = squareArray.GetLength(0);
        int height = squareArray.GetLength(1);

        var linearMap = new float[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                linearMap[x * width + y] = squareArray[x, y];
            }
        }
        return linearMap;
    }

    public static float[,] Convert1DFloatArrayTo2D(float[] linearArray, int width, int height)
    {
        var squareArray = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                squareArray[x, y] = linearArray[x * width + y];
            }
        }
        return squareArray;
    }

    public static int[] Convert2DIntArrayTo1D(int[,] squareArray)
    {
        int width = squareArray.GetLength(0);
        int height = squareArray.GetLength(1);

        var linearMap = new int[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                linearMap[x * width + y] = squareArray[x, y];
            }
        }
        return linearMap;
    }

    public static int[,] Convert1DIntArrayTo2D(int[] linearArray, int width, int height)
    {
        var squareArray = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                squareArray[x, y] = linearArray[x * width + y];
            }
        }
        return squareArray;
    }
}