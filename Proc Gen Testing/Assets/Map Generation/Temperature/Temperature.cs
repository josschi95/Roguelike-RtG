public static class Temperature
{
    public static float CelsiusToFarenheit(float degreesCelsius)
    {
        return degreesCelsius * 1.8f + 32;
    }

    public static float FarenheitToCelsius(float degreesFarenheit)
    {
        return (degreesFarenheit - 32) / 1.8f;
    }
}