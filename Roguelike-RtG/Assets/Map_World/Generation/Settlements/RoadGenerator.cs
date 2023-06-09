using JS.WorldMap;
using System.Collections.Generic;
using UnityEngine;
using DelaunayVoronoi;

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
            SetMovementPenalties();
            ConstructRoads();
        }

        private void SetMovementPenalties()
        {
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var cost = 3;
                    var node = worldMap.GetNode(x, y);
                    if (worldMap.TerrainData.Coasts[x, y]) cost = 5;

                    var biome = biomeHelper.GetBiome(node.BiomeID);
                    if (biome.isDifficultTerrain) cost = 7;

                    var river = worldMap.TerrainData.FindRiverAt(x, y, out var index);
                    if (river != null)
                    {
                        cost = 15;
                        var flow = river.Nodes[index].PathDirection;
                        if (flow == Compass.NorthEast || flow == Compass.NorthWest || flow == Compass.SouthEast || flow == Compass.SouthWest)
                            cost = 500;
                    }

                    if (node.BiomeID == biomeHelper.Mountain.ID) cost = 100;
                    if (!node.IsLand) cost = 500;

                    node.movementCost = cost;
                }
            }
        }

        private void ConstructRoads()
        {
            //look around node 66,175 with seed 10, Small map
            //Debug.LogWarning("Pick up from here.");

            roads = new List<Road>();
            bridges = new List<Bridge>();
            var points = new List<Point>();

            foreach(var settlement in worldMap.SettlementData.Settlements)
            {
                points.Add(new Point(settlement.X, settlement.Y));
            }

            var triangles = BowyerWatson.Triangulate(points);
            var graph = new HashSet<Edge>();
            foreach (var triangle in triangles)
                graph.UnionWith(triangle.edges);

            var tree = Kruskal.MinimumSpanningTree(graph);

            foreach(var edge in tree)
            {
                var a = worldMap.GetNode((int)edge.Point1.X, (int)edge.Point1.Y);
                var b = worldMap.GetNode((int)edge.Point2.X, (int)edge.Point2.Y);

                var newRoad = new Road();
                newRoad.pointA = Find(a).ID;
                newRoad.pointB = Find(b).ID;

                var path = worldMap.FindNodePath(a.x, a.y, b.x, b.y);
                if (path == null) continue;

                BuildRoad(newRoad, path);
                roads.Add(newRoad);
            }
            worldMap.TerrainData.Roads = roads.ToArray();
            worldMap.TerrainData.Bridges = bridges.ToArray();
        }

        private Settlement Find(WorldTile tile)
        {
            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                if (tile.x == settlement.X && tile.y == settlement.Y) return settlement;
            }
            return null;
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

        #region - Obsolete -
        private void FindPaths_Old()
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

            foreach (var settlement in worldMap.SettlementData.Settlements)
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
        #endregion
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