using JS.WorldMap;
using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldMap.Generation
{
    public class RoadGenerator : MonoBehaviour
    {
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private WorldGenerationParameters mapFeatures;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private BiomeHelper biomeHelper;
        private List<Bridge> bridges;
        private List<Road> roads;

        public void GenerateRoads()
        {
            SetTravelCosts();
            FindPaths();
        }

        private void SetTravelCosts()
        {
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var cost = 2;
                    var node = worldMap.GetNode(x, y);
                    if (worldMap.TerrainData.Coasts[x, y]) cost = 3;

                    var biome = biomeHelper.GetBiome(node.BiomeID);
                    if (biome.isDifficultTerrain) cost = 4;

                    var river = worldMap.TerrainData.FindRiverAt(x, y, out var index);
                    if (river != null)
                    {
                        cost = 15;
                        var flow = river.Nodes[index].PathDirection;
                        if (flow == Compass.NorthEast || flow == Compass.NorthWest || flow == Compass.SouthEast || flow == Compass.SouthWest)
                            cost = 500;
                    }

                    if (node.BiomeID == biomeHelper.Mountain.ID) cost = 100;
                    if (node.Rivers.Count > 1) cost = 500;
                    if (!node.IsLand) cost = 500;

                    node.movementCost = cost;
                }
            }
        }

        private void FindPaths()
        {
            roads = new List<Road>();
            bridges = new List<Bridge>();

            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                var newRoad = new Road();
                newRoad.pointA = settlement.ID;

                var end = FindNearest(settlement);
                if (end == null)
                {
                    Debug.LogWarning("Couldn't find end");
                    continue;
                }
                newRoad.pointB = end.ID;

                var path = worldMap.FindNodePath(settlement.X, settlement.Y, end.X, end.Y);
                if (path == null)
                {
                    //Debug.Log("No path found between " + settlement.X + ", " + settlement.Y + " and " + end.X + "," + end.Y);
                    //Debug.DrawLine(new Vector3(settlement.X, settlement.Y), new Vector3(end.X, end.Y), Color.red, 1000f);
                    continue;
                }
                BuildRoad(newRoad, path);
                roads.Add(newRoad);
                //Debug.Log("Road Successfully built " + settlement.X + ", " + settlement.Y + " and " + end.X + "," + end.Y);
                //Debug.DrawLine(new Vector3(settlement.X, settlement.Y), new Vector3(end.X, end.Y), Color.green, 1000f);
            }

            worldMap.TerrainData.Roads = roads.ToArray();
            worldMap.TerrainData.Bridges = bridges.ToArray();
        }

        private Settlement FindNearest(Settlement start)
        {
            float minDist = int.MaxValue;
            Settlement closestSettlement = null;

            foreach(var settlement in worldMap.SettlementData.Settlements)
            {
                if (settlement == start) continue;
                if (IsDuplicateRoad(start, settlement)) continue;

                var dist = Vector2.Distance(new Vector2(start.X, start.Y), new Vector2(settlement.X, settlement.Y));
                if (dist < minDist)
                {
                    closestSettlement = settlement;
                    minDist = dist;
                }
            }
            return closestSettlement;
        }

        private bool IsDuplicateRoad(Settlement start, Settlement end)
        {
            for (int i = 0; i < roads.Count; i++)
            {
                if (roads[i].pointA == start.ID || roads[i].pointB == start.ID)
                {
                    if (roads[i].pointA == end.ID || roads[i].pointB == end.ID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void BuildRoad(Road road, List<WorldTile> list)
        {
            road.Nodes = new RiverNode[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                list[i].movementCost = 0;

                var newNode = new RiverNode(list[i].x, list[i].y);
                road.Nodes[i] = newNode;

                if (i == 0) newNode.PathDirection = list[i].NeighborDirection_Adjacent(list[i + 1]);
                else if (i == list.Count - 1) newNode.PathDirection = list[i].NeighborDirection_Adjacent(list[i - 1]);
                else
                {
                    var from = list[i].NeighborDirection_Adjacent(list[i - 1]);
                    var to = list[i].NeighborDirection_Adjacent(list[i + 1]);
                    newNode.PathDirection = DirectionHelper.CombineDirections(from, to);
                }

                if (list[i].Rivers.Count > 0)
                {
                    //Debug.LogWarning("Need to generate new bridge.");
                    var bridge = new Bridge();
                    bridge.x = list[i].x; 
                    bridge.y = list[i].y;
                    if (newNode.PathDirection == Compass.North || newNode.PathDirection == Compass.South)
                        bridge.isVertical = true;
                    
                    bridges.Add(bridge);
                }
            }
        }
    }
}

[System.Serializable]
public class Road
{
    public int pointA;
    public int pointB;
    public RiverNode[] Nodes;
}

[System.Serializable]
public class Bridge
{
    public int x, y;
    public bool isVertical;
}