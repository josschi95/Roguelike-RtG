using UnityEngine;
using JS.WorldGeneration;

[CreateAssetMenu(fileName = "Terrain Data", menuName = "World Generation/Terrain/Terrain Data")]
public class TerrainData : ScriptableObject
{
    public int mapSize { get; private set; }
    public Vector3Int mapOrigin { get; private set; }

    public float[,] heightMap;
    public float[,] heatMap;

    [SerializeField] private MountainRange[] mountains;
    [SerializeField] private Lake[] lakes;
    [SerializeField] private River[] rivers;
    [SerializeField] private BiomeGroup[] biomes;
    [SerializeField] private Island[] islands;

    public MountainRange[] Mountains => mountains;
    public Lake[] Lakes => lakes;
    public River[] Rivers => rivers;
    public BiomeGroup[] Biomes => biomes;
    public Island[] Islands => islands;

    public void ClearData()
    {
        mountains = null;
        lakes = null;
        rivers = null;
        biomes = null;
        islands = null;
    }

    public void SetMapSize(int size, Vector3Int origin)
    {
        mapSize = size;
        mapOrigin = origin;
    }

    public void SetMountains(MountainRange[] mountains)
    {
        this.mountains = mountains;
    }

    public void SetIslands(Island[] islands)
    {
        this.islands = islands;
    }

    public void SetLakes(Lake[] lakes)
    {
        this.lakes = lakes;
    }

    public void SetRivers(River[] rivers)
    {
        this.rivers = rivers;
    }

    public void SetBiomes(BiomeGroup[] biomes)
    {
        this.biomes = biomes;
    }
}
