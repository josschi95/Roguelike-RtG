using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace JS.World.Alchemy
{
    public static class FloraGenerator
    {
        private static Dictionary<string, AlchemyBlueprint> _alchemyBlueprints;

        #region - File Loading -
        private static void LoadBlueprints(bool loadFromResources = true)
        {
            _alchemyBlueprints = new Dictionary<string, AlchemyBlueprint>();

            if (loadFromResources)
            {
                LoadFromResources();
                return;
            }

            string[] files = Directory.GetFiles(Application.persistentDataPath);
            foreach (string fileName in files)
            {
                if (fileName.Contains("Alchemy"))
                {
                    ProcessBlueprints(fileName);
                    break;
                }
            }
        }

        private static void LoadFromResources()
        {
            var textAsset = Resources.Load("Blueprints/Alchemy") as TextAsset;

            StringReader reader = new StringReader(textAsset.text);
            string json = reader.ReadToEnd();

            AlchemyFile file = JsonUtility.FromJson<AlchemyFile>(json);

            foreach(var blueprint in file.AlchemyEffects)
            {
                _alchemyBlueprints[blueprint.Name] = blueprint;
            }
        }

        private static void ProcessBlueprints(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string json = reader.ReadToEnd();

            AlchemyFile file = JsonUtility.FromJson<AlchemyFile>(json);
            foreach (var blueprint in file.AlchemyEffects)
            {
                _alchemyBlueprints[blueprint.Name] = blueprint;
            }
        }
        #endregion

        public static void GenerateFlore()
        {
            LoadBlueprints();

            int count = _alchemyBlueprints.Count;

            var flora = new Flora[count];

            for (int i = 0; i < count; i++)
            {
                var newFlora = new Flora();
                newFlora.Name = MarkovNames.GetName("Herbs", false, UnityEngine.Random.Range(7, 12));

                //newFlora.Name = MarkovChainNames.get
            }
        }
    }

    [Serializable]
    public class Flora
    {
        public string Name;
        public string Effects;
        public string FoundIn;
    }

    [Serializable]
    public class AlchemyFile
    {
        public AlchemyBlueprint[] AlchemyEffects;
    }

    [Serializable]
    public class AlchemyBlueprint
    {
        public string Name;
        public string Biome;
    }
}