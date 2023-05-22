using System.Collections.Generic;
using JS.WorldMap;

public class Settlement
{
    public int x { get; private set; }
    public int y { get; private set; }

    public string name { get; private set; }
    public int ID { get; private set; }
    public SettlementType type { get; private set; }
    public HumanoidTribe tribe { get; private set; }
    public int population { get; private set; }
    //public bool isSeaFaring { get; private set; }
    public bool isSubterranean { get; private set; }

    public List<GridCoordinates> Territory;
    public List<GridCoordinates> Reach;

    private Dictionary<Settlement, int> foreignRelations;

    public Settlement(string name, int ID, WorldTile node, SettlementType type, HumanoidTribe humanoids, int population)
    {
        this.name = name;
        this.ID = ID;
        x = node.x; 
        y = node.y;

        //Set to a seafaring settlement if placed on an island
        //isSeaFaring = node.Island != null;
        //Sets the settlement as subterranean if placed in a mountain
        isSubterranean = node.Mountain != null;

        this.type = type;
        tribe = humanoids;
        this.population = population;

        Territory = new List<GridCoordinates>();
        Reach = new List<GridCoordinates>();
        foreignRelations = new Dictionary<Settlement, int>();
    }

    public void Relocate(WorldTile node)
    {
        x = node.x;
        y = node.y;
    }

    public void Relocate(int x, int y)
    {
        this.x = x; this.y = y;
    }

    public void AdjustSize(SettlementType type, int newPopulation)
    {
        this.type = type;
        population = newPopulation;
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
        //node.Territory = this;
    }

    public void AddNewRelation(Settlement otherSettlement, int initialDisposition = 0)
    {
        if (!foreignRelations.ContainsKey(otherSettlement))
        {
            foreignRelations.Add(otherSettlement, initialDisposition);
        }
        else
        {
            foreignRelations[otherSettlement] = initialDisposition;
        }
    }

    public void ModifyRelation(Settlement otherSettlement, int dispositionChange)
    {
        if (foreignRelations.ContainsKey(otherSettlement))
        {
            foreignRelations[otherSettlement] += dispositionChange;
        }
        else
        {
            foreignRelations.Add(otherSettlement, dispositionChange);
        }
    }

    public void DeconstructSettlement()
    {
        //if (Node != null && Node.Settlement == this) Node.Settlement = null;
        //Node = null;

        /*for (int i = 0; i < territory.Count; i++)
        {
            if (territory[i].Territory == this) territory[i].Territory = null;
        }*/
        //territory.Clear();
        Territory.Clear();
        //areaOfInfluence.Clear();
        Reach.Clear();
        foreignRelations.Clear();
    }
}