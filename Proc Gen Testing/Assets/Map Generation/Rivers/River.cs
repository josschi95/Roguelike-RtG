using System.Collections.Generic;

[System.Serializable]
public class River
{
	public int Length;
	public List<TerrainNode> Nodes;
	public int ID { get; private set; }

	public int Intersections;
	public float TurnCount;
	public Direction CurrentDirection;

	public bool hasBeenDug;

	public River()
	{
		Nodes = new List<TerrainNode>();
	}

	public void Register(int id)
    {
		ID = id;
        for (int i = 0; i < Nodes.Count; i++)
        {
			Nodes[i].SetRiverPath(this);
        }
    }

	public void AddNode(TerrainNode node)
	{
		Nodes.Add(node);
	}
}
