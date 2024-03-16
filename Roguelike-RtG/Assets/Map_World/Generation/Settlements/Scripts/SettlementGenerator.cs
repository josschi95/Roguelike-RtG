using System.Collections.Generic;
using UnityEngine;
using JS.World.Map.Features;

namespace JS.World.Map.Generation
{
    public class SettlementGenerator : MonoBehaviour
    {
        [SerializeField] private WorldGenerator worldGenerator;
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

        private List<Vector2> seedLocations;
        private List<CitySeed> seeds;
        private List<Settlement> settlements;
        private string historyReport;

        private readonly int startingPop = 25;
        private readonly int poissonRadius = 10;
        private readonly int adjustmentRange = 5;

        private void Start()
        {
            historyReport = string.Empty;
            settlements = new List<Settlement>();
            seeds = new List<CitySeed>();
        }

        private void Report(string message)
        {
            historyReport += message + "\n";
        }

        #region - Seed Placement -
        public void PlaceSeeds()
        {
            var size = new Vector2(worldMap.Width, worldMap.Height);
            seedLocations = Poisson.GeneratePoints(worldMap.Seed, poissonRadius, size);
            Debug.Log($"Initial Settlement Seeds: {seedLocations.Count}");
            //Removes all invalid locations
            for (int i = seedLocations.Count - 1; i >= 0; i--)
            {
                var node = worldMap.GetNode((int)seedLocations[i].x, (int)seedLocations[i].y);

                if (!node.IsLand) node = TryFindLand(node); //No settlements in the sea yet
                if (node != null && node.Mountain != null) node = TryFindPlains(node); //No settlements in the mountains
                if (node == null) seedLocations.RemoveAt(i);
            }
            Debug.Log($"Number of Nodes not land: {nodeNotLand}");
            Debug.Log($"Number of Nodes in Mountain: {nodeInMountain}");
            Debug.Log($"Culled Settlement Seeds: {seedLocations.Count}");
        }
        int nodeNotLand = 0;
        int nodeInMountain = 0;
        /// <summary>
        /// Returns the nearest WorldTile that is land, within the landCheckRadius
        /// </summary>
        private WorldTile TryFindLand(WorldTile tile)
        {
            nodeNotLand++;
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
            nodeInMountain++;
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

        #region - Settlement Expansion -
        public void RunSettlementHistory(int year)
        {
            Report("\n*** Year " + year + " ***\n");

            if (year == 0)
            {
                int startingSeeds = 6;
                for (int i = 0; i < startingSeeds; i++)
                {
                    OnNewSeed(seedLocations[0], 0);
                    seedLocations.RemoveAt(0);
                }
            }
            else if (seedLocations.Count > 0)
            {
                OnNewSeed(seedLocations[0], year);
                seedLocations.RemoveAt(0);
            }

            for (int i = seeds.Count - 1; i >= 0; i--)
            {
                Report(seeds[i].Name);
                OnSeedTurn(seeds[i]);
                Report("\n");
            }
        }

        private void OnNewSeed(Vector2 location, int year)
        {
            var node = worldMap.GetNode((int)location.x, (int)location.y);

            var newSeed = new CitySeed()
            {
                Node = node,
                YearFounded = year,
                Name = MarkovNames.GetName("TownNames", true, worldGenerator.PRNG.Next(5, 10), worldGenerator.PRNG),
                Population = startingPop,
                Type = hamlet,
            };

            string founder = MarkovNames.GetName("Forenames_Male", false, worldGenerator.PRNG.Next(5, 8), worldGenerator.PRNG);
            Report("The settlement of " + newSeed.Name + " was founded in year " + year + " by " + founder + "\n");

            newSeed.Facilities.Add(new Facility("Crop Farm", 5, string.Empty, "Food", newSeed.Node.x, newSeed.Node.y));

            //Settlements adjacent to water sources get docks, also provides trade
            if (worldMap.TerrainData.Coasts[newSeed.Node.x, newSeed.Node.y] || newSeed.Node.Rivers.Count > 0)
                newSeed.Facilities.Add(new Facility("Docks", 5, string.Empty, "Food", newSeed.Node.x, newSeed.Node.y));

            //Settlements adjacent to forests get Hunting Lodges
            if (biomeHelper.GetBiome(newSeed.Node.BiomeID).CanBeHunted)
                newSeed.Facilities.Add(new Facility("Hunting Lodge", 5, string.Empty, "Food", newSeed.Node.x, newSeed.Node.y));

            //Determine initial defensibility based on natural barriers
            foreach (var neighbor in newSeed.Node.neighbors_all)
            {
                if (!neighbor.IsLand || neighbor.Mountain != null || neighbor.Rivers.Count > 0)
                {
                    newSeed.Defense++;
                }
            }

            seeds.Add(newSeed);
        }

        private void OnSeedTurn(CitySeed seed)
        {
            int availableWorkforce = AssignWorkers(seed);

            CalculateFoodProduction(seed);

            AdjustToFoodAvailability(seed);

            DetermineCasualties(seed);

            if (seed.Population <= 0)
            {
                OnSeedLost(seed);
                return;
            }
            else if (seed.Population <= hamlet.minPopulation)
            {
                //Migrate to nearest settlement
            }

            TryAddNewFacility(seed, availableWorkforce);

            var adjustedType = GetAdjustedType(seed);
            if (adjustedType != seed.Type)
            {
                Report(seed.Name + " transitioned from a " + seed.Type.TypeName + " to a " + adjustedType.TypeName);
            }
            seed.Type = adjustedType;
        }

        private SettlementEvent GetEvent()
        {
            if (worldGenerator.PRNG.Next(0, 100) > 75)
            {
                return (SettlementEvent)worldGenerator.PRNG.Next(1, System.Enum.GetNames(typeof(SettlementEvent)).Length);

                /*apply the effect of a random event
                Plague (0.5-.075 population), GoodHarvest(foodMod = 2)
                BadHarvest(foodMod = 0.5), RaiderAttack, MonsterAttack, etc. */
            }
            else return SettlementEvent.None;
        }

        /// <summary>
        /// Assigns available workforce to each facility
        /// </summary>
        private int AssignWorkers(CitySeed seed)
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

            return availableWorkforce;
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
                    //crop yield is determined by precipitation and temperature, y = -4x^2 + 5.6x - 0.96 : optimal temp
                    float farmability = (-4f * (node.heatValue * node.heatValue) + 5.6f * node.heatValue - 0.96f) * node.precipitationValue;
                    float yieldMod = 1 + worldGenerator.PRNG.Next(-25, 25) * 0.01f; //produces +/- 25% diff each year

                    int output = Mathf.RoundToInt(50 * farmability * manning * yieldMod);

                    seed.FoodProduction += output;
                    Report(seed.Name + "'s Farm produced " + output + " food");
                }
                else if (facility.Name.Equals("Docks"))
                {
                    float yieldMod = 1 + worldGenerator.PRNG.Next(-25, 25) * 0.01f; //produces +/- 25% diff each year
                    int output = Mathf.Clamp(Mathf.RoundToInt((50 - node.DangerTier * 2) * manning * yieldMod), 0, 50);
                    seed.FoodProduction += output;
                    Report(seed.Name + "'s Docks produced " + output + " food");
                }
                else if (facility.Name.Equals("Hunting Lodge"))
                {
                    float yieldMod = 1 + worldGenerator.PRNG.Next(-25, 25) * 0.01f; //produces +/- 25% diff each year
                    int output = Mathf.Clamp(Mathf.RoundToInt((50 - node.DangerTier * 2) * manning * yieldMod), 0, 50);
                    seed.FoodProduction += output;
                    Report(seed.Name + "'s Hunting Lodge produced " + output + " food");
                }
            }

