using JS.ECS;
using UnityEngine;

namespace JS.ECS
{
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
            var corpse = EntityManager.GetComponent<Corpse>(entity);
            if (corpse != null && Random.value * 100 >= corpse.CorpseChance)
            {
                Debug.Log("Dropping corpse");
                var newCorpse = EntityFactory.GetEntity(corpse.CorpseBlueprint);
                if (newCorpse == null) throw new System.Exception("Corpse Blueprint not found!");

                var ePhys = EntityManager.GetComponent<Physics>(entity);
                var cPhys = EntityManager.GetComponent<Physics>(newCorpse);
                cPhys.Position = ePhys.Position;
                cPhys.LocalPosition = ePhys.LocalPosition;
            }
        }
    }
}