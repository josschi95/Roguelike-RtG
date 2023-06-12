using JS.ECS;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    private static LootSystem instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public static void OnRandomLoot(Entity entity)
    {
        if (Random.value > 0.25f) return; //25% chance to drop random loot
        Debug.Log("Spawning loot from slain entity");
        //Check zone tier

        //check level of entity?

        //Roll dice, if higher than some random number, spawn loot

        //spawn loot at entity's position
    }
}
