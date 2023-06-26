using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    //Position
    public Vector3Int World;
    public Vector2Int Region;
    public Vector2Int Local;
}