namespace JS.ECS
{
    public class Locomotion : ComponentBase
    {
        public int MovementSpeed = 100;
        public Transform Transform;

        public Locomotion(Transform transform)
        {
            Transform = transform;
            entity = transform.entity;
            LocomotionSystem.Register(this);
        }
    }
}