using JS.Combat;
using UnityEngine;

namespace JS.CharacterSystem
{
    [System.Serializable]
    public class StatField
    {
        [SerializeField] private AttributeReference[] attributeModifiers;

        [Space]

        [SerializeField] private SkillReference[] skillModifiers;

        [Space]

        [SerializeField] private DamageReference[] damageModifiers;



        public AttributeReference[] AttributeModifiers => attributeModifiers;
        public SkillReference[] SkillModifiers => skillModifiers;
        public DamageReference[] DamageModifiers => damageModifiers;
    }
}