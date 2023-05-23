using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldMap
{
    [System.Serializable]
    public class Settlement
    {
        [SerializeField] private int x;
        [SerializeField] private int y;
        public int X => x;
        public int Y => y;

        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private int id;
        public int ID => id;
        [SerializeField] private int typeID;
        public int TypeID => typeID;
        [SerializeField] private int tribeID;
        public int TribeID => tribeID;
        [SerializeField] private int _population;
        public int Population => _population;

        //public bool isSeaFaring { get; private set; }
        public bool isSubterranean { get; private set; }

        public List<GridCoordinates> Territory;
        public List<GridCoordinates> Reach;

        [SerializeField] private Dictionary<int, int> foreignRelations; //ID, disposition

        public Settlement(string name, int ID, WorldTile node, SettlementType type, HumanoidTribe humanoids, int population)
        {
            _name = name;
            id = ID;
            x = node.x;
            y = node.y;

            //Set to a seafaring settlement if placed on an island
            //isSeaFaring = node.Island != null;
            //Sets the settlement as subterranean if placed in a mountain
            isSubterranean = node.Mountain != null;

            typeID = type.ID;
            tribeID = humanoids.ID;
            _population = population;

            Territory = new List<GridCoordinates>();
            Reach = new List<GridCoordinates>();
            foreignRelations = new Dictionary<int, int>();
        }

        public void Relocate(WorldTile node)
        {
            x = node.x;
            y = node.y;
        }

        public void AdjustSize(SettlementType type, int newPopulation)
        {
            typeID = type.ID;
            _population = newPopulation;
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
            Reach.Clear();
            foreignRelations.Clear();
        }
    }
}