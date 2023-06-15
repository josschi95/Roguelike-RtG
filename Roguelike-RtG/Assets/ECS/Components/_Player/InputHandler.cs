using JS.CharacterSystem;

namespace JS.ECS
{
    public class InputHandler : ComponentBase
    {
        public override void OnRegistered()
        {
            if (EntityManager.TryGetComponent<Brain>(entity, out var brain)) brain.HasOverride = true;
            InputSystem.OnNewInputTarget(this);
        }

        public override void Disassemble()
        {
            if (EntityManager.TryGetComponent<Brain>(entity, out var brain)) brain.HasOverride = false;
        }

        private Transform _transform;
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = EntityManager.GetComponent<Transform>(entity);
                }
                return _transform;
            }
        }

        private Physics _physics;
        public Physics Physics
        {
            get
            {
                if (_physics == null)
                {
                    _physics = EntityManager.GetComponent<Physics>(entity);
                }
                return _physics;
            }
        }

        private TimedActor _actor;
        public TimedActor Actor
        {
            get
            {
                if (_actor == null)
                {
                    _actor = EntityManager.GetComponent<TimedActor>(entity);
                }
                return _actor;
            }
        }

        private Combat _combat;
        public Combat Combat
        {
            get
            {
                if (_combat == null)
                {
                    _combat = EntityManager.GetComponent<Combat>(entity);
                }
                return _combat;
            }
        }

        public int MoveSpeed
        {
            get
            {
                if (EntityManager.TryGetStat(entity, "WALK", out StatBase stat))
                {
                    return stat.Value;
                }
                return 1;
            }
        }
    }
}