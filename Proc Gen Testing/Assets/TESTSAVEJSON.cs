using JS.CharacterSystem;
using System.IO;
using UnityEngine;

public class TESTSAVEJSON : MonoBehaviour
{
    public RacialArchetype[] archetypes;
    public CharacterRace[] races;

    [ContextMenu("Save")]
    public void TestSave()
    {
        var data = new RaceData();

        //racial archetypes
        data.archetypes = new SpeciesArchetype[archetypes.Length];
        for (int i = 0; i < data.archetypes.Length; i++)
        {
            string parent = "";
            if (archetypes[i] is RacialChildArchetype child) parent = child.parentArchetype.ArchetypeName;
            var type = new SpeciesArchetype(archetypes[i].ArchetypeName, archetypes[i].NeedsAir, archetypes[i].NeedsFood, archetypes[i].NeedsSleep, parent);
            data.archetypes[i] = type;
        }

        //character races
        data.species = new CharacterSpecies[races.Length];
        for (int i = 0; i < data.species.Length; i++)
        {
            var race = new CharacterSpecies();
            race.name = races[i].name;

            race.archetype = races[i].Archetype.ArchetypeName;
            race.racialType = (int)races[i].RaceType;

            race.size = races[i].Size.SizeModifier;

            race.attributeModifiers = new ObjectIDReference[races[i].RacialStats.AttributeModifiers.Length];
            for (int j = 0; j < race.attributeModifiers.Length; j++)
            {
                var pair = races[i].RacialStats.AttributeModifiers[j];
                var mod = new ObjectIDReference((int)pair.attribute, pair.value);
                race.attributeModifiers[j] = mod;
            }

            race.skillModifiers = new ObjectIDReference[races[i].RacialStats.SkillModifiers.Length];
            for (int j = 0; j < race.skillModifiers.Length; j++)
            {
                var pair = races[i].RacialStats.SkillModifiers[j];
                var mod = new ObjectIDReference((int)pair.skill, pair.value);
                race.skillModifiers[j] = mod;
            }

            race.resistanceModifiers = new ObjectIDReference[races[i].RacialStats.ResistanceModifiers.Length];
            for (int j = 0; j < race.resistanceModifiers.Length; j++)
            {
                var pair = races[i].RacialStats.ResistanceModifiers[j];
                var mod = new ObjectIDReference(pair.damageType.ID, pair.value);
                race.resistanceModifiers[j] = mod;
            }

            race.startingAge = races[i].StartingAge;
            race.lifespan = races[i].LifeSpan;

            data.species[i] = race;
        }

        var persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "RaceData.json";
        Debug.Log(persistentPath);
        string json = JsonUtility.ToJson(data, true);

        using StreamWriter writer = new StreamWriter(persistentPath);
        writer.Write(json);
    }
}

public class RaceData
{
    public SpeciesArchetype[] archetypes;
    public CharacterSpecies[] species;
}

[System.Serializable]
public class CharacterSpecies
{
    public string name;
    
    public int racialType;
    public string archetype;
    public int size;
    
    public ObjectIDReference[] attributeModifiers;
    public ObjectIDReference[] skillModifiers;
    public ObjectIDReference[] resistanceModifiers;

    public DiceCombo startingAge;
    public DiceCombo lifespan;
}

[System.Serializable]
public class SpeciesArchetype
{
    public string name;
    public string parent;
    public bool breathes, eats, sleeps;

    public SpeciesArchetype(string name, bool breathes, bool eats, bool sleeps, string parent)
    {
        this.name = name;
        this.breathes = breathes;
        this.eats = eats;
        this.sleeps = sleeps;
        this.parent = parent;
    }
}

[System.Serializable]
public class ObjectIDReference
{
    public int id;
    public int value;

    public ObjectIDReference(int i, int v)
    {
        id = i; value = v;
    }
}