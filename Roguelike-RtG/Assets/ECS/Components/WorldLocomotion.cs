using JS.CharacterSystem;

namespace JS.ECS
{
    public class WorldLocomotion : ComponentBase
    {
        private Transform _transform;
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = entity.GetComponent<Transform>();
                }
                return _transform;
            }
        }

        public int MoveSpeed
        {
            get
            {
                if (entity.TryGetStat("MoveSpeed", out StatBase stat))
                {
                    return stat.Value;
                }
                return 1;
            }
        }

        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}

