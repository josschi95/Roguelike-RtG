using System.Collections.Generic;
using UnityEngine;

namespace JS.World.Map
{
    [System.Serializable]
    public class Settlement
    {
        [SerializeField] private Vector2Int _coordinates;
        public Vector2Int Coordinates => _coordinates;

        public int x => _coordinates.x;
        public int y => _coordinates.y;

        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private int id;
        public int ID => id;
        [SerializeField] private int typeID;
        public int TypeID => typeID;
        [SerializeField] private int tribeID;
        public int TribeID => tribeID;

        public int Population;

        public int FoodProduction;
        public int FoodSupply;

        public int Defensibility;
        public int ResourceRating;
        public List<Facility> Facilities;
        public Dictionary<string, int> Resources;
        public List<GridCoordinates> Territory;

        [SerializeField] private Dictionary<int, int> foreignRelations; //ID, disposition


        public Settlement(string name, int ID, WorldTile node, SettlementType type, HumanoidTribe humanoids, int population)
        {
            _name = name;
            id = ID;
            _coordinates = new Vector2Int(node.x, node.y);

            //Set to a seafaring settlement if placed on an island
            //isSeaFaring = node.Island != null;

            typeID = type.ID;
            tribeID = humanoids.ID;
            Population = population;

            Territory = new List<GridCoordinates>();
            Facilities = new List<Facility>();
            Resources = new Dictionary<string, int>();
            foreignRelations = new Dictionary<int, int>();
        }

        public void Relocate(WorldTile node)
        {
            _coordinates = new Vector2Int(node.x, node.y);
            //x = node.x;
            //y = node.y;
        }

        public void AdjustSize(SettlementType type, int newPopulation)
        {
            typeID = type.ID;
            Population = newPopulation;
        }

        public bool OwnsTerritory(int x, int y)
        {
            for (int i = 0; i < Territory.Count; i++)
            {
                if (Territory[i].x == x && Territory[i].y == y) return true;
            }
            return false;
        }

        public void AddTerritory(WorldTile node)
        {
            if (OwnsTerritory(node.x, node.y)) return;
            Territory.Add(new GridCoordinates(node.x, node.y));
        }

        public void AddNewRelation(Settlement otherSettlement, int initialDisposition = 0)
        {
            if (!foreignRelations.ContainsKey(otherSettlement.ID))
            {
                foreignRelations.Add(otherSettlement.ID, initialDisposition);
            }
            else
            {
                foreignRelations[otherSettlement.ID] = initialDisposition;
            }
        }

        public void ModifyRelation(Settlement otherSettlement, int dispositionChange)
        {
            if (foreignRelations.ContainsKey(otherSettlement.ID))
            {
                foreignRelations[otherSettlement.ID] += dispositionChange;
            }
            else
            {
                foreignRelations.Add(otherSettlement.ID, dispositionChange);
            }
        }

        public void DeconstructSettlement()
        {
            Territory.Clear();
            foreignRelations.Clear();
        }
    }

    public class CitySeed
    {
        public string Name;
        public WorldTile Node;
        public int YearFounded;

        public SettlementType Type;
        public SettlementType LargestSize; //How big it was at the height of its population

        public int Population;
        public int AvailableWorkforce;

        public int FoodStores;
        public int FoodProduction;

        public int Offense;
        public int Defense;

        public List<WorldTile> Territory = new List<WorldTile>();
        public List<Facility> Facilities = new List<Facility>();

        public int starvations;
        public int casualties;
        public int plagueDeaths;
    }
}

public class Facility
{
    public string Name;
    public int AssignedWorkers;
    public int RequiredWorkers;

    public string Input;
    public string Output;
    public int MaxCount;

    public int X, Y;

    public Facility() { }

    public Facility(string name, int workers, string input, string output, int x, int y)
    {
        Name = name;
        RequiredWorkers = workers;
        Input = input;
        Output = output;
        X = x; Y = y;
    }
}

public class SettlementStructure
{
    public int ID;
    public string Name;
    public int Size; //Square
}