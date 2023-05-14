namespace JS.Combat
{
    [System.Serializable]
    public class DamageReference
    {
        [ReadOnly] public string name;
        public DamageType damageType;
        public int value;
    }
}