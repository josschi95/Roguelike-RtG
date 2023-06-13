using JS.CharacterSystem;
using JS.ECS.Tags;

namespace JS.ECS
{
    public class ObjectStack : ComponentBase
    {
        public ObjectStack() { }

        public int MaxStack { get; private set; }
        public int Amount { get; private set; }

        public bool CanStackWith(Entity entity)
        {
            if (entity == null) return false; //nothing to compare to
            if (entity.Name != this.entity.Name) return false; //Differently named
            if (EntityManager.GetTag<NeverStack>(entity) || EntityManager.GetTag<NeverStack>(this.entity)) return false;
            if (EntityManager.TryGetStat(entity, "HitPoints", out StatBase hp) && EntityManager.TryGetStat(this.entity, "HitPoints", out StatBase myHP))
            {
                if (hp != myHP) return false;
                return true;
            }
            return false;
        }

        public bool AddToStack(Entity entity)
        {
            if (!CanStackWith(entity)) return false;
            if (Amount >= MaxStack) return false;

            EntityManager.Destroy(entity);
            Amount++;

            return true;
        }

        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}