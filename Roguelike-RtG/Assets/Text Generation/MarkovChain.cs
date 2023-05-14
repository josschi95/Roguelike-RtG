using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MarkovChain : MonoBehaviour
{
    [Range(1, 4)]
    [SerializeField] private int n_gram = 3;
    [SerializeField] private int outputSize = 100;
    [SerializeField] private string startText;
    [SerializeField] private bool randomStartText;
    [SerializeField] private bool cleanText;
    [Space]

    [SerializeField] private TextAsset[] textFiles;
    [SerializeField] private bool debugDictionary;

    private string[] charactersToRemove = { "[", "]", "{", "}", "(", ")", ".", ",", "!", "?", ";", ":" };

    [ContextMenu("Generate")]
    public void Generate()
    {
        Debug.Log(GenerateText());
        //MakeUnweightedMarkovModel();
        //var model = MakeWeightedMarkovModel();
        //Debug.Log(GenerateText(model));
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
        string[] words = body.Split(" ");

        //Debug.Log(words.Length);

        return words;
    }

    #region - Weighted Markov -
    private string GenerateText(Dictionary<string, Dictionary<string, float>> markovModel)
    {
        int n = 0;
        
        if (!markovModel.ContainsKey(startText) || randomStartText)
        {
            var newText = markovModel.ElementAt(Random.Range(0, markovModel.Count)).Key;
            startText = newText;
        }
        
        string currentState = startText;
        string nextState;
        string output = "";

        output += currentState + " ";

        while(n < outputSize)
        {
            nextState = GetWeightedRandom(markovModel[currentState]);

            currentState = nextState;
            output += currentState + " ";
            n++;
        }

        return output;
    }

    private string GetWeightedRandom(Dictionary<string, float> choices)
    {
        var choice = Random.value; //returns random value between 0 and 1
        float cumulative = 0;
        int option = 1;
        Debug.Log("Options: " + choices.Keys.Count);
        foreach(var pair in choices)
        {
            cumulative += pair.Value;
            if (choice <= cumulative)
            {
                Debug.Log("Selected option: " + option);
                return pair.Key;
            }
            option++;
        }
        throw new UnityException("Ya done fucked");
    }

    private Dictionary<string, Dictionary<string, float>> MakeWeightedMarkovModel()
    {
        string[] words = GetText();
        
        //Created a nested frequency table
        //the Key and Value of the nested dictionary represent how frequently that string follows the Key of the first dictionary
        var markovModel = new Dictionary<string, Dictionary<string, float>>();

        int length = words.Length - Mathf.RoundToInt(Mathf.Pow(n_gram, 2)) + 1;
        for (int i = 0; i < length; i++)
        {
            string currentState = "", nextState = "";
            for (int j = 0; j < n_gram; j++)
            {
                currentState += words[i + j] + " ";
                nextState += words[i + j + n_gram] + " ";
            }

            currentState = currentState.Trim();
            nextState = nextState.Trim();

            if (!markovModel.ContainsKey(currentState))
            {
                markovModel[currentState] = new Dictionary<string, float>();
                markovModel[currentState][nextState] = 1;
                //markovModel.Add(currentState, new Dictionary<string, float>());
                //markovModel[currentState].Add(nextState, 1);
            }
            else
            {
                if (markovModel[currentState].ContainsKey(nextState))
                {
                    markovModel[currentState][nextState] += 1;
                }
                else
                {
                    //markovModel[currentState].Add(nextState, 1);
                    markovModel[currentState][nextState] = 1;
                }
            }
        }

        markovModel = SetMarkovFrequency(markovModel);
        return markovModel;
    }

    /// <summary>
    /// Returns a copy of the given markov model with values modified to reflect frequency of each string value for each key
    /// </summary>
    private Dictionary<string, Dictionary<string, float>> SetMarkovFrequency(Dictionary<string, Dictionary<string, float>> original)
    {
        int n = 0;
        var copy = new Dictionary<string, Dictionary<string, float>>();
        
        //Copy over the values of the original
        foreach(var key in original.Keys)
        {
            string newKey = key;
            copy.Add(newKey, new Dictionary<string, float>());

            foreach(var nestedPair in original[key])
            {
                copy[newKey].Add(nestedPair.Key, nestedPair.Value);
            }
        }

        //Modify the values of the original 
        foreach(var key in copy.Keys)
        {
            //Debug.Log("Key[" + key + "] : " + original[key].Keys.Count);
            float total = 0;
            foreach(var pair in copy[key])
            {
                total += pair.Value;
            }

            foreach(var pair in copy[key])
            {
                string nestedKey = pair.Key;
                float value = pair.Value / total;
                original[key][nestedKey] = value;
                if (value == 1) n++;
            }
        }
        Debug.Log(n + " single options out of " + original.Keys.Count + " states");
        return original;
    }
    #endregion

    private string GenerateText()
    {
        var markovModel = MakeUnweightedMarkovModel();

        int n = 0;

        if (!markovModel.ContainsKey(startText) || randomStartText)
        {
            var newText = markovModel.ElementAt(Random.Range(0, markovModel.Count)).Key;
            startText = newText;
        }

        string currentState = startText;
        string nextState;
        string output = ""; output += currentState + " ";

        while (n < outputSize)
        {
            Debug.Log(n + ": Current State: " + currentState);

            int index = Random.Range(0, markovModel[currentState].Count);
            nextState = markovModel[currentState][index];
            Debug.Log(n + ": Next State: " + nextState);

            output += nextState + " ";
            
            //
            var newState = currentState.Split();
            currentState = "";
            for (int i = 1; i < n_gram; i++)
            {
                currentState += newState[i] + " ";
            }
            currentState += nextState;

            n++;
        }

        return output;
    }

    private Dictionary<string, List<string>> MakeUnweightedMarkovModel()
    {
        string[] words = GetText();

        //Created a nested frequency table
        //the Key and Value of the nested dictionary represent how frequently that string follows the Key of the first dictionary
        var markovModel = new Dictionary<string, List<string>>();

        int length = words.Length - Mathf.RoundToInt(Mathf.Pow(n_gram, 2)) + 1;
        for (int i = 0; i < length; i++)
        {
            string currentState = "";
            for (int j = 0; j < n_gram; j++)
            {
                currentState += words[i + j] + " "; //This checks out
            }

            string nextState = words[i + n_gram]; //Just set it to the word immediately following the n_gram

            currentState = currentState.Trim();
            nextState = nextState.Trim();

            if (!markovModel.ContainsKey(currentState))
            {
                markovModel[currentState] = new List<string>();
            }

            markovModel[currentState].Add(nextState);
        }

        foreach(var pair in markovModel)
        {
            
            for (int i = 0; i < pair.Value.Count; i++)
            {
                if (debugDictionary) Debug.Log(pair.Key + ": " + pair.Value[i]);
            }
        }

        return markovModel;
    }


}
