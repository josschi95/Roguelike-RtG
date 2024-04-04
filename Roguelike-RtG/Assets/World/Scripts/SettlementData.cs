using JS.World.Map;

public static class SettlementData
{
    private static Settlement[] _settlements;
    public static Settlement[] Settlements => _settlements;

    //later also include roads
    //maybe bridges, etc.

    public static void PlaceSettlements(Settlement[] settlements)
    {
        UnityEngine.Debug.Log("Setting Settlements.");

        _settlements = settlements;
    }

    public static Settlement FindSettlement(int x, int y)
    {
        if (_settlements == null) return null;

        for (int i = 0; i < _settlements.Length; i++)
        {
            if (_settlements[i].x == x && _settlements[i].y == y)
            {
                return _settlements[i];
            }
        }
        return null;
    }

    public static Settlement FindClaimedTerritory(int x, int y)
    {
        if (_settlements == null) return null;

        for (int i = 0; i < _settlements.Length; i++)
        {
            if (_settlements[i].OwnsTerritory(x,y)) return _settlements[i];
        }
        return null;
    }
}
