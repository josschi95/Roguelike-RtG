using UnityEngine;

public class CompoundRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer baseSprite;

    [SerializeField] private SpriteRenderer[] renderers;

    public SpriteRenderer Base => baseSprite;
    public SpriteRenderer[] Renderers => renderers;
}