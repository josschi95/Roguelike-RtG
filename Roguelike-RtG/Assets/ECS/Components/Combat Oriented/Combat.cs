namespace JS.ECS
{
    /// <summary>
    /// Component that allows an object to declare attacks against others
    /// </summary>
    public class Combat : ComponentBase
    {
        public bool hasMultiStrike = false; //removes major penalty to making multiple attacks
        public int CritRangeBonus = 0;

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
    }
}