namespace JS.ECS
{
    public class RenderBase : ComponentBase
    {
        protected Transform transform;
        public Transform Transform
        {
            get
            {
                if (transform == null)
                {
                    transform = entity.GetComponent<Transform>();
                }
                return transform;
            }
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TransformChanged) RenderSystem.UpdatePosition(this);
        }

        public override void Disassemble()
        {
            base.Disassemble();
            RenderSystem.RemoveRender(this);
            transform = null;
        }
    }
}