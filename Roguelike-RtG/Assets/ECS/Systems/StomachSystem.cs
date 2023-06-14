using JS.ECS;
using System.Collections.Generic;
using UnityEngine;

public class StomachSystem : MonoBehaviour
{
    private static StomachSystem instance;

    private List<Stomach> stomachs;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        stomachs = new List<Stomach>();
    }

    public static void Register(Stomach stomach)
    {
        instance.stomachs.Add(stomach);
    }

    public static void Unregister(Stomach stomach)
    {
        instance.stomachs.Remove(stomach);
    }

    //Called from GameEventListener on TimeSystem's NewRound
    public void OnGameTick()
    {
        for (int i = 0; i < stomachs.Count; i++)
        {
            stomachs[i].Satiation--;
            stomachs[i].Hydration--;

            var hunger = GetHunger(stomachs[i]);
            var thirst = GetThirst(stomachs[i]);

            //Add in game-mode setting where if it is reduced to 0, you die, or something really bad happens like falling unconscious

            if (stomachs[i].entity == EntityManager.Player)
            {
                if (hunger != stomachs[i].Hunger && hunger != HungerState.Neutral)
                    MessageSystem.NewMessage("You become " + hunger.ToString());

                if (thirst != stomachs[i].Thirst && thirst != ThirstState.Neutral)
                    MessageSystem.NewMessage("You become " + thirst.ToString());
            }

            //Add/Remove condition as necessary
            stomachs[i].Hunger = hunger;
            stomachs[i].Thirst = thirst;
        }
    }

    private HungerState GetHunger(Stomach stomach)
    {
        if (stomach.Satiation >= stomach.MaxSatiation) return HungerState.Bloated;
        if (stomach.Satiation >= stomach.MaxSatiation * 0.7f) return HungerState.Satiated;

        if (stomach.Satiation <= stomach.MaxSatiation * 0.1f) return HungerState.Starving;
        if (stomach.Satiation <= stomach.MaxSatiation * 0.25f) return HungerState.Famished;
        if (stomach.Satiation <= stomach.MaxSatiation * 0.4f) return HungerState.Hungry;

        return HungerState.Neutral;
    }

    private ThirstState GetThirst(Stomach stomach)
    {
        if (stomach.Satiation >= stomach.MaxSatiation) return ThirstState.Bloated;
        if (stomach.Satiation >= stomach.MaxSatiation * 0.7f) return ThirstState.Quenched;

        if (stomach.Satiation <= stomach.MaxSatiation * 0.1f) return ThirstState.Dehydrated;
        if (stomach.Satiation <= stomach.MaxSatiation * 0.25f) return ThirstState.Parched;
        if (stomach.Satiation <= stomach.MaxSatiation * 0.4f) return ThirstState.Thirsty;

        return ThirstState.Neutral;
    }
}

public enum HungerState
{
    Bloated,
    Satiated,

    Neutral,

    Hungry,
    Famished,
    Starving
}

public enum ThirstState
{
    Bloated,
    Quenched,

    Neutral,

    Thirsty,
    Parched,
    Dehydrated
}