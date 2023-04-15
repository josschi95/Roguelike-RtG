using System.Collections.Generic;
using JS.WorldGeneration;

public class Settlement
{
    public TerrainNode Node { get; private set; }

    public string name { get; private set; }
    public int ID { get; private set; }
    public SettlementType type { get; private set; }
    public HumanoidTribe tribe { get; private set; }
    public int population { get; private set; }

    public List<TerrainNode> territory;
    public List<TerrainNode> areaOfInfluence;

    private Dictionary<Settlement, int> foreignRelations;

    public Settlement(string name, int ID, TerrainNode node, SettlementType type, HumanoidTribe humanoids, int population)
    {
        this.name = name;
        this.ID = ID;
        Node = node;
        Node.Settlement = this;

        this.type = type;
        tribe = humanoids;
        this.population = population;

        territory = new List<TerrainNode>();
        areaOfInfluence = new List<TerrainNode>();
        foreignRelations = new Dictionary<Settlement, int>();
    }

    public void AddTerritory(TerrainNode node)
    {
        if (territory.Contains(node)) return;

        territory.Add(node);
        node.Territory = this;
    }

    public void RemoveTerritory(TerrainNode node)
    {
        if (!territory.Contains(node)) return;

        node.Territory = null;
        territory.Remove(node);
    }

    public void AddAreaOfInfluence(TerrainNode node)
    {
        areaOfInfluence.Add(node);
    }

    public void RemoveAreaOfInfluence(TerrainNode node)
    {
        areaOfInfluence.Remove(node);
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
}