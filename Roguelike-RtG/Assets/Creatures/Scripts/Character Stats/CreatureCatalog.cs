using UnityEngine;
using JS.DomainSystem;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Creature Catalog")]
    public class CreatureCatalog : ScriptableObject
    {
        [SerializeField] private CreatureType[] creatureTypes;

        [Space]

        [SerializeField] private CreatureSubType[] creatureSubtypes;

        [Space]

        [SerializeField] private CharacterRace[] races;
        public CharacterRace[] Races => races;

        [Space]

        [SerializeField] private SizeCategory[] sizes;

        [Space]

        [SerializeField] private AgeCategory[] ages;
        [SerializeField] private AgeCategory ageless;
        public AgeCategory[] Ages => ages;
        public AgeCategory Ageless => ageless;

        [Space]

        [SerializeField] private CharacterAttribute[] primaryAttributes;
        [SerializeField] private CharacterAttribute[] secondaryAttributes;
        public CharacterAttribute[] PrimaryAttributes => primaryAttributes;

        [Space]

        [SerializeField] private CharacterSkill[] skills;
        public CharacterSkill[] Skills => skills;

        [Space]
        
        [SerializeField] private CharacterClass[] classes; 
        public CharacterClass[] Classes => classes;

        [Space]

        [SerializeField] private Domain[] domains;
        public Domain[] Domains => domains;


        public CharacterRace GetRace(int raceID)
        {
            for (int i = 0; i < races.Length; i++)
            {
                if (races[i].ID == raceID) return races[i];
            }
            return null;
        }

        public CharacterClass GetClass(int classID)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                if (classes[i].ID == classID) return classes[i];
            }
            return null;
        }

        public Domain GetDomain(int domainID)
        {
            for (int i = 0; i < domains.Length; i++)
            {
                if (domains[i].ID == domainID) return domains[i];
            }
            return null;
        }
    }
}