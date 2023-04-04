using System.Collections.Generic;

[System.Serializable]
public class MountainRange
{
    public int ID { get; private set; }
    public List<TerrainNode> Nodes;
    public List<River> Rivers;

    public float AverageAltitude;
    public float peakAltitude;

    public MountainRange()
    {
        Nodes = new List<TerrainNode>();
        Rivers = new List<River>();
    }

    public void MergeMountain(MountainRange otherMountain)
    {
        //UnityEngine.Debug.Log("Mountain Merged");

        otherMountain.Nodes.AddRange(Nodes);
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].Mountain = otherMountain;
        }
        Nodes.Clear();
    }


    public void FinalizeValues(int ID)
    {
        this.ID = ID;
        AverageAltitude = 0;
        peakAltitude = 0;
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].Mountain = this;

            AverageAltitude += Nodes[i].altitude;
            if (Nodes[i].altitude > peakAltitude) peakAltitude = Nodes[i].altitude;
        }

        AverageAltitude /= Nodes.Count;
    }

    public void DeconstructRange()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].Mountain = null;
        }
        Nodes.Clear();
    }
}