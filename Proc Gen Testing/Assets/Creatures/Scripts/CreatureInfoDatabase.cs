using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CreateAssetMenu(menuName = "Characters/Creature Info Database")]
public class CreatureInfoDatabase : ScriptableObject
{
    private Dictionary<string, SpeciesArchetype> archetypes = new Dictionary<string, SpeciesArchetype>();
    private Dictionary<string, CharacterSpecies> species = new Dictionary<string, CharacterSpecies>();

    public void LoadArchetypes(SpeciesArchetype[] speciesArchetypes, bool addChildren = false)
    {
        archetypes.Clear();

        var children = new List<SpeciesArchetype>();
        for (int i = 0; i < speciesArchetypes.Length; i++)
        {
            //Pass on any child archetypes for the first pass
            if (!addChildren && speciesArchetypes[i].parent == "")
            {
                children.Add(speciesArchetypes[i]);
                continue;
            }

            archetypes.Add(speciesArchetypes[i].name.ToLower(), speciesArchetypes[i]);
        }

        if (!addChildren)
        {
            LoadArchetypes(children.ToArray(), true);
        }
    }

    public void LoadSpecies(CharacterSpecies[] newSpecies)
    {
        species.Clear();

        for (int i = 0; i < newSpecies.Length; i++)
        {
            species.Add(newSpecies[i].name, newSpecies[i]);
        }
    }

    public CharacterSpecies GetSpecies(string name)
    {
        return species[name.ToLower()];
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(CreatureInfoDatabase))]
public class CreatureInfoDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Load File"))
        {
            LoadFile();
        }
    }

    private void LoadFile()
    {
        var persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "RaceData.json";

        StreamReader reader = new StreamReader(persistentPath);
        string json = reader.ReadToEnd();

        RaceData data = JsonUtility.FromJson<RaceData>(json);

        var database = target as CreatureInfoDatabase;
        database.LoadArchetypes(data.archetypes);
        database.LoadSpecies(data.species);
    }
}
#endif