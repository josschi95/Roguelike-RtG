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
            if (corpse == null || Random.value * 100 > corpse.CorpseChance) return;

            var newCorpse = EntityFactory.GetEntity(corpse.CorpseBlueprint);
            if (newCorpse == null) throw new System.Exception("Corpse Blueprint not found!");

            var entityPos = EntityManager.GetComponent<Transform>(entity);
            var corpsePos = EntityManager.GetComponent<Transform>(newCorpse);
            TransformSystem.SetPosition(corpsePos, entityPos);

            //EntityManager.TryGetComponent<Inventory>(entity, out  var inventory);

            //Also need to drop all items in inventory and any equipped items
        }
    }
}