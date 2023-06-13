namespace JS.ECS
{
    /// <summary>
    /// Component that allows an object to declare attacks against others
    /// </summary>
    public class Combat : ComponentBase
    {
        public bool hasMultiStrike = false; //removes major penalty to making multiple attacks
        public int CritRangeBonus = 0;

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

        public override void OnEvent(Event newEvent)
        {
            
        }
    }
}