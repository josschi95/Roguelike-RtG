using UnityEngine;
using JS.WorldMap;

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

    public Settlement FindSettlement(int x, int y)
    {
        if (settlements == null) return null;

        for (int i = 0; i < settlements.Length; i++)
        {
            if (settlements[i].x == x && settlements[i].y == y)
            {
                return settlements[i];
            }
        }
        return null;
    }

    public Settlement FindClaimedTerritory(int x, int y)
    {
        if (settlements == null) return null;

        for (int i = 0; i < settlements.Length; i++)
        {
            if (settlements[i].OwnsTerritory(x,y)) return settlements[i];
        }
        return null;
    }
}
