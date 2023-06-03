using JS.EventSystem;
using JS.WorldMap;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;

    public class GameGrid
    {
        public Grid<GridNode> Grid;
        //So I could just hold these values within the grid itself...
        public Vector2Int WorldCoordinates; //x,y for WorldPosition
        public Vector2Int RegionCoordinates; //x,y within the region

        public GameGrid(Vector2Int world,  Vector2Int region)
        {
            WorldCoordinates = world;
            RegionCoordinates = region;
        }
    }

    [SerializeField] private WorldGenerationParameters worldGenParams;

    private bool worldMapActive;
    public static bool WorldMapActive => instance.worldMapActive;

    private GameGrid activeGrid;
    private List<GameGrid> gameGrids;

    public static GameGrid ActiveGrid => instance.activeGrid;

    [Space]
    [SerializeField] private GameEvent worldToLocalGenerationEvent;
    public bool waitingForLocalGenerationToLoad { get; set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        gameGrids = new List<GameGrid>();

        worldMapActive = true;
    }

    //Called from GameEventListener on WorldMap scene loaded
    public void OnSwitchToWorldMap() => worldMapActive = true;

    //Called from GameEventListener on LocalMap scene loaded
    public void OnSwitchToLocalMap() => worldMapActive = false;

    public static void OnEnterNewMap(Vector2Int world, Vector2Int region, int depth)
    {
        instance.OnPlayerEnterNewMap(world, region, depth);
    }

    private void OnPlayerEnterNewMap(Vector2Int world, Vector2Int region, int depth)
    {
        for (int i = 0; i < gameGrids.Count; i++)
        {
            if (gameGrids[i].WorldCoordinates != world) continue;
            if (gameGrids[i].RegionCoordinates != region) continue;
            if (gameGrids[i].Grid.Depth != depth) continue;

            LoadMap(gameGrids[i]);
            return;
        }
        //Player is entering a new map, create it
        NewLocalMap(world, region, depth);
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
        
        gameGrids.Add(newGrid);

        //Reset current map, load in values of new map, change player position to appropriate position
        LoadMap(newGrid);
    }

    /// <summary>
    /// The player has entered a previously entered map, reload it
    /// </summary>
    private void LoadMap(GameGrid grid)
    {
        activeGrid = grid;
        //Reset current map, load in values of new map, change player position to appropriate position
    }
}
