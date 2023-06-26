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

    private bool worldMapActive;
    public static bool WorldMapActive => instance.worldMapActive;

    private GameGrid activeLocalGrid;
    public static GameGrid ActiveGrid => instance.activeLocalGrid;
    private List<GameGrid> localGrids;

    [Header("Game Events")]
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
    }

    /// <summary>
    /// Player switched view to World Map.
    /// </summary>
    public static void OnSwitchToWorldView()
    {
        instance.SwitchToWorldView();
    }

    private void SwitchToWorldView()
    {
        worldMapActive = true;
        localToWorldMapEvent?.Invoke(); //loads world map scene
    }

    private void SwitchToLocalView()
    {
        worldMapActive = false;
        worldToLocalMapEvent?.Invoke(); //loads local map scene
    }

    /// <summary>
    /// Player moved to new local map.
    /// </summary>
    public static void OnEnterLocalMap(Vector3Int world, Vector2Int region)
    {
        instance.EnterLocalMap(world, region);
    }

    private void EnterLocalMap(Vector3Int world, Vector2Int region)
    {
        for (int i = 0; i < localGrids.Count; i++)
        {
            if (localGrids[i].World != world) continue;
            if (localGrids[i].Region != region) continue;
            LoadMap(localGrids[i]);
            return;
        }
        //Player is entering a new map, create it
        NewLocalMap(world, region);
    }

    /// <summary>
    /// The player has entered a previously entered map, reload it
    /// </summary>
    private void LoadMap(GameGrid grid)
    {
        activeLocalGrid = grid;
        activeMapWorldPosition.Value = grid.World;
        //Debug.Log("Loading Map. World: " + activeLocalGrid.Position);

        if (worldMapActive) SwitchToLocalView();
        else
        {
            localToLocalMapEvent?.Invoke();
            RenderSystem.OnNewSceneLoaded();
        }
    }

    /// <summary>
    /// The player has entered a new Local map, generate it
    /// </summary>
    private void NewLocalMap(Vector3Int world, Vector2Int region)
    {
        var width = worldGenParams.LocalDimensions.x;
        var height = worldGenParams.LocalDimensions.y;
        var newGrid = new GameGrid(world, region, width, height);
        
        localGrids.Add(newGrid);
        SpawnMonsters(newGrid);
        SpawnItems(newGrid);
        //Reset current map, load in values of new map, change player position to appropriate position
        LoadMap(newGrid);
    }

    public static GameGrid GetGrid(JS.ECS.Transform t)
    {
        for (int i = 0; i < instance.localGrids.Count; i++)
        {
            if (instance.localGrids[i].World != t.WorldPosition) continue;
            if (instance.localGrids[i].Region != t.RegionPosition) continue;
            return instance.localGrids[i];
        }
        return null;
    }

    public static GameGrid GetGrid(Vector3Int world, Vector2Int region)
    {
        foreach(var grid in instance.localGrids)
        {
            if (grid.World != world || grid.Region != region) continue;
            return grid;
        }
        return null;
    }

    private void SpawnMonsters(GameGrid grid)
    {
        int count = Random.Range(3, 5);
        for (int i = 0; i < count; i++)
        {
            var enemy = EntityFactory.GetEntity("Orc");
            enemy.Name = "Orc_0" + i;

            EntityManager.GetComponent<Brain>(enemy).IsHibernating = false;

            var transform = EntityManager.GetComponent<JS.ECS.Transform>(enemy);
            var local = new Vector2Int(Random.Range(45, 55), Random.Range(45, 55));
            TransformSystem.SetPosition(transform, grid.World, grid.Region, local);
        }
    }

    private void SpawnItems(GameGrid grid)
    {
        var sword = EntityFactory.GetEntity("IronSword");
        var a = EntityManager.GetComponent<JS.ECS.Transform>(sword);
        TransformSystem.SetPosition(a, grid.World, grid.Region, new Vector2Int(50, 50));

        var shield = EntityFactory.GetEntity("WoodenShield");
        var b = EntityManager.GetComponent<JS.ECS.Transform>(shield);
        TransformSystem.SetPosition(b, grid.World, grid.Region, new Vector2Int(51, 50));

        var armor = EntityFactory.GetEntity("LeatherArmor");
        var c = EntityManager.GetComponent<JS.ECS.Transform>(armor);
        TransformSystem.SetPosition(c, grid.World, grid.Region, new Vector2Int(52, 50));

        var boots = EntityFactory.GetEntity("LeatherBoots");
        var d = EntityManager.GetComponent<JS.ECS.Transform>(boots);
        TransformSystem.SetPosition(d, grid.World, grid.Region, new Vector2Int(53, 50));
    }
}

public class GameGrid
{
    public Grid<GridNode> Grid;
    public Vector3Int World;
    public Vector2Int Region;

    public GameGrid(Vector3Int world, Vector2Int region, int width, int height)
    {
        World = world;
        Region = region;

        Grid = new Grid<GridNode>(width, height, 1, Vector3.zero, (Grid<GridNode> g, int x, int y) => new GridNode(g, x, y));

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                Grid.GetGridObject(x, y).GetNeighbors();
            }
        }
    }
}