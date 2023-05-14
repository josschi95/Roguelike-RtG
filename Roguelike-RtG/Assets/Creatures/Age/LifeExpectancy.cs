using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Age/Life Expectancy")]
    public class LifeExpectancy : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField] public bool ages = true;
        public bool Ages => ages;

        [Space]

        [SerializeField] private DiceCombo lifespan;
        public DiceCombo Lifespan => lifespan;

        public int ChildAge { get; private set; }
        public int AdolescentAge { get; private set; }
        public int YoungAdultAge { get; private set; }
        public int MiddleAge { get; private set; }
        public int OldAge { get; private set; }
        public int VenerableAge { get; private set; }
        public int MinLifeExpectancy { get; private set; }
        public int MaxLifeExpectancy { get; private set; }

        private void OnValidate()
        {
            MaxLifeExpectancy = lifespan.modifier + (lifespan.diceCount * lifespan.diceSides);
            MinLifeExpectancy = lifespan.modifier + lifespan.diceCount;

            VenerableAge = Mathf.RoundToInt(MaxLifeExpectancy * 0.75f);
            OldAge = Mathf.RoundToInt(MaxLifeExpectancy * 0.5f);
            MiddleAge = Mathf.RoundToInt(VenerableAge * 0.5f);
            YoungAdultAge = Mathf.RoundToInt(MiddleAge * 0.5f);
            AdolescentAge = Mathf.RoundToInt(YoungAdultAge * 0.75f);
            ChildAge = Mathf.RoundToInt(YoungAdultAge * 0.5f);
        }
    }
}