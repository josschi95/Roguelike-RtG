using JS.EventSystem;
using JS.WorldMap;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    private static GridSystem instance;

    private class GameGrid
    {
        public Grid<GridNode> grid;
        public Vector2Int worldCoordinates; //x,y for WorldPosition
        public Vector2Int regionCoordinates; //x,y within the region
        public int depth = 0; //0 is for surface, levels below ground

        public GameGrid(Vector2Int world,  Vector2Int region, int level)
        {
            worldCoordinates = world;
            regionCoordinates = region;
            depth = level;
        }
    }

    [SerializeField] private WorldGenerationParameters worldGenParams;
    private List<GameGrid> gameGrids;

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
    }

    private Grid<GridNode> worldMapGrid;

    public static void OnEnterNewMap(Vector2Int world, Vector2Int region, int depth)
    {
        instance.OnPlayerEnterNewMap(world, region, depth);
    }

    private void OnPlayerEnterNewMap(Vector2Int world, Vector2Int region, int depth)
    {
        for (int i = 0; i < gameGrids.Count; i++)
        {
            if (gameGrids[i].worldCoordinates != world) continue;
            if (gameGrids[i].regionCoordinates != region) continue;
            if (gameGrids[i].depth != depth) continue;

            LoadMap(gameGrids[i]);
            return;
        }
        //Player is entering a new map, create it
        NewLocalMap(world, region, depth);
    }

    /// <summary>
    /// The player has entered a new Local map, generate it
    /// </summary>
    private GameGrid NewLocalMap(Vector2Int world, Vector2Int region, int depth)
    {
        var newGrid = new GameGrid(world, region, depth);
        var width = worldGenParams.LocalDimensions.x;
        var height = worldGenParams.LocalDimensions.y;
        newGrid.grid = new Grid<GridNode>(width, height, 1, Vector3.zero, (Grid<GridNode> g, int x, int y) => new GridNode(g, x, y));
        
        gameGrids.Add(newGrid);

        //Reset current map, load in values of new map, change player position to appropriate position
        return newGrid;
    }

    /// <summary>
    /// The player has entered a previously entered map, reload it
    /// </summary>
    private void LoadMap(GameGrid grid)
    {
        //Reset current map, load in values of new map, change player position to appropriate position
    }
}
