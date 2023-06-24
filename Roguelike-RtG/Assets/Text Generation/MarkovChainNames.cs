using System.Collections.Generic;
using UnityEngine;

public class MarkovChainNames : MonoBehaviour
{
    [Range(5, 20)]
    [SerializeField] private int nameLength = 5;
    [Range(1, 20)]
    [SerializeField] private int namesToGenerate = 1;
    [SerializeField] private bool cleanText;
    [Space]

    [SerializeField] private TextAsset[] textFiles;
    [SerializeField] private bool debugDictionary;

    private string[] charactersToRemove = { "[", "]", "{", "}", "(", ")", ".", ",", "!", "?", ";", ":", " ", "\n" };
    private string[] townSuffixes = {"bury", "borough", "brough", "burgh", "by", "cester", "ford", "ham", "mouth", 
        "stead", "port", "clare", "ley", "view", "folk", "dorf", "wych", "wick", "wich", "thorpe", "thorp", "ceter", 
        "stadt", "caster", "dale", "field", "town", "ton", "ville" };
    
    private List<char> uppers;
    private Dictionary<char, List<char>> markovModel;
    public bool townName;

    public string GetName()
    {
        if (markovModel == null) GenerateModels();

        char currentState = uppers[Random.Range(0, uppers.Count)];
        char nextState;
        string output = string.Empty;
        output += currentState;

        for (int i = 0; i < nameLength; i++)
        {
            if (!markovModel.ContainsKey(currentState)) break;

            int index = Random.Range(0, markovModel[currentState].Count);
            nextState = markovModel[currentState][index];
            output += nextState;

            currentState = nextState;
        }

        if (townName)
        {
            char last = output[output.Length - 1];
            foreach (var suffix in townSuffixes)
            {
                if (suffix[0] == last)
                {
                    output = output.Substring(0, output.Length - 1) + suffix;
                    break;
                }
            }
        }
        //Debug.Log(output);
        return output;
    }

    public void GetNames()
    {
        for (int i = 0; i < namesToGenerate; i++)
        {
            GetName();
        }
    }


    public void GenerateModels()
    {
        uppers = new List<char>();
        markovModel = new Dictionary<char, List<char>>();
        string[] names = GetText();

        foreach(var name in names)
        {
            for (int i = 0; i < name.Length - 1; i++)
            {
                var first = name[i];
                var second = name[i + 1];

                //Create a separate list starting with all uppercase 
                if (char.IsUpper(first)) uppers.Add(first);

                if (!markovModel.ContainsKey(first))
                {
                    markovModel[first] = new List<char>();
                }
                markovModel[first].Add(second);
            }
        }

        foreach (var pair in markovModel)
        {
            for (int i = 0; i < pair.Value.Count; i++)
            {
                if (debugDictionary) Debug.Log(pair.Key + ": " + pair.Value[i]);
            }
        }

        Debug.Log("Model Generated. " + markovModel.Keys.Count + " keys.");
    }

    private Dictionary<char, List<char>> GetMarkovModel()
    {
        string[] names = GetText();
        var markovModel = new Dictionary<char, List<char>>();

        for (int i = 0; i < names.Length; i++)
        {
            for (int j = 0; j < names[i].Length - 1; j++)
            {
                var first = names[i][j];
                var second = names[i][j + 1];
                if (!markovModel.ContainsKey(first))
                {
                    markovModel[first] = new List<char>();
                }
                markovModel[first].Add(second);
            }
        }

        foreach (var pair in markovModel)
        {

            for (int i = 0; i < pair.Value.Count; i++)
            {
                if (debugDictionary) Debug.Log(pair.Key + ": " + pair.Value[i]);
            }
        }

        return markovModel;
    }

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
}
