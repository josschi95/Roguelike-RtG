using JS.ECS;
using UnityEngine;

public class CorpseManager : MonoBehaviour
{
    public static CorpseManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public static void OnCreatureDeath(Entity entity)
    {
        var corpse = entity.GetComponent<Corpse>();
        if (corpse != null && Random.value * 100 >= corpse.CorpseChance)
        {
            Debug.Log("Dropping corpse");
            var newCorpse = EntityFactory.GetEntity(corpse.CorpseBlueprint);
            if (newCorpse == null) throw new System.Exception("Corpse Blueprint not found!");

            var ePhys = entity.GetComponent<JS.ECS.Physics>();
            var cPhys = newCorpse.GetComponent<JS.ECS.Physics>();
            cPhys.Position = ePhys.Position;
            cPhys.LocalPosition = ePhys.LocalPosition;
        }
    }
}
