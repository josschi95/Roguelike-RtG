using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    //Position
    public Vector3Int Position;
    public Vector2Int LocalPosition;
}