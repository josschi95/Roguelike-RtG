using System.Collections.Generic;
using UnityEngine;

public class MarkovChainNames : MonoBehaviour
{
    [Range(1, 3)]
    [SerializeField] private int n_gram = 2;

    [Space]

    [Range(5, 20)]
    [SerializeField] private int nameLength = 5;
    [Range(1, 20)]
    [SerializeField] private int namesToGenerate = 1;
    [SerializeField] private bool cleanText;
    [Space]

    [SerializeField] private TextAsset[] textFiles;
    [SerializeField] private bool debugDictionary;

    private string[] charactersToRemove = { "[", "]", "{", "}", "(", ")", ".", ",", "!", "?", ";", ":", " ", "\n" };

    private int previousN_gram; //make sure the current parameter aligns with the model
    private List<string> capitalLetters;
    private Dictionary<string, List<char>> markovModel;

    public bool townName;
    private string[] townSuffixes = {"bury", "borough", "brough", "burgh", "by", "cester", "ford", "ham", "mouth", 
        "stead", "port", "clare", "ley", "view", "folk", "dorf", "wych", "wick", "wich", "thorpe", "thorp", "ceter", 
        "stadt", "caster", "dale", "field", "town", "ton", "ville" };

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

    public void GetNames()
    {
        for (int i = 0; i < namesToGenerate; i++)
        {
            Debug.Log(GetName());
        }
    }

    public string GetName()
    {
        if (markovModel == null || previousN_gram != n_gram) GenerateModel();

        string currentState = capitalLetters[Random.Range(0, capitalLetters.Count)];
        char nextState;
        string output = string.Empty;
        output += currentState;

        for (int i = 0; i < nameLength; i++)
        {
            if (!markovModel.ContainsKey(currentState)) break;
            int index = Random.Range(0, markovModel[currentState].Count);
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

    public void GenerateModel()
    {
        previousN_gram = n_gram;
        capitalLetters = new List<string>();
        markovModel = new Dictionary<string, List<char>>();

        string[] names = GetText();
        /*foreach (var name in names)
        {
            if (name.Length < n_gram + 1) continue; //not long enough to give a valid pair

            for (int i = 0; i < name.Length - n_gram; i++)
            {
                string key = "";
                for (int j = 0; j < n_gram; j++)
                {
                    key += name[i + j];
                }
                char value = name[i + n_gram];

                if (!_markov.ContainsKey(key))
                {
                    if (i == 0) _uppers.Add(key);
                    if (i == 0) Debug.Log("i is zero, new key: " + key);

                    _markov[key] = new List<char>();
                    //Debug.Log("Key: " + key);
                }
                else Debug.Log("Already contains key: " + key);
                _markov[key].Add(value);

                //Debug.Log("Value: " + value);
            }
        }*/

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
                    if (char.IsUpper(key[0])) capitalLetters.Add(key);
                    markovModel[key] = new List<char>();
                }
                markovModel[key].Add(value);
            }
        }
        //Debug.Log("Model Generated. " + capitalLetters.Count + " uppers.");
        //Debug.Log("Model Generated. " + markovModel.Keys.Count + " keys.");
    }
}
