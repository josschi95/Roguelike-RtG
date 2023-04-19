using UnityEngine;
using JS.WorldGeneration;

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
