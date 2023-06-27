namespace JS.ECS
{
    public class Render : ComponentBase
    {
        public string Layer = "Default";
        public int Order = 0;
        public bool RenderIfDark = true; //Render if outside FoV
        public bool RenderOnWorldMap = false; //Render if on world map?

        public string Tile;
        public bool IsComposite = false;

        protected Transform transform;
        public Transform Transform
        {
            get
            {
                if (transform == null)
                {
                    transform = EntityManager.GetComponent<Transform>(entity);
                }
                return transform;
            }
        }

        public override void OnRegistered()
        {
            RenderSystem.NewRender(this);
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TransformChanged t) RenderSystem.UpdatePosition(this, t.smooth);
            else if (newEvent is AddedToInventory) RenderSystem.RemoveRender(this);
            else if (newEvent is RemovedFromInventory) RenderSystem.NewRender(this);
            else if (newEvent is Death) RenderSystem.RemoveRender(this);
        }

        public override void Disassemble()
        {
            RenderSystem.RemoveRender(this);
            transform = null;
        }
    }
}