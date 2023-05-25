using UnityEngine;

public class LissajousCurve : MonoBehaviour
{
    [SerializeField] private float swayTime;
    public float offset = 0.1f;
    public float duration = 1f;
    public float height = -0.1f;
    bool goLeft;

    private void Update()
    {
        IdleSway();
    }

    private void IdleSway()
    {
        swayTime += Time.deltaTime;
        if (swayTime > 1)
        {
            swayTime = 0;
            goLeft = !goLeft;
        }
        var left = Vector2.zero; 
        var right = Vector2.zero;
        left.x -= offset;
        right.x += offset;

        if (goLeft) transform.localPosition = Parabola.GetParabola(right, left, height, swayTime / 1);
        else transform.localPosition = Parabola.GetParabola(left, right, height, swayTime / 1);
    }
}
