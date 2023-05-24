using UnityEngine;

[CreateAssetMenu(menuName = "World Generation/Settlements/Tribal Relations")]
public class TribalRelation : ScriptableObject
{
    [System.Serializable]
    public class TribeRelation
    {
        public HumanoidTribe tribeA;
        public HumanoidTribe tribeB;
        [Range(-100, 100)]
        public int disposition;
    }

    [field: SerializeField] public TribeRelation[] Relations { get; private set; }

    public int GetDisposition(HumanoidTribe tribeA, HumanoidTribe tribeB)
    {
        if (tribeA == tribeB) return 100;
        foreach(TribeRelation relation in Relations)
        {
            if (relation.tribeA != tribeA && relation.tribeB != tribeA) continue;
            if (relation.tribeA != tribeB && relation.tribeB != tribeB) continue;
            return relation.disposition;
        }
        Debug.LogWarning("Relation Not Found Between " + tribeA + " and " + tribeB);
        return 0;
    }
}