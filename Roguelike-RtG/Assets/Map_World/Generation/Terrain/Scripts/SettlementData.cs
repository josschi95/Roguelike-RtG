using UnityEngine;
using JS.World.Map;

[CreateAssetMenu(fileName = "Settlement Data", menuName = "World Generation/Settlements/Settlement Data")]
public class SettlementData : ScriptableObject
{
    [Header("References")]
    [SerializeField] private SettlementType[] settlementTypes;
    public SettlementType[] Types => settlementTypes;

    [SerializeField] private HumanoidTribe[] tribes;
    public HumanoidTribe[] Tribes => tribes;

    [Space]

    [SerializeField] private Settlement[] settlements;
    public Settlement[] Settlements => settlements;

    //later also include roads
    //maybe bridges, etc.

    public void PlaceSettlements(Settlement[] settlements)
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
