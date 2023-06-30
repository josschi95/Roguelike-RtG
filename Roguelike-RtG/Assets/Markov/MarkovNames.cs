using System.Collections.Generic;
using UnityEngine;

public static class MarkovNames
{
    private class MarkovModel
    {
        public List<string> startLetters;
        public Dictionary<string, List<char>> model;
    }

    private static int n_gram = 2;

    private static string[] townSuffixes = {"bury", "borough", "brough", "burgh", "by", "cester", "ford", "ham", "mouth",
        "stead", "port", "clare", "ley", "view", "folk", "dorf", "wych", "wick", "wich", "thorpe", "thorp", "ceter",
        "stadt", "caster", "dale", "field", "town", "ton", "ville" };

    private static Dictionary<string, MarkovModel> markovModels;

    public static void LoadModels()
    {
        markovModels = new Dictionary<string, MarkovModel>();

        var files = Resources.LoadAll("Corpora", typeof(TextAsset));

        foreach (var file in files)
        {
            var corpus = file as TextAsset;

            markovModels.Add(file.name, new MarkovModel());
            GenerateModel(markovModels[file.name], corpus);
        }
    }

    private static void GenerateModel(MarkovModel markov, TextAsset asset)
    {
        markov.startLetters = new List<string>();
        markov.model = new Dictionary<string, List<char>>();

        string[] names = asset.text.Trim().Split(',');

        for (int i = 0; i < names.Length; i++)
        {
            var name = names[i];
            if (name.Length < n_gram + 1) continue; //not long enough to give a valid pair

            for (int j = 0; j < name.Length - n_gram; j++)
            {
                string key = "";
                for (int k = 0; k < n_gram; k++)
                {
                    key += name[j + k];
                }
                char value = name[j + n_gram];

                if (!markov.model.ContainsKey(key))
                {
                    if (char.IsUpper(key[0])) markov.startLetters.Add(key);
                    markov.model[key] = new List<char>();
                }
                markov.model[key].Add(value);
            }
        }
    }

    public static string GetName(string source, bool town = false, int length = 5, System.Random PRNG = null)
    {
        if (!markovModels.ContainsKey(source)) return null;
        if (PRNG == null) PRNG = new System.Random();
        var markov = markovModels[source];

        string currentState = markov.startLetters[PRNG.Next(0, markov.startLetters.Count)];
        char nextState;
        string output = string.Empty;
        output += currentState;

        for (int i = 0; i < length; i++)
        {
            if (!markov.model.ContainsKey(currentState)) break;

            int index = PRNG.Next(0, markov.model[currentState].Count);
            nextState = markov.model[currentState][index];

            output += nextState;

            currentState = output.Substring(output.Length - n_gram, n_gram);
        }

        if (town) output = AppendTownSuffix(output, PRNG);

        return output;
    }

    /// <summary>
    /// Add a random town suffix onto the end of the name
    /// </summary>
    private static string AppendTownSuffix(string input, System.Random PRNG)
    {
        if (PRNG.Next(0, 100) > 35) return input;

        char last = input[input.Length - 1];

        var suffixes = new List<string>();
        foreach(var suffix in townSuffixes)
        {
            if (suffix[0] == last) suffixes.Add(suffix);
        }

        if (suffixes.Count == 0)
        {
            input += townSuffixes[PRNG.Next(0, townSuffixes.Length)];
        }
        else
        {
            input = input.Substring(0, input.Length - 1) + suffixes[PRNG.Next(0, suffixes.Count)];
        }

        return input;
    }
}
