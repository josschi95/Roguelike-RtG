using JS.CharacterSystem;

namespace JS.ECS
{
    public class ObjectStack : ComponentBase
    {
        public int MaxStack { get; private set; } = int.MaxValue;

        public bool CanStackWith(Entity entity)
        {
            if (entity == null) return false; //nothing to compare to
            if (entity.Name != this.entity.Name) return false; //Differently named
            if (entity.TryGetStat("HitPoints", out StatBase hp) && this.entity.TryGetStat("HitPoints", out StatBase myHP))
            {
                if (hp != myHP) return false;
                return true;
            }
            return false;
        }

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}