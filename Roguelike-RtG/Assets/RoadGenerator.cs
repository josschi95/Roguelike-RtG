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

        public void GenerateRoads()
        {
            SetTravelCosts();
            FindPaths();
        }

        private void FindPaths()
        {
            var roads = new List<Road>();
            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                var newRoad = new Road();
                newRoad.pointA = settlement.ID;

                var end = FindNearest(settlement);
                newRoad.pointB = end.ID;

                var path = worldMap.FindNodePath(settlement.X, settlement.Y, end.X, end.Y);
                if (path == null)
                {
                    Debug.Log("No path found between " + settlement.X + ", " + settlement.Y + " and " + end.X + "," + end.Y);
                    Debug.DrawLine(new Vector3(settlement.X, settlement.Y), new Vector3(end.X, end.Y), Color.red, 1000f);
                    continue;
                }
                BuildRoad(newRoad, path);
                roads.Add(newRoad);
                Debug.Log("Road Successfully built " + settlement.X + ", " + settlement.Y + " and " + end.X + "," + end.Y);
                Debug.DrawLine(new Vector3(settlement.X, settlement.Y), new Vector3(end.X, end.Y), Color.green, 1000f);
            }
            worldMap.TerrainData.Roads = roads.ToArray();
        }

        private Settlement FindNearest(Settlement start)
        {
            float minDist = int.MaxValue;
            Settlement closestSettlement = null;
            foreach(var settlement in worldMap.SettlementData.Settlements)
            {
                if (settlement == start) continue;

                var dist = Vector2.Distance(new Vector2(start.X, start.Y), new Vector2(settlement.X, settlement.Y));
                if (dist < minDist)
                {
                    closestSettlement = settlement;
                    minDist = dist;
                }
            }
            return closestSettlement;
        }

        private void SetTravelCosts()
        {
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var cost = 1;
                    var node = worldMap.GetNode(x, y);
                    if (node.isCoast) cost = 2;

                    var biome = biomeHelper.GetBiome(node.BiomeID);
                    if (biome.isDifficultTerrain) cost = 3;

                    var river = worldMap.TerrainData.FindRiverAt(x, y, out var index);
                    if (river != null)
                    {
                        cost = 10;
                        var flow = river.Nodes[index].Flow;
                        if (flow == Compass.NorthEast || flow == Compass.NorthWest || flow == Compass.SouthEast || flow == Compass.SouthWest)
                            cost = 500;
                    }

                    if (node.BiomeID == biomeHelper.Mountain.ID) cost = 50;
                    if (node.rivers.Count > 1) cost = 500;
                    if (!node.IsLand) cost = 500;

                    node.movementCost = cost;
                }
            }
        }

        private void BuildRoad(Road road, List<WorldTile> list)
        {
            road.Nodes = new RiverNode[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                list[i].movementCost = 0;

                var newNode = new RiverNode();
                road.Nodes[i] = newNode;
                newNode.Coordinates = new GridCoordinates(list[i].x, list[i].y);

                if (i == 0) newNode.Flow = list[i].NeighborDirection_Adjacent(list[i + 1]);
                else if (i == list.Count - 1) newNode.Flow = list[i].NeighborDirection_Adjacent(list[i - 1]);
                else
                {
                    var from = list[i].NeighborDirection_Adjacent(list[i - 1]);
                    var to = list[i].NeighborDirection_Adjacent(list[i + 1]);
                    newNode.Flow = DirectionHelper.CombineDirections(from, to);
                }

                if (list[i].rivers.Count > 0)
                {
                    Debug.LogWarning("Need to generate new bridge.");
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