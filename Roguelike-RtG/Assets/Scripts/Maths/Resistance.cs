using UnityEngine;

public class Resistance
{
    public static float GetModifier(int value)
    {
        return 1 - value * 0.01f;
    }
}