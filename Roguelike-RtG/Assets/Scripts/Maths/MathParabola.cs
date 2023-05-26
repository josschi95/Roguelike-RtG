using System;
using UnityEngine;

public static class MathParabola
{
    public static Vector3 GetParabola(Vector3 start, Vector3 end, float height, float time)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, time);

        return new Vector3(mid.x, f(time) + Mathf.Lerp(start.y, end.y, time), mid.z);
    }

    public static Vector2 GetParabola(Vector2 start,  Vector2 end, float height, float time)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, time);

        return new Vector2(mid.x, f(time) + Mathf.Lerp(start.y, end.y, time));
    }
}