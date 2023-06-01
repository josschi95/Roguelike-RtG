using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Direction Tiles")]
public class DirectionTiles : ScriptableObject
{
    [SerializeField] private RuleTile[] tiles;

    public RuleTile GetTile(Compass compass)
    {
        return tiles[(int)compass];
    }
}
