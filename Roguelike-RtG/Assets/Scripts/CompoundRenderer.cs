using UnityEngine;

public class CompoundRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] renderers;
    public SpriteRenderer[] Renderers => renderers;
}