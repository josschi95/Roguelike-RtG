using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class MarkovChainNames : MonoBehaviour
{
    private class MarkovModel
    {
        public List<string> startLetters;
        public Dictionary<string, List<char>> model;
    }

    private readonly int n_gram = 2;

    [Space]

    [Range(5, 20)]
    [SerializeField] private int nameLength = 5;
    [Range(1, 20)]
    [SerializeField] private int namesToGenerate = 1;
    [SerializeField] private bool cleanText;
    [Space]

    [Space]
    [SerializeField] private TextAsset[] textFiles;
    [SerializeField] private bool debugDictionary;

    private string[] charactersToRemove = { "[", "]", "{", "}", "(", ")", ".", ",", "!", "?", ";", ":", " ", "\n" };

    private List<string> startLetters;
    private Dictionary<string, List<char>> markovModel;

    public bool townName;
    private string[] townSuffixes = {"bury", "borough", "brough", "burgh", "by", "cester", "ford", "ham", "mouth", 
        "stead", "port", "clare", "ley", "view", "folk", "dorf", "wych", "wick", "wich", "thorpe", "thorp", "ceter", 
        "stadt", "caster", "dale", "field", "town", "ton", "ville" };


    private Dictionary<string, MarkovModel> newModels;


    public void LoadModels()
    {
        newModels = new Dictionary<string, MarkovModel>();

        var files = Resources.LoadAll("Corpora", typeof(TextAsset));

        foreach(var file in files)
        {
            var corpus = file as TextAsset;

            newModels.Add(file.name, new MarkovModel());
            GenerateModel(newModels[file.name], corpus);
        }
    }

    private void GenerateModel(MarkovModel model, TextAsset assets)
    {
        model.startLetters = new List<string>();
        model.model = new Dictionary<string, List<char>>();

        string[] names = GetText(assets);

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

                if (!model.model.ContainsKey(key))
                {
                    if (char.IsUpper(key[0])) model.startLetters.Add(key);
                    model.model[key] = new List<char>();
                }
                model.model[key].Add(value);
            }
        }
    }

    private string[] GetText(TextAsset asset)
    {
        string body = asset.text;

        if (cleanText)
        {
            for (int i = 0; i < charactersToRemove.Length; i++)
            {
                body.Replace(charactersToRemove[i], "");
            }
        }
        //splits the text into an array of words
        string[] names = body.Trim().Split(",");
        return names;
    }

    public string GetName()
    {
        //if (markovModel == null || previousN_gram != n_gram) GenerateModel();

        string currentState = startLetters[UnityEngine.Random.Range(0, startLetters.Count)];
        char nextState;
        string output = string.Empty;
        output += currentState;

        for (int i = 0; i < nameLength; i++)
        {
            if (!markovModel.ContainsKey(currentState)) break;
            int index = UnityEngine.Random.Range(0, markovModel[currentState].Count);
            nextState = markovModel[currentState][index];

            output += nextState;

            currentState = output.Substring(output.Length - n_gram, n_gram);
        }

        if (townName)
        {
            char last = output[output.Length - 1];
            foreach (var suffix in townSuffixes)
            {
                if (suffix[0] == last)
                {
                    //cuts off last letter and attaches suffix
                    output = output.Substring(0, output.Length - 1) + suffix;
                    break;
                }
            }
        }

        return output;
    }

    /*
    private string[] GetText()
    {
        //pulls in the text from the given file
        string body = "";
        for (int i = 0; i < textFiles.Length; i++)
        {
            body += textFiles[i].text + " ";
        }

        if (cleanText)
        {
            for (int i = 0; i < charactersToRemove.Length; i++)
            {
                body.Replace(charactersToRemove[i], "");
            }
        }
        //splits the text into an array of words
        string[] names = body.Trim().Split(",");

        //Debug.Log(words.Length);

        return names;
    }

    public void GenerateModel()
    {
        previousN_gram = n_gram;
        startLetters = new List<string>();
        markovModel = new Dictionary<string, List<char>>();

        string[] names = GetText();

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

                if (!markovModel.ContainsKey(key))
                {
                    if (char.IsUpper(key[0])) startLetters.Add(key);
                    markovModel[key] = new List<char>();
                }
                markovModel[key].Add(value);
            }
        }
        //Debug.Log("Model Generated. " + capitalLetters.Count + " uppers.");
        //Debug.Log("Model Generated. " + markovModel.Keys.Count + " keys.");
    }
    */

    public void PrintName()
    {
        //Debug.Log(GetName());
    }

    public void PrintNames()
    {
        for (int i = 0; i < namesToGenerate; i++)
        {
            //PrintName();
        }
    }
}
