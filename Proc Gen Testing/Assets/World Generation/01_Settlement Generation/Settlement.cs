public class Settlement
{
    public int x { get; private set; }
    public int y { get; private set; }

    public int ID { get; private set; }
    public SettlementType settlementType { get; private set; }
    public HumanoidTribe occupants { get; private set; }
    public int population { get; private set; }

    public Settlement(int ID, int x, int y, SettlementType type, HumanoidTribe humanoids, int population)
    {
        this.ID = ID;

        this.x = x;
        this.y = y;

        settlementType = type;
        occupants = humanoids;
        this.population = population;
    }
}