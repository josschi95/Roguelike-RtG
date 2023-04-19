using UnityEngine;
using JS.WorldGeneration;

[CreateAssetMenu(fileName = "Settlement Data", menuName = "World Generation/Settlements/Settlement Data")]
public class SettlementData : ScriptableObject
{
    private Settlement[] settlements;
    public Settlement[] Settlements => settlements;

    //later also include roads
    //maybe bridges, etc.

    public void AddSettlements(Settlement[] settlements)
    {
        this.settlements = settlements;
    }
}
