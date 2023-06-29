using System.Collections.Generic;
using UnityEngine;
using DelaunayVoronoi;

namespace JS.WorldMap.Generation
{
    public class SettlementGenerator : MonoBehaviour
    {
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private MarkovChainNames markov;
        [SerializeField] private WorldGenerationParameters mapFeatures;
        [SerializeField] private BiomeHelper biomeHelper;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private TribalRelation tribeRelations;

        [Space]

        [SerializeField] private HumanoidTribe[] tribes;
        [SerializeField] private SettlementType[] settlementTypes;
        [SerializeField] private SettlementType city;
        [SerializeField] private SettlementType town;
        [SerializeField] private SettlementType village;
        [SerializeField] private SettlementType hamlet;
        private int startingPopulation = 25;
        private int[] settlementCount;

        private List<CitySeed> seeds;
        private List<Settlement> settlements;
        private string historyReport;

        private int poissonRadius = 10;
        private int adjustmentRange = 5;

        /// <summary>
        /// Sets the initial values for Settlement Generation.
        /// </summary>
        public void SetInitialValues(WorldSize size)
        {
            settlements = new List<Settlement>();
            seeds = new List<CitySeed>();

            settlementCount = mapFeatures.SettlementCount(size);
            for (int i = 0; i < settlementCount.Length; i++)
            {
                settlementCount[i] *= tribes.Length;
            }
        }

        private void Report(string message)
        {
            historyReport += message + "\n";
        }

        private HumanoidTribe GetTribe(int ID)
        {
            for (int i = 0; i < tribes.Length; i++)
            {
                if (tribes[i].ID == ID) return tribes[i];
            }
            throw new System.Exception("Tribe ID not found.");
        }

        #region - Settlement Placement -
        public void PlaceSettlements()
        {
            historyReport = string.Empty;

            markov.townName = true;
            var size = new Vector2(worldMap.Width, worldMap.Height);
            var points = Poisson.GeneratePoints(worldMap.Seed, poissonRadius, size);

            for (int i = 0; i < points.Count; i++)
            {
                var node = worldMap.GetNode((int)points[i].x, (int)points[i].y);

                if (!node.IsLand) node = TryFindLand(node); //No settlements in the sea
                if (node != null && node.Mountain != null) node = TryFindPlains(node); //No settlements in the mountains
                if (node == null) continue;

                //Place seed at the node
                var newSeed = new CitySeed()
                {
                    Node = node,
                    Name = markov.GetName(),
                    Population = startingPopulation,
                    Type = hamlet,
                };
                seeds.Add(newSeed);
            }

            SetStartingValues();

            int seedGrowthYears = 10;
            for (int i = 0; i < seedGrowthYears; i++)
            {
                Report("\nYear " + i);
                RunSettlementHistory();
                //Debug.Break();
            }

            //ClaimResources();
            /*Determine maximum sustainable population
            On or adjacent to arable land? +1, add farm facility
            On or adjacent to water source? +1, add fishery facility, docks
            On or adjacent to mineral deposit? +1, add mine facility
            On or adjacent to forest? +1, add lumber mill


            On an island? Clamp that value, probs max Village, maybe town depending on size

            */
            //All settlements start out as hamlets, then calculate the resource wealth of the surrounding area
            //Using that, as well as a tribe's ExpansionRating, claim territory for each settlement and grow in size
            //Settlement location also needs to be tracked on the Regional scale, 
            //Hamlets and Villages can occupy a single Region Map, Towns should take 4? and cities 9
            //Place satellite locations in regino tiles surrounding each settlement,
            //Farms
            //Mines - require presence of an Ore
            //Quarries
            //Hatcheries - requires presence of lake/river
            //Ports/Docks - these can serve as travel points for crossing bodies of water

            //CleanUpMinorSettlements();

            //Set Area of Influence
            //Based on settlement size
            //Probably use flood fill again
            //GetSettlementConnections();

            ConvertSeedsToSettlements();
            //GetTerritoryReport();
            ReportWriter.FileReport("Settlement History", historyReport);

            DrawLines();
            worldMap.SettlementData.PlaceSettlements(settlements.ToArray());
        }

        #region - Seed Placement Adjustments -
        /// <summary>
        /// Returns the nearest WorldTile that is land, within the landCheckRadius
        /// </summary>
        private WorldTile TryFindLand(WorldTile tile)
        {
            WorldTile landNode = null;
            float dist = int.MaxValue;

            var nodes = worldMap.GetNodesInRange_Square(tile, adjustmentRange);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (!nodes[i].IsLand) continue;
                var newDist = GridMath.GetStraightDist(tile.x, tile.y, nodes[i].x, nodes[i].y);
                if (newDist < dist)
                {
                    dist = newDist;
                    landNode = nodes[i];
                }
            }
            return landNode;
        }