            seed.FoodStores += seed.FoodProduction;
        }

        /// <summary>
        /// Adjust population depending on surplus or deficit of food
        /// </summary>
        private void AdjustToFoodAvailability(CitySeed seed)
        {
            if (seed.FoodStores < seed.Population)
            {
                //Food stores weren't enough to provide for the whole town
                int diff = seed.Population - seed.FoodStores;
                seed.FoodStores = 0;
                seed.starvations += diff;
                seed.Population -= diff;
                Report(seed.Name + " lost " + diff + " lives to starvation.");
            }
            else //Everyone was fed, make some babies
            {
                seed.FoodStores -= seed.Population;

                int babies = seed.Population / 2;
                Report(seed.Name + "'s population grew from " + seed.Population + " to " + (seed.Population + babies));
                seed.Population += babies;
            }
        }

        /// <summary>
        /// Decrease population based on how dangerous the area is, minus their defenses
        /// </summary>
        private void DetermineCasualties(CitySeed seed)
        {
            int casualties = Mathf.Clamp(seed.Node.DangerTier - seed.Defense, 0, seed.Population);
            seed.Population -= casualties;

            if (casualties > 0)
            {
                seed.casualties += casualties;
                Report(seed.Name + " lost " + casualties + " lives to the wilds.");
            }
        }

        private void TryAddNewFacility(CitySeed seed, int availableWorkforce)
        {
            if (availableWorkforce < 5) return;

            if (availableWorkforce < 10 && availableWorkforce > 5)
            {
                //Add new farm/fishery
                return;
            }

            //Baker? Butcher? etc.
        }

        private SettlementType GetAdjustedType(CitySeed settlement)
        {
            if (settlement.Population >= city.minPopulation) return city;
            if (settlement.Population >= town.minPopulation) return town;
            if (settlement.Population >= village.minPopulation) return village;
            return hamlet;
        }

        private void OnSeedLost(CitySeed seed)
        {
            Report(seed.Name + " was lost.");
            Report(seed.starvations + " members starved to death.");
            Report(seed.casualties + " were killed by monsters.");
            Report(seed.plagueDeaths + " were killed by the plague.\n");
            seeds.Remove(seed);
        }
        #endregion

        #region - Finalization -
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

        public void FinalizeSettlements()
        {
            ConvertSeedsToSettlements();
            //GetTerritoryReport();
            ReportWriter.FileReport("Settlement History", historyReport);

            worldMap.SettlementData.PlaceSettlements(settlements.ToArray());
        }

        private void ConvertSeedsToSettlements()
        {
            foreach (var seed in seeds)
            {
                var tribe = GetTribeBids(seed.Node);
                var newSettlement = new Settlement(seed.Name, settlements.Count, seed.Node, seed.Type, tribe, seed.Population);
                settlements.Add(newSettlement);
            }
        }
        #endregion

        #region - Obsolete -
        private HumanoidTribe GetTribe(int ID)
        {
            for (int i = 0; i < tribes.Length; i++)
            {
                if (tribes[i].ID == ID) return tribes[i];
            }
            throw new System.Exception("Tribe ID not found.");
        }

        private Settlement FindNearestSettlement(Settlement fromSettlement)
        {
            Settlement toSettlement = null;
            float dist = int.MaxValue;
            for (int i = 0; i < settlements.Count; i++)
            {
                if (settlements[i] == fromSettlement) continue;
                var newDist = GridMath.GetStraightDist(fromSettlement.Coordinates, settlements[i].Coordinates);

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
                var node = worldMap.GetNode(settlement.x, settlement.y);
                int range = (settlement.TypeID + 1) * 2;
                var area = worldMap.GetNodesInRange_Circle(node, range);
                foreach(var areaNode in area)
                {
                    var newSettlement = FindSettlement(areaNode.x, areaNode.y);
                    if (newSettlement == null || newSettlement == settlement) continue;

                    var disposition = tribeRelations.GetDisposition(GetTribe(settlement.TribeID), GetTribe(newSettlement.TribeID));

                    settlement.AddNewRelation(newSettlement, disposition);
                    newSettlement.AddNewRelation(settlement, disposition);
                    Debug.Log(GetTribe(settlement.TribeID) + " (" + settlement.TypeID + ") at " + settlement.x + "," + settlement.y +
                        " has encountered a " + GetTribe(newSettlement.TribeID) + " (" + newSettlement.TypeID + ") at " +
                        newSettlement.x + "," + newSettlement.y + ". They have a disposition of " + disposition);
                }
            }
        }

        public Settlement FindSettlement(int x, int y)
        {
            if (settlements == null) return null;

            for (int i = 0; i < settlements.Count; i++)
            {
                if (settlements[i].x == x && settlements[i].y == y)
                {
                    return settlements[i];
                }
            }
            return null;
        }

        private void ExpandTerritory(Settlement settlement)
        {
            //clamp max size based on settlement size

            int[,] mapFlags = new int[worldMap.Width, worldMap.Height];
            Queue<WorldTile> queue = new Queue<WorldTile>();
            queue.Enqueue(worldMap.GetNode(settlement.x, settlement.y));

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
                    if (GridMath.GetStraightDist(settlement.x, settlement.y, neighbor.x, neighbor.y) > 50 + Random.Range(-10, 5)) continue;

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

        private void ClaimResources()
        {
            foreach (var settlement in settlements)
            {
                var node = worldMap.GetNode(settlement.x, settlement.y);
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


                /*var type = GetNewType(settlement);
                int population = worldGenerator.PRNG.Next(type.minPopulation, type.maxPopulation);
                if (population < settlement.Population) population = settlement.Population;

                Debug.Log(settlements[i].Name + " upgraded from hamlet, pop. " + settlements[i].Population +
                    " to " + type.TypeName + ", pop. " + population);

                settlement.AdjustSize(type, population);*/
            }
        }

        private SettlementType GetNewType(Settlement settlement)
        {
            if (settlement.Facilities.Count > 6) return city;
            if (settlement.Facilities.Count > 4) return town;
            if (settlement.Facilities.Count >= 3) return village;
            return hamlet;
        }
        #endregion
    }
}

public enum SettlementEvent
{
    None,
    Feast,
    Famine,
    Plague,
    Raid,
    MonsterAttack
}

public enum SettlementAmenities
{
    Bakery,
    Butcher,

}
public enum SettlementNeeds
{
    Food,

}

//So I think for the current process
//all start as hamlets
//give them starting defensibility and facilities based on resources, use this to determine secondary starting size
//then generate roads, and for each road leading to a location (-1) add a "Trade" facility


//let's get crazy with it, start out as a hamlet with a set population
//for now, I can assume that all settlements have access to water, either through wells or rivers
//everybody gets a free farm to start
//farms produce 50 food, which can support up to 50 people