using UnityEngine;


[CreateAssetMenu(fileName = "New Settlement", menuName = "World Generation/Settlements/Settlement Type")]
public class SettlementType : ScriptableObject
{
    [field: SerializeField] public int territorySize { get; private set; } = 1;
    [field: SerializeField] public int minPopulation { get; private set; }
    [field: SerializeField] public int maxPopulation { get; private set; }

    [field: SerializeField] public RuleTile settlementTile { get; private set; }
}
