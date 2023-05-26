using UnityEngine;

public static class LissajousCurve
{
    public static Vector3 GetCurve(float magnitude, float cycles, float time)
    {
        return new Vector3(Mathf.Sin(time), magnitude * Mathf.Sin(cycles * time + Mathf.PI));
    }
}