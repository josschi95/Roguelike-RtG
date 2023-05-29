namespace JS.ECS
{
    public class Locomotion : ComponentBase
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

        public int MovementSpeed = 100;

        public Locomotion()
        {
            LocomotionSystem.Register(this);
        }

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}