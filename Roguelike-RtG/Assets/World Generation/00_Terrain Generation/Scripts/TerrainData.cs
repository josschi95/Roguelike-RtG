using UnityEngine;
using JS.WorldGeneration;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Terrain Data", menuName = "World Generation/Terrain/Terrain Data")]
public class TerrainData : ScriptableObject
{
    public int mapSize { get; private set; }
    public Vector3Int mapOrigin { get; private set; }

    public float[,] heightMap;
    public float[,] heatMap;

    public MountainRange[] Mountains { get; private set; }
    public Lake[] Lakes { get; private set; }
    public River[] Rivers { get; private set; }
    public BiomeGroup[] Biomes { get; private set; }
    public Island[] Islands { get; private set; }


    [field: SerializeField] public RuleTile RiverTile { get; private set; }

    [field: SerializeField] public RuleTile riverNorth;
    [field: SerializeField] public RuleTile riverEast;
    [field: SerializeField] public RuleTile riverNorthEast;
    [field: SerializeField] public RuleTile riverNorthWest;
    [field: SerializeField] public RuleTile riverSouthEast;
    [field: SerializeField] public RuleTile riverSouthWest;

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

    public void ClearData()
    {
        Mountains = null;
        Lakes = null;
        Rivers = null;
        Biomes = null;
        Islands = null;
    }

    public void SetMapSize(int size, Vector3Int origin)
    {
        mapSize = size;
        mapOrigin = origin;
    }

    public void SetMountains(MountainRange[] mountains)
    {
        Mountains = mountains;
    }

    public void SetIslands(Island[] islands)
    {
        Islands = islands;
    }

    public void SetLakes(Lake[] lakes)
    {
        Lakes = lakes;
    }

    public void SetRivers(River[] rivers)
    {
        Rivers = rivers;
    }

    public void SetBiomes(BiomeGroup[] biomes)
    {
        Biomes = biomes;
    }
}
