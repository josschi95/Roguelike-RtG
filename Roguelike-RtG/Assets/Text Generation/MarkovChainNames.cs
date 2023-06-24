using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

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

    private List<char> uppers;
    private Dictionary<char, List<char>> markovModel;

    private string GetNewName()
    {
        var markovModel = MakeMarkovModel();

        char currentState = markovModel.ElementAt(Random.Range(0, markovModel.Count)).Key;
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

        return output;
    }

    private char GetUpper()
    {
        char c = 'c';
        while (!char.IsUpper(c))
        {
            c = markovModel.ElementAt(Random.Range(0, markovModel.Count)).Key;
        }
        return c;
    }
    public void GetName()
    {
        if (markovModel == null) Debug.LogError("Model has not been generated.");

        char currentState = uppers[Random.Range(0, uppers.Count)];
        //char currentState = markovModel.ElementAt(Random.Range(0, markovModel.Count)).Key;
        //char currentState = GetUpper();
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

        Debug.Log(output);
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
    }

    private Dictionary<char, List<char>> MakeMarkovModel()
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
