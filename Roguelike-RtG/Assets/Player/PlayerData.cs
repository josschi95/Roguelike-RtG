using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private Vector3Int _worldPosition;
    [SerializeField] private Vector2Int _regionPosition;
    [SerializeField] private Vector2Int _localPosition;

    public Vector3Int World
    {
        get => _worldPosition;
        set => _worldPosition = value;
    }
    public Vector2Int Region
    {
        get => _regionPosition;
        set => _regionPosition = value;
    }
    public Vector2Int Local
    {
        get => _localPosition;
        set => _localPosition = value;
    }

}