namespace JS.ECS
{
    public class Render : ComponentBase
    {
        public bool RenderIfDark = true; //Render if outside FoV
        public bool RenderOnWorldMap = false; //Render if on world map?

        public string Tile;
        public bool IsComposite = false;

        protected Physics _physics;
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

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TransformChanged) RenderSystem.UpdatePosition(this);
            if (newEvent is Death) RenderSystem.RemoveRender(this);
        }

        public override void Disassemble()
        {
            base.Disassemble();
            RenderSystem.RemoveRender(this);
            _physics = null;
        }
    }
}