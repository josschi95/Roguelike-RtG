namespace JS.CharacterSystem
{
    public class CharacterSheet
    {
        public string Name { get; private set; }
        public CharacterRace PrimaryRace { get; private set; }
        public CharacterRace SecondaryRace { get; private set; }
        public CharacterClass Class { get; private set; }

        private Attribute[] attributes;
        private Skill[] skills;

        public CharacterSheet(string name, CharacterRace race1, CharacterRace race2, 
            CharacterClass job, int[] attributeScores, int[] skillScores)
        {
            Name = name;
            PrimaryRace = race1;
            SecondaryRace = race2;
            Class = job;

            attributes = new Attribute[attributeScores.Length];
            for (int i = 0; i < attributes.Length; i++)
            {
                int baseValue = attributeScores[i];

                foreach(var bonus in PrimaryRace.RacialStats.AttributeModifiers)
                {
                    if (bonus.attribute.ID == i) baseValue += bonus.value;
                }

                foreach (var bonus in Class.ArchetypeStas.AttributeModifiers)
                {
                    if (bonus.attribute.ID == i) baseValue += bonus.value;
                }

                attributes[i] = new Attribute(baseValue, 100, 100);
            }

            skills = new Skill[skillScores.Length];
            for (int i = 0; i < skills.Length; i++)
            {
                int baseValue = skillScores[i];

                foreach (var bonus in PrimaryRace.RacialStats.SkillModifiers)
                {
                    if (bonus.skill.ID == i) baseValue += bonus.value;
                }

                foreach (var bonus in Class.ArchetypeStas.SkillModifiers)
                {
                    if (bonus.skill.ID == i) baseValue += bonus.value;
                }

                skills[i] = new Skill(baseValue);
            }
        }

        public int GetAttributeValue(int attribute)
        {
            return attributes[attribute].Value;
        }

        public int GetSkillValue(int skill)
        {
            return skills[skill].Value;
        }
    }
}

