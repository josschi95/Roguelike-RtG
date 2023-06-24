using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "River Tile Helper", menuName = "World Generation/Terrain/River Tile Helper")]
    public class PathTileHelper : ScriptableObject
    {
        [SerializeField] public RuleTile RiverTile { get; private set; }

        [SerializeField] private RuleTile pathVertical;
        [SerializeField] private RuleTile pathHorizontal;

        [Space]

        [SerializeField] private RuleTile pathNorthEast;
        [SerializeField] private RuleTile pathNorthWest;
        [SerializeField] private RuleTile pathSouthEast;
        [SerializeField] private RuleTile pathSouthWest;

        [Space]

        [SerializeField] private RuleTile pathT_North;
        [SerializeField] private RuleTile pathT_South;
        [SerializeField] private RuleTile pathT_East;
        [SerializeField] private RuleTile pathT_West;

        [Space]

        [SerializeField] private RuleTile pathIntersection;

        public RuleTile GetRiverTile(Compass direction)
        {
            switch (direction)
            {
                case Compass.North: return pathVertical;
                case Compass.South: return pathVertical;
                case Compass.East: return pathHorizontal;
                case Compass.West: return pathHorizontal;
                case Compass.NorthEast: return pathNorthEast;
                case Compass.NorthWest: return pathNorthWest;
                case Compass.SouthEast: return pathSouthEast;
                case Compass.SouthWest: return pathSouthWest;
                default: return pathVertical;
            }
        }

        public RuleTile GetIntersectionTile(RuleTile tile, Compass dir)
        {
            if (tile == null) return GetRiverTile(dir);
            if (tile == pathVertical)
            {
                if (dir == Compass.North || dir == Compass.South) return pathVertical;
                if (dir == Compass.East || dir == Compass.West) return pathIntersection;
                if (dir == Compass.NorthEast || dir == Compass.SouthEast) return pathT_East;
                if (dir == Compass.SouthEast || dir == Compass.SouthWest) return pathT_West;
            }
            else if (tile == pathHorizontal)
            {
                if (dir == Compass.North || dir == Compass.South) return pathIntersection;
                if (dir == Compass.East || dir == Compass.West) return pathHorizontal;
                if (dir == Compass.NorthEast || dir == Compass.NorthWest) return pathT_North;
                if (dir == Compass.SouthEast || dir == Compass.SouthWest) return pathT_South;
            }
            else if (tile == pathNorthEast)
            {
                if (dir == Compass.East || dir == Compass.West) return pathT_North;
                if (dir == Compass.North || dir == Compass.South) return pathT_East;
                if (dir == Compass.SouthEast) return pathT_East;
            }
            else if (tile == pathNorthWest)
            {
                if (dir == Compass.SouthWest) return pathT_West;
                if (dir == Compass.West || dir == Compass.East) return pathT_North;
                if (dir == Compass.North || dir == Compass.South) return pathT_West;
                if (dir == Compass.NorthEast) return pathT_North;
            }
            else if (tile == pathSouthEast)
            {
                if (dir == Compass.North || dir == Compass.NorthEast) return pathT_East;
                if (dir == Compass.East || dir == Compass.SouthWest) return pathT_South;
            }
            else if (tile == pathSouthWest)
            {
                if (dir == Compass.East || dir == Compass.West) return pathT_South;
                if (dir == Compass.North || dir == Compass.NorthWest) return pathT_West;
            }

            if (tile == pathT_North || tile == pathT_South ||
                tile == pathT_East || tile == pathT_West) return pathIntersection;

            //Debug.Log(tile.m_DefaultSprite.name + " from " + dir.ToString());
            //Debug.LogWarning("Returning default.");
            return GetRiverTile(dir);
        }
    }
}