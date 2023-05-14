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
}