        /// <summary>
        /// Returns the nearest WorldTile that is not in the Mountains.
        /// </summary>
        private WorldTile TryFindPlains(WorldTile tile)
        {
            WorldTile flatNode = null;
            float dist = int.MaxValue;
            var nodes = worldMap.GetNodesInRange_Square(tile, adjustmentRange);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (!nodes[i].IsLand) continue; //no water
                if (nodes[i].Mountain != null) continue; //no mountains

                var newDist = GridMath.GetStraightDist(tile.x, tile.y, nodes[i].x, nodes[i].y);
                if (newDist < dist)
                {
                    dist = newDist;
                    flatNode = nodes[i];
                }
            }

            return flatNode;
        }

        /// <summary>
        /// Returns the nearest coastal WorldTile
        /// </summary>
        private WorldTile TryFindWater(WorldTile tile)
        {
            WorldTile flatNode = null;
            float dist = int.MaxValue;
            var nodes = worldMap.GetNodesInRange_Square(tile, adjustmentRange);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (!nodes[i].IsLand) continue; //no water
                if (nodes[i].Mountain != null) continue; //no mountains
                if (!worldMap.TerrainData.Coasts[nodes[i].x, nodes[i].y] && nodes[i].Rivers.Count == 0) continue;

                var newDist = GridMath.GetStraightDist(tile.x, tile.y, nodes[i].x, nodes[i].y);
                if (newDist < dist)
                {
                    dist = newDist;
                    flatNode = nodes[i];
                }
            }

            return flatNode;
        }
        #endregion
        #endregion

        #region - Settlement Expansion -
        public void SetStartingValues()
        {
            //Grant each settlement a starting farm and fishery if relevant
            foreach (var seed in seeds)
            {
                seed.Facilities.Add(new Facility("Crop Farm", 5, string.Empty, "Food", seed.Node.x, seed.Node.y));

                //Settlements adjacent to water sources get docks, also provides trade
                if (worldMap.TerrainData.Coasts[seed.Node.x, seed.Node.y] || seed.Node.Rivers.Count > 0)
                    seed.Facilities.Add(new Facility("Docks", 5, string.Empty, "Food", seed.Node.x, seed.Node.y));

                //Settlements adjacent to forests get Hunting Lodges
                if (biomeHelper.GetBiome(seed.Node.BiomeID).CanBeHunted)
                    seed.Facilities.Add(new Facility("Hunting Lodge", 5, string.Empty, "Food", seed.Node.x, seed.Node.y));

                //Determine initial defensibility based on natural barriers
                foreach (var neighbor in seed.Node.neighbors_all)
                {
                    if (!neighbor.IsLand || neighbor.Mountain != null || neighbor.Rivers.Count > 0)
                    {
                        seed.Defensibility++;
                    }
                }
            }
        }

        private void ClaimResources()
        {
            foreach(var settlement in settlements)
            {
                var node = worldMap.GetNode(settlement.X, settlement.Y);
                settlement.Facilities.Add(new Facility("Crop Farm", 5, string.Empty, "Food", node.x, node.y));

                //Settlements adjacent to water sources get docks, also provides trade
                if (worldMap.TerrainData.Coasts[node.x, node.y] || node.Rivers.Count > 0)
                    settlement.Facilities.Add(new Facility("Docks", 5, string.Empty, "Food", node.x, node.y));


                if (worldMap.TerrainData.CoalMap[node.x, node.y] > 0)
                    settlement.Facilities.Add(new Facility("Coal Mine", 5, string.Empty, "Coal", node.x, node.y));

                if (worldMap.TerrainData.CopperMap[node.x, node.y] > 0)
                    settlement.Facilities.Add(new Facility("Copper Mine", 5, string.Empty, "Copper Ore", node.x, node.y));

                if (worldMap.TerrainData.IronMap[node.x, node.y] > 0)
                {
                    settlement.Defensibility++; //Iron weapons
                    if (worldMap.TerrainData.CoalMap[node.x, node.y] > 0) settlement.Defensibility++; //Steel weapons

                    settlement.Facilities.Add(new Facility("Iron Mine", 5, string.Empty, "Iron Ore", node.x, node.y));
                }

                if (worldMap.TerrainData.MithrilMap[node.x, node.y] > 0)
                {
                    settlement.Defensibility += 4; //Mithril weapons
                    settlement.Facilities.Add(new Facility("Mithril Mine", 5, string.Empty, "Mithril Ore", node.x, node.y));
                }

                if (worldMap.TerrainData.AdmanatineMap[node.x, node.y] > 0)
                {
                    settlement.Defensibility += 5; //Adamantine weapons
                    settlement.Facilities.Add(new Facility("Adamantine Mine", 5, string.Empty, "Adamantine Ore", node.x, node.y));
                }


                if (worldMap.TerrainData.SilverMap[node.x, node.y] > 0)
                    settlement.Facilities.Add(new Facility("Silver Mine", 5, string.Empty, "Silver Ore", node.x, node.y));

                if (worldMap.TerrainData.GoldMap[node.x, node.y] > 0)
                    settlement.Facilities.Add(new Facility("Gold Mine", 5, string.Empty, "Gold Ore", node.x, node.y));

                if (worldMap.TerrainData.GemstoneMap[node.x, node.y] > 0)
                    settlement.Facilities.Add(new Facility("Gem Mine", 5, string.Empty, "Gemstones", node.x, node.y));


                var type = GetNewType(settlement);
                int population = worldGenerator.PRNG.Next(type.minPopulation, type.maxPopulation);
                if (population < settlement.Population) population = settlement.Population;

                /*Debug.Log(settlements[i].Name + " upgraded from hamlet, pop. " + settlements[i].Population +
                    " to " + type.TypeName + ", pop. " + population);*/

                settlement.AdjustSize(type, population);
            }
        }

        private SettlementType GetNewType(Settlement settlement)
        {
            if (settlement.Facilities.Count > 6) return city;
            if (settlement.Facilities.Count > 4) return town;
            if (settlement.Facilities.Count >= 3) return village;
            return hamlet;
        }

        //I will likely end up moving this over to a HistoryGenerator class later on
        private void RunSettlementHistory()
        {
            for (int i = seeds.Count - 1; i >= 0; i--)
            {
                var seed = seeds[i];
                Report(seed.Name);
                if (worldGenerator.PRNG.Next(0, 100) > 75)
                {
                    /*apply the effect of a random event
                    Plague (0.5-.075 population), GoodHarvest(foodMod = 2)
                    BadHarvest(foodMod = 0.5), RaiderAttack, MonsterAttack, etc. */
                }

                AssignWorkers(seed);

                CalculateFoodProduction(seed);

                AdjustToFoodProduction(seed);

                //Decrease population based on how dangerous the area is, minus their defenses
                int casualties = Mathf.Clamp(seed.Node.DangerTier - seed.Defensibility, 0, seed.Population);
                seed.Population -= casualties;

                if (casualties > 0)
                {
                    seed.casualties += casualties;
                    Report(seed.Name + " lost " + casualties + " lives to the wilds.");
                }

                if (seed.Population <= 0)
                {
                    Report(seed.Name + " was lost.");
                    Report(seed.starvations + " members starved to death.");
                    Report(seed.casualties + " were killed by monsters.");
                    Report(seed.plagueDeaths + " were killed by the plague.");
                    Report("\n");

                    seeds.Remove(seed);
                    continue;
                }
                else if (seed.Population <= hamlet.minPopulation)
                {
                    //Migrate to nearest settlement
                }

                var adjustedType = GetAdjustedType(seed);
                if (adjustedType != seed.Type)
                {
                    Report(seed.Name + " transitioned from a " + seed.Type.TypeName + " to a " + adjustedType.TypeName);
                }
                seed.Type = adjustedType;
                Report("\n");
            }
        }

        /// <summary>
        /// Assigns available workforce to each facility
        /// </summary>
        private void AssignWorkers(CitySeed seed)
        {
            int availableWorkforce = seed.Population;
            for (int i = 0; i < seed.Facilities.Count; i++)
            {
                seed.Facilities[i].AssignedWorkers = 0;
            }

            foreach(var facility in seed.Facilities)
            {
                if (availableWorkforce <= 0) break;

                while(availableWorkforce > 0 && facility.AssignedWorkers < facility.RequiredWorkers)
                {
                    availableWorkforce--;
                    facility.AssignedWorkers++;
                }
            }

            foreach (var facility in seed.Facilities)
            {
                if (facility.AssignedWorkers < facility.RequiredWorkers)
                {
                    Report(seed.Name + "'s " + facility.Name + " is undermanned: " + 
                        facility.AssignedWorkers + "/" + facility.RequiredWorkers);
                }
            }
        }
        
        /// <summary>
        /// Calculate the year's food yield
        /// </summary>
        private void CalculateFoodProduction(CitySeed seed)
        {
            seed.FoodProduction = 0;
            foreach (var facility in seed.Facilities)
            {
                var node = worldMap.GetNode(facility.X, facility.Y);
                float manning = (float)facility.AssignedWorkers / facility.RequiredWorkers;

                if (facility.Name.Equals("Crop Farm"))
                {
                    float p = node.precipitationValue * 100;
                    float t = -4f * (node.heatValue * node.heatValue) + 5.6f * node.heatValue - 0.96f;
                    float farmability = (p * 1) * 0.01f;

                    int output = Mathf.RoundToInt(50 * farmability * manning);

                    seed.FoodProduction += output;
                    Report(seed.Name + "'s Farm produced " + output + " food");
                }
                else if (facility.Name.Equals("Docks"))
                {
                    int output = Mathf.Clamp(Mathf.RoundToInt((50 - node.DangerTier * 2) * manning), 0, 50);
                    seed.FoodProduction += output;
                    Report(seed.Name + "'s Docks produced " + output + " food");
                }
                else if (facility.Name.Equals("Hunting Lodge"))
                {
                    int output = Mathf.Clamp(Mathf.RoundToInt((50 - node.DangerTier * 2) * manning), 0, 50);
                    seed.FoodProduction += output;
                    Report(seed.Name + "'s Hunting Lodge produced " + output + " food");
                }
            }
        }

        private void AdjustToFoodProduction(CitySeed seed)
        {
            if (seed.Population > seed.FoodProduction)
            {
                seed.starvations += (seed.Population - seed.FoodProduction);

                Report(seed.Name + "'s population shrank from " + seed.Population + " to " + seed.FoodProduction);
            }
            else if (seed.Population < seed.FoodProduction)
            {
                Report(seed.Name + "'s population grew from " + seed.Population + " to " + seed.FoodProduction);
            }
            //Adjust population to available food supply
            seed.Population = seed.FoodProduction;
        }

        private SettlementType GetAdjustedType(CitySeed settlement)
        {
            if (settlement.Population >= city.minPopulation) return city;
            if (settlement.Population >= town.minPopulation) return town;
            if (settlement.Population >= village.minPopulation) return village;
            return hamlet;
        }

        private void ConvertSeedsToSettlements()
        {
            markov.townName = true;
            
            foreach(var seed in seeds)
            {
                var name = markov.GetName();
                var tribe = GetTribeBids(seed.Node);
                var newSettlement = new Settlement(name, settlements.Count, seed.Node, seed.Type, tribe, seed.Population);
                settlements.Add(newSettlement);
            }
        }

        private HumanoidTribe GetTribeBids(WorldTile node)
        {
            var biome = biomeHelper.GetBiome(node.BiomeID);

            var tribalBids = new List<HumanoidTribe>(tribes);

            for (int i = 0; i < tribes.Length; i++)
            {
                if (tribes[i].preferredBiomes.Contains(biome)) tribalBids.Add(tribes[i]);
                if (!tribes[i].opposedBiomes.Contains(biome)) tribalBids.Add(tribes[i]);
            }

            return tribalBids[worldGenerator.PRNG.Next(0, tribalBids.Count)];
        }
        #endregion

        //Draws a minimum spanning tree that represents where all of the roads will be formed
        private void DrawLines()
        {
            var test = new List<Point>();
            for (int i = 0; i < settlements.Count; i++)
            {
                test.Add(new Point(settlements[i].X, settlements[i].Y));
            }

            var triangles = BowyerWatson.Triangulate(test);
            var graph = new HashSet<Edge>();
            foreach (var triangle in triangles)
                graph.UnionWith(triangle.edges);

            var tree = Kruskal.MinimumSpanningTree(graph);
            foreach (var edge in tree)
            {
                var a = new Vector3((float)edge.Point1.X, (float)edge.Point1.Y);
                var b = new Vector3((float)edge.Point2.X, (float)edge.Point2.Y);
                Debug.DrawLine(a, b, Color.green, 1000f);
            }
        }

        #region - Obsolete -
        private Settlement FindNearestSettlement(Settlement fromSettlement)
        {
            Settlement toSettlement = null;
            float dist = int.MaxValue;
            for (int i = 0; i < settlements.Count; i++)
            {
                if (settlements[i] == fromSettlement) continue;
                var newDist = GridMath.GetStraightDist(fromSettlement.X, fromSettlement.Y, settlements[i].X, settlements[i].Y);

                if (newDist < dist)
                {
                    toSettlement = settlements[i];
                    dist = newDist;
                }
            }
            return toSettlement;
        }

        private void GetSettlementConnections()
        {
            foreach (Settlement settlement in settlements)
            {
                var node = worldMap.GetNode(settlement.X, settlement.Y);
                int range = (settlement.TypeID + 1) * 2;
                var area = worldMap.GetNodesInRange_Circle(node, range);
                foreach(var areaNode in area)
                {
                    var newSettlement = FindSettlement(areaNode.x, areaNode.y);
                    if (newSettlement == null || newSettlement == settlement) continue;

                    var disposition = tribeRelations.GetDisposition(GetTribe(settlement.TribeID), GetTribe(newSettlement.TribeID));

                    settlement.AddNewRelation(newSettlement, disposition);
                    newSettlement.AddNewRelation(settlement, disposition);
                    Debug.Log(GetTribe(settlement.TribeID) + " (" + settlement.TypeID + ") at " + settlement.X + "," + settlement.Y +
                        " has encountered a " + GetTribe(newSettlement.TribeID) + " (" + newSettlement.TypeID + ") at " +
                        newSettlement.X + "," + newSettlement.Y + ". They have a disposition of " + disposition);
                }
            }
        }

        public Settlement FindSettlement(int x, int y)
        {
            if (settlements == null) return null;

            for (int i = 0; i < settlements.Count; i++)
            {
                if (settlements[i].X == x && settlements[i].Y == y)
                {
                    return settlements[i];
                }
            }
            return null;
        }

        private SettlementType GetSettlementType()
        {
            for (int i = 0; i < settlementCount.Length; i++)
            {
                if (settlementCount[i] > 0)
                {
                    settlementCount[i]--;
                    return settlementTypes[i];
                }
            }
            return settlementTypes[settlementTypes.Length - 1]; //hamlets
        }

        private void ExpandTerritory(Settlement settlement)
        {
            //clamp max size based on settlement size

            int[,] mapFlags = new int[worldMap.Width, worldMap.Height];
            Queue<WorldTile> queue = new Queue<WorldTile>();
            queue.Enqueue(worldMap.GetNode(settlement.X, settlement.Y));

            //So this is going to repeat the same territory every single time, which seems a waste
            while (queue.Count > 0)// && territoryToAdd > 0)
            {
                //Limit settlements from getting too large at the start
                if (settlement.Territory.Count >= settlementTypes[settlement.TypeID].MaxTerritory) break;

                var node = queue.Dequeue();
                if (!settlement.OwnsTerritory(node.x, node.y))
                {
                    //AssignTerritory(settlement, node);
                    //territoryToAdd--;
                }

                for (int i = 0; i < node.neighbors_adj.Count; i++)
                {
                    var neighbor = node.neighbors_adj[i];
                    if (mapFlags[neighbor.x, neighbor.y] == 1) continue; //visited

                    mapFlags[neighbor.x, neighbor.y] = 1;
                    if (!neighbor.IsLand) continue; //is water
                    //if (settlement.isSubterranean != (neighbor.Mountain != null)) continue; //subterranean can't extend outside mountain, others can't extend into mountains
                    if (GetTribe(settlement.TribeID).opposedBiomes.Contains(biomeHelper.GetBiome(neighbor.BiomeID))) continue; //won't extend into opposed biomes

                    var claimant = FindClaimedTerritory(neighbor.x, neighbor.y);
                    if (claimant != null && claimant != settlement) continue; //already claimed
                    //The distance in node path count from the settlement to the new territory
                    //Also need to take into account going around mountains instead of through them
                    //if (worldMap.GetPathCount(settlement.Node, neighbor, settlement) > 50 + Random.Range(-10, 5)) continue;
                    if (GridMath.GetStraightDist(settlement.X, settlement.Y, neighbor.x, neighbor.y) > 50 + Random.Range(-10, 5)) continue;

                    queue.Enqueue(neighbor);
                }
            }
        }

        public Settlement FindClaimedTerritory(int x, int y)
        {
            if (settlements == null) return null;

            for (int i = 0; i < settlements.Count; i++)
            {
                if (settlements[i].OwnsTerritory(x, y)) return settlements[i];
            }
            return null;
        }
        #endregion
    }
}

//So I think for the current process
//all start as hamlets
//give them starting defensibility and facilities based on resources, use this to determine secondary starting size
//then generate roads, and for each road leading to a location (-1) add a "Trade" facility


//let's get crazy with it, start out as a hamlet with a set population
//for now, I can assume that all settlements have access to water, either through wells or rivers
//everybody gets a free farm to start
//farms produce 50 food, which can support up to 50 people