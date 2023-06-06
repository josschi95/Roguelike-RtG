using UnityEngine;

public class IdleSway : MonoBehaviour
{
    [SerializeField] private float swayTime;
    [SerializeField] private float offset = 0.03f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float height = -0.04f;
    private Vector3 pointA = new Vector3(-0.03f, 0, 0);
    private Vector3 pointB = new Vector3(0.03f, 0, 0);
    private bool goLeft;

    private void Start()
    {
        //Add some variance in sways
        swayTime = Random.Range(0, duration);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        swayTime += Time.deltaTime;
        if (swayTime >= duration)
        {
            swayTime = 0;
            goLeft = !goLeft;
        }
        var left = Vector2.zero; 
        var right = Vector2.zero;
        left.x -= offset;
        right.x += offset;

        if (goLeft) transform.localPosition = MathParabola.GetParabola(right, left, height, swayTime / duration);
        else transform.localPosition = MathParabola.GetParabola(left, right, height, swayTime / duration);
    }

    private void MoveObject()
    {
        swayTime += Time.deltaTime;
        if (swayTime >= duration)
        {
            swayTime = 0;
            goLeft = !goLeft;
        }
        if (goLeft) transform.localPosition = MathParabola.GetParabola(pointA, pointB, height, swayTime / duration);
        else transform.localPosition = MathParabola.GetParabola(pointB, pointA, height, swayTime / duration);
    }
}
