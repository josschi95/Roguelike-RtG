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
                if (blueprint.Name.Contains('*')) continue;
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
                if (blueprint.Name.Contains('*')) continue;
                _alchemyBlueprints[blueprint.Name] = blueprint;
            }
        }
        #endregion

        public static void GenerateFlora(System.Random PRNG = null)
        {
            if (PRNG == null)
            {
                Debug.LogWarning("Creating new random");
                PRNG = new System.Random();
            }

            string report = string.Empty;
            report += "Herbs\n\n";

            LoadBlueprints();

            int count = _alchemyBlueprints.Count;

            var effectsList = new List<AlchemyBlueprint>();
            var effectQueue1 = new Queue<AlchemyBlueprint>();
            var effectQueue2 = new Queue<AlchemyBlueprint>();
            var effectQueue3 = new Queue<AlchemyBlueprint>();
            var effectQueue4 = new Queue<AlchemyBlueprint>();

            foreach (var blueprint in _alchemyBlueprints)
            {
                effectsList.Add(blueprint.Value);
            }

            MathsUtil.ShuffleList(effectsList, PRNG);
            for (int i = 0; i < effectsList.Count; i++)
            {
                effectQueue1.Enqueue(effectsList[i]);
            }
            MathsUtil.ShuffleList(effectsList, PRNG);
            for (int i = 0; i < effectsList.Count; i++)
            {
                effectQueue2.Enqueue(effectsList[i]);
            }
            MathsUtil.ShuffleList(effectsList, PRNG);
            for (int i = 0; i < effectsList.Count; i++)
            {
                effectQueue3.Enqueue(effectsList[i]);
            }
            MathsUtil.ShuffleList(effectsList, PRNG);
            for (int i = 0; i < effectsList.Count; i++)
            {
                effectQueue4.Enqueue(effectsList[i]);
            }

            var flora = new Flora[count];
            for (int i = 0; i < count; i++)
            {
                var newFlora = new Flora();
                newFlora.Name = MarkovNames.GetName("Herbs", false, PRNG.Next(7, 12), PRNG);

                var effect1 = effectQueue1.Dequeue();
                var effect2 = effectQueue2.Dequeue();
                var effect3 = effectQueue3.Dequeue();
                var effect4 = effectQueue4.Dequeue();

                newFlora.Effects[0] = effect1.Name;
                newFlora.Effects[1] = effect2.Name;
                newFlora.Effects[2] = effect3.Name;
                newFlora.Effects[3] = effect4.Name;

                newFlora.FoundIn[0] = effect1.Biome;
                newFlora.FoundIn[1] = effect2.Biome;
                newFlora.FoundIn[2] = effect3.Biome;
                newFlora.FoundIn[3] = effect4.Biome;
                
                flora[i] = newFlora;

                report += "\n\n" + newFlora.Name + "\nEffects: ";
                for (int j = 0; j < newFlora.Effects.Length; j++)
                {
                    report += newFlora.Effects[j] + ",";
                }
                report += "\nFound In: ";
                for (int j = 0; j < newFlora.FoundIn.Length; j++)
                {
                    report += newFlora.FoundIn[j] + ",";
                }
                report += "\n";
            }

            ReportWriter.FileReport("Herbs", report);
        }
    }

    [Serializable]
    public class Flora
    {
        public string Name;
        public string[] Effects = new string[4];
        public string[] FoundIn = new string[4];
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