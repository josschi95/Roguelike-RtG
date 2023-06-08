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
    public static void OnEnterLocalMap(Vector3Int position)
    {
        instance.EnterLocalMap(position);
    }

    private void EnterLocalMap(Vector3Int position)
    {
        for (int i = 0; i < localGrids.Count; i++)
        {
            if (localGrids[i].Position != position) continue;
            LoadMap(localGrids[i]);
            return;
        }
        //Player is entering a new map, create it
        NewLocalMap(position);
    }

    /// <summary>
    /// The player has entered a previously entered map, reload it
    /// </summary>
    private void LoadMap(GameGrid grid)
    {
        activeLocalGrid = grid;
        activeMapWorldPosition.Value = grid.Position;
        Debug.Log("Loading Map. World: " + activeLocalGrid.Position);

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
    private void NewLocalMap(Vector3Int position)
    {
        var width = worldGenParams.LocalDimensions.x;
        var height = worldGenParams.LocalDimensions.y;
        var newGrid = new GameGrid(position, width, height);
        
        localGrids.Add(newGrid);
        SpawnMonsters(newGrid);
        //Reset current map, load in values of new map, change player position to appropriate position
        LoadMap(newGrid);
    }

    public static GameGrid GetGrid(Vector3Int position)
    {
        for (int i = 0; i < instance.localGrids.Count; i++)
        {
            if (instance.localGrids[i].Position == position) 
                return instance.localGrids[i];
        }
        return null;
    }

    private void SpawnMonsters(GameGrid grid)
    {
        int count = Random.Range(2, 5);
        for (int i = 0; i < count; i++)
        {
            var enemy = EntityFactory.GetEntity("BaseCreature");
            enemy.Name = "Orc_0" + i;

            var physics = enemy.GetComponent<JS.ECS.Physics>();
            enemy.GetComponent<Brain>().IsSleeping = false;

            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Orc");
            physics.Position = grid.Position;
            physics.LocalPosition = new Vector2Int(Random.Range(35, 65), Random.Range(35, 65));
            enemy.AddComponent(new RenderSingle(physics, sprites[i]));
        }
    }
}

public class GameGrid
{
    public Grid<GridNode> Grid;
    public Vector3Int Position;

    public GameGrid(Vector3Int position, int width, int height)
    {
        Position = position;
        
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