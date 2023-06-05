using JS.ECS;
using JS.EventSystem;
using JS.Primitives;
using JS.WorldMap;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;

    [SerializeField] private WorldGenerationParameters worldGenParams;
    [SerializeField] private Vector3IntVariable activeMapWorldPosition;
    [SerializeField] private Vector3IntVariable activeMapRegionPosition;

    private bool worldMapActive;
    public static bool WorldMapActive => instance.worldMapActive;

    private GameGrid activeLocalGrid;
    private List<GameGrid> localGrids;

    public static GameGrid ActiveGrid => instance.activeLocalGrid;

    [Space]

    [SerializeField] private GameEvent worldToLocalMapEvent;
    [SerializeField] private GameEvent localToLocalMapEvent;
    [SerializeField] private GameEvent localToWorldMapEvent;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        localGrids = new List<GameGrid>();

        worldMapActive = true;
        activeMapWorldPosition.Value.z = 1;
        activeMapRegionPosition.Value = Vector3Int.one;
    }

    public static void OnSwitchToWorldMap()
    {
        instance.SwitchLocalToWorld();
    }

    private void SwitchLocalToWorld()
    {
        worldMapActive = true;
        activeMapWorldPosition.Value.z = 1;
        activeMapRegionPosition.Value = Vector3Int.one;

        localToWorldMapEvent?.Invoke(); //loads world map scene
    }

    private void SwitchWorldToLocal()
    {
        worldMapActive = false;
        worldToLocalMapEvent?.Invoke(); //loads local map scene
    }

    /// <summary>
    /// Player moved to new local map.
    /// </summary>
    public static void OnEnterLocalMap(Vector2Int world, Vector2Int region, int depth)
    {
        instance.EnterLocalMap(world, region, depth);
    }

    private void EnterLocalMap(Vector2Int world, Vector2Int region, int depth)
    {
        for (int i = 0; i < localGrids.Count; i++)
        {
            if (localGrids[i].WorldCoordinates != world) continue;
            if (localGrids[i].RegionCoordinates != region) continue;
            if (localGrids[i].Grid.Depth != depth) continue;

            LoadMap(localGrids[i]);
            return;
        }
        //Player is entering a new map, create it
        NewLocalMap(world, region, depth);
    }

    /// <summary>
    /// The player has entered a previously entered map, reload it
    /// </summary>
    private void LoadMap(GameGrid grid)
    {
        activeLocalGrid = grid;
        activeMapWorldPosition.Value = (Vector3Int)grid.WorldCoordinates;
        activeMapWorldPosition.Value.z = grid.Grid.Depth;
        activeMapRegionPosition.Value = (Vector3Int)grid.RegionCoordinates;
        Debug.Log("Loading Map. World: " +  activeLocalGrid.WorldCoordinates + ", Region: " + activeLocalGrid.RegionCoordinates);
        if (worldMapActive) SwitchWorldToLocal();
        else localToLocalMapEvent?.Invoke();
        RenderSystem.OnNewSceneLoaded();
    }

    /// <summary>
    /// The player has entered a new Local map, generate it
    /// </summary>
    private void NewLocalMap(Vector2Int world, Vector2Int region, int depth)
    {
        var newGrid = new GameGrid(world, region);

        var width = worldGenParams.LocalDimensions.x;
        var height = worldGenParams.LocalDimensions.y;
        newGrid.Grid = new Grid<GridNode>(width, height, depth, 1, Vector3.zero, (Grid<GridNode> g, int x, int y) => new GridNode(g, x, y));
        for (int x = 0; x < newGrid.Grid.Width; x++)
        {
            for (int y = 0; y < newGrid.Grid.Height; y++)
            {
                newGrid.Grid.GetGridObject(x, y).GetNeighbors();
            }
        }

        localGrids.Add(newGrid);
        SpawnMonsters(newGrid);
        //Reset current map, load in values of new map, change player position to appropriate position
        LoadMap(newGrid);
    }

    private void SpawnMonsters(GameGrid grid)
    {
        int count = Random.Range(2, 5);
        for (int i = 0; i < count; i++)
        {
            var enemy = Blueprints.GetCreature("Orc_0" + i);
            var t = enemy.GetComponent<JS.ECS.Transform>();
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Orc");
            enemy.AddComponent(new RenderSingle(t, sprites[i]));
            t.WorldPosition = grid.WorldCoordinates;
            t.RegionPosition = grid.RegionCoordinates;
            t.Depth = grid.Grid.Depth;
            t.LocalPosition = new Vector2Int(Random.Range(35, 65), Random.Range(35, 65));
        }
    }
}

public class GameGrid
{
    public Grid<GridNode> Grid;
    //So I could just hold these values within the grid itself...
    public Vector2Int WorldCoordinates; //x,y for WorldPosition
    public Vector2Int RegionCoordinates; //x,y within the region

    public GameGrid(Vector2Int world, Vector2Int region)
    {
        WorldCoordinates = world;
        RegionCoordinates = region;
    }
}