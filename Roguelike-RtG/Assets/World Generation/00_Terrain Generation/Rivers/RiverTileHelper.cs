using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "River Tile Helper", menuName = "World Generation/Terrain/River Tile Helper")]
    public class RiverTileHelper : ScriptableObject
    {
        [SerializeField] public RuleTile RiverTile { get; private set; }

        [SerializeField] private RuleTile riverVertical;
        [SerializeField] private RuleTile riverHorizontal;

        [Space]

        [SerializeField] private RuleTile riverNorthEast;
        [SerializeField] private RuleTile riverNorthWest;
        [SerializeField] private RuleTile riverSouthEast;
        [SerializeField] private RuleTile riverSouthWest;

        [Space]

        [SerializeField] private RuleTile riverT_North;
        [SerializeField] private RuleTile riverT_South;
        [SerializeField] private RuleTile riverT_East;
        [SerializeField] private RuleTile riverT_West;

        [Space]

        [SerializeField] private RuleTile riverIntersection;

        public RuleTile GetRiverTile(Compass direction)
        {
            switch (direction)
            {
                case Compass.North: return riverVertical;
                case Compass.South: return riverVertical;
                case Compass.East: return riverHorizontal;
                case Compass.West: return riverHorizontal;
                case Compass.NorthEast: return riverNorthEast;
                case Compass.NorthWest: return riverNorthWest;
                case Compass.SouthEast: return riverSouthEast;
                case Compass.SouthWest: return riverSouthWest;
                default: return riverVertical;
            }
        }

        public RuleTile GetIntersectionTile(RuleTile tile, Compass dir)
        {
            if (tile == null) return GetRiverTile(dir);
            if (tile == riverVertical)
            {
                if (dir == Compass.North || dir == Compass.South) return riverVertical;
                if (dir == Compass.East || dir == Compass.West) return riverIntersection;
                if (dir == Compass.NorthEast || dir == Compass.SouthEast) return riverT_East;
                if (dir == Compass.SouthEast || dir == Compass.SouthWest) return riverT_West;
            }
            else if (tile == riverHorizontal)
            {
                if (dir == Compass.North || dir == Compass.South) return riverIntersection;
                if (dir == Compass.East || dir == Compass.West) return riverHorizontal;
                if (dir == Compass.NorthEast || dir == Compass.NorthWest) return riverT_North;
                if (dir == Compass.SouthEast || dir == Compass.SouthWest) return riverT_South;
            }
            else if (tile == riverNorthEast)
            {
                if (dir == Compass.East || dir == Compass.West) return riverT_North;
                if (dir == Compass.North || dir == Compass.South) return riverT_East;
                if (dir == Compass.SouthEast) return riverT_East;
            }
            else if (tile == riverNorthWest)
            {
                if (dir == Compass.SouthWest) return riverT_West;
                if (dir == Compass.West || dir == Compass.East) return riverT_North;

                if (dir == Compass.NorthEast) return riverT_North;
            }
            else if (tile == riverSouthEast)
            {
                if (dir == Compass.North || dir == Compass.NorthEast) return riverT_East;
                if (dir == Compass.East || dir == Compass.SouthWest) return riverT_South;
            }
            else if (tile == riverSouthWest)
            {
                if (dir == Compass.East || dir == Compass.West) return riverT_South;
                if (dir == Compass.North || dir == Compass.NorthWest) return riverT_West;
            }

            if (tile == riverT_North || tile == riverT_South ||
                tile == riverT_East || tile == riverT_West) return riverIntersection;

            Debug.Log(tile.m_DefaultSprite.name + " from " + dir.ToString());
            Debug.LogWarning("Returning default.");
            return GetRiverTile(dir);
        }
    }
}