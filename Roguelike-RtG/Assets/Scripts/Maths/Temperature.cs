public static class Temperature
{
    /// <summary>
    /// Converts temperature between 0 and 1 to Farenheit
    /// </summary>
    public static float FloatToFarenheit(float value)
    {
        return UnityEngine.Mathf.Clamp(value - 0.1f, 0, 1) * 100;
    }

    /// <summary>
    /// Converts temperature between 0 and 1 to Celsius
    /// </summary>
    public static float FloatToCelsius(float value)
    {
        return FarenheitToCelsius(FloatToFarenheit(value));
    }

    public static float CelsiusToFarenheit(float degreesCelsius)
    {
        return degreesCelsius * 1.8f + 32;
    }

    public static float FarenheitToCelsius(float degreesFarenheit)
    {
        return (degreesFarenheit - 32) / 1.8f;
    }
}