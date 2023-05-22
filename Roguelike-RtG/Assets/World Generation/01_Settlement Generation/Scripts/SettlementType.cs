using UnityEngine;


[CreateAssetMenu(fileName = "New Settlement", menuName = "World Generation/Settlements/Settlement Type")]
public class SettlementType : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public string TypeName { get; private set; }
    [field: SerializeField] public int MaxTerritory { get; private set; }
    [field: SerializeField] public int territorySize { get; private set; } = 1;

    [field: SerializeField] public int minPopulation { get; private set; }
    [field: SerializeField] public int maxPopulation { get; private set; }

    [field: SerializeField] public RuleTile settlementTile { get; private set; }

    public int AreaOfInfluence()
    {
        return Mathf.RoundToInt(Mathf.Pow(territorySize + 1, 2));
    }
}
