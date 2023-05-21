using UnityEngine;

[CreateAssetMenu(fileName = "River Tile Helper", menuName = "World Generation/Terrain/River Tile Helper")]
public class RiverTileHelper : ScriptableObject
{
    [SerializeField] public RuleTile RiverTile { get; private set; }

    [SerializeField] private RuleTile riverNorth;
    [SerializeField] private RuleTile riverEast;
    [SerializeField] private RuleTile riverNorthEast;
    [SerializeField] private RuleTile riverNorthWest;
    [SerializeField] private RuleTile riverSouthEast;
    [SerializeField] private RuleTile riverSouthWest;

    public RuleTile GetRiverTile(Compass direction)
    {
        switch (direction)
        {
            case Compass.North: return riverNorth;
            case Compass.South: return riverNorth;
            case Compass.East: return riverEast;
            case Compass.West: return riverEast;
            case Compass.NorthEast: return riverNorthEast;
            case Compass.NorthWest: return riverNorthWest;
            case Compass.SouthEast: return riverSouthEast;
            case Compass.SouthWest: return riverSouthWest;
            default: return riverNorth;
        }
    }
}
