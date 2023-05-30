using JS.CharacterSystem;
using JS.ECS.Tags;

namespace JS.ECS
{
    public class ObjectStack : ComponentBase
    {
        public int MaxStack { get; private set; }
        public int Amount { get; private set; }

        public ObjectStack(int maxStack = int.MaxValue, int amount = 1)
        {
            MaxStack = maxStack;
            Amount = amount;
        }

        public bool CanStackWith(Entity entity)
        {
            if (entity == null) return false; //nothing to compare to
            if (entity.Name != this.entity.Name) return false; //Differently named
            if (entity.GetTag<NeverStack>() || this.entity.GetTag<NeverStack>()) return false; //cannot stack
            if (entity.TryGetStat("HitPoints", out StatBase hp) && this.entity.TryGetStat("HitPoints", out StatBase myHP))
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

            entity.Destroy();
            Amount++;

            return true;
        }

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}