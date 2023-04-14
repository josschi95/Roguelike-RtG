using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkovChain : MonoBehaviour
{
    [Range(1, 4)]
    [SerializeField] private int n_gram = 3;
    [SerializeField] private int outputSize = 100;
    private static string Join(string a, string b)
    {
        return a + " " + b;
    }

    private string Join(string[] strings)
    {
        string s = "";
        for (int i = 0; i < strings.Length; i++)
        {
            s += " " + strings[i];
        }
        s = s.Trim();
        return s;
    }

    //Need to document and comment all of this before using it so I actually understand what is happening
    static string Markov(string filePath, int keySize, int outputSize)
    {
        if (keySize < 1) throw new ArgumentException("Key size cannot be less than 1");

        string body;
        using (StreamReader sr = new StreamReader(filePath))
        {
            body = sr.ReadToEnd();
        }

        var words = body.Split();
        if (outputSize < keySize || words.Length < outputSize) throw new ArgumentException("Output size is out of range");

        Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
        for (int i = 0; i < words.Length - keySize; i++)
        {
            var key = words.Skip(i).Take(keySize).Aggregate(Join);

            string value;
            if (i + keySize < words.Length) value = words[i + keySize];
            else value = "";

            if (dict.ContainsKey(key)) dict[key].Add(value);
            else dict.Add(key, new List<string>() { value });
        }

        System.Random rand = new System.Random();
        List<string> output = new List<string>();

        int n = 0;
        int rn = rand.Next(dict.Count);
        string prefix = dict.Keys.Skip(rn).Take(1).Single();
        output.AddRange(prefix.Split());

        while (true)
        {
            var suffix = dict[prefix];
            if (suffix.Count == 1)
            {
                if (suffix[0] == "") return output.Aggregate(Join);
                output.Add(suffix[rn]);
            }
            else
            {
                rn = rand.Next(suffix.Count);
                output.Add(suffix[rn]);
            }

            if (output.Count >= outputSize)
            {
                return output.Take(outputSize).Aggregate(Join);
            }
            n++;
            prefix = output.Skip(n).Take(keySize).Aggregate(Join);
        }
    }

    private string Markov()
    {
        if (n_gram < 1) throw new ArgumentException("Key size cannot be less than 1");

        string body = textFile.text;

        var words = body.Split();
        if (outputSize < n_gram || words.Length < outputSize) throw new ArgumentException("Output size is out of range");

        Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
        for (int i = 0; i < words.Length - n_gram; i++)
        {
            var key = words.Skip(i).Take(n_gram).Aggregate(Join);

            string value;
            if (i + n_gram < words.Length) value = words[i + n_gram];
            else value = "";

            if (dict.ContainsKey(key)) dict[key].Add(value);
            else dict.Add(key, new List<string>() { value });
        }

        System.Random rand = new System.Random();
        List<string> output = new List<string>();
        int n = 0;
        int rn = rand.Next(dict.Count);
        string prefix = dict.Keys.Skip(rn).Take(1).Single();
        output.AddRange(prefix.Split());

        while (true)
        {
            var suffix = dict[prefix];
            if (suffix.Count == 1)
            {
                if (suffix[0] == "")
                {
                    return output.Aggregate(Join);
                }
                output.Add(suffix[0]);
            }
            else
            {
                rn = rand.Next(suffix.Count);
                output.Add(suffix[rn]);
            }
            if (output.Count >= outputSize)
            {
                return output.Take(outputSize).Aggregate(Join);
            }
            n++;
            prefix = output.Skip(n).Take(n_gram).Aggregate(Join);
        }
    }

    public string Markov2() //should return a string
    {
        var rule = MakeRule2();
        var k = rule.ElementAt(UnityEngine.Random.Range(0, rule.Count)).Key;
        var oldWords = rule[k];
        string output = Join(oldWords.ToArray());

        for (int i = 0; i < outputSize; i++)
        {
            var key = Join(oldWords.ToArray());
            var value = rule[key];
            string newWord = value[UnityEngine.Random.Range(0, value.Count)];
            output += newWord + " ";

            for (int j = 0; j < oldWords.Count; j++)
            {
                oldWords[j] = oldWords[(j + 1) % oldWords.Count];
            }
            oldWords[oldWords.Count - 1] = newWord;
        }
        return output;
    }

    Dictionary<string, List<string>> MakeRule2()
    {
        //Make a rule dict for given data
        Dictionary<string, List<string>> rule = new Dictionary<string, List<string>>();

        string body = textFile.text;
        string[] words = body.Split(" ");
        Debug.Log(words.Length);
        

        foreach(string word in words)
        {
            int index = n_gram;
            var strings = new List<string>();
            for (int i = index - n_gram; i < n_gram + 1; i++)
            {
                strings.Add(words[i]);
            }
            if (!rule.ContainsKey(word)) rule.Add(word, strings);
            else rule[word].AddRange(strings);
            index++;
        }

        foreach (KeyValuePair<string, List<string>> pair in rule)
        {
            string s = pair.Key;
            for (int i = 0; i < pair.Value.Count; i++)
            {
                s += " " + pair.Value[i];
            }
            Debug.Log(s);
        }
        return rule;
    }

    Dictionary<string, List<string>> MakeRule()
    {
        //Make a rule dict for given data
        Dictionary<string, List<string>> rule = new Dictionary<string, List<string>>();

        string body = textFile.text;
        string[] words = body.Split(" ");
        Debug.Log(words.Length);
        int index = n_gram;

        foreach(var word in words)
        {
            var strings = new List<string>();
            for (int i = index - n_gram; i < n_gram + 1; i++)
            {
                strings.Add(words[i]);
            }
            var key = Join(strings.ToArray()); //I think? It's possible I'm supposed to join all words

            if (!rule.ContainsKey(key)) rule.Add(key, new List<string>());
            rule[key].Add(word);

            index++;
        }

        foreach(KeyValuePair<string, List<string>> pair in rule)
        {
            string s = pair.Key;
            for (int i = 0; i < pair.Value.Count; i++)
            {
                s += " " + pair.Value[i];
            }
            //Debug.Log(s);
        }
        return rule;
    }

    public TextAsset textFile;

    [ContextMenu("Generate")]
    public void GenerateText()
    {
        var model = MakeMarkovModel();
        Debug.Log(model.Keys.Count);
    }

    private Dictionary<string, Dictionary<string, float>> MakeMarkovModel()
    {
        //pulls in the text from the given file
        string body = textFile.text;
        //splits the text into an array of words
        string[] words = body.Split(" ");
        //Debug.Log(words.Length);
        
        //Created a nested frequency table
        //the Key and Value of the nested dictionary represent how frequently that string follows the Key of the first dictionary
        var markovModel = new Dictionary<string, Dictionary<string, float>>();

        //Loop through all words minus n_gram length - 2
        for (int i = 0; i < words.Length - n_gram - 2; i++)
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
                markovModel.Add(currentState, new Dictionary<string, float>());
                //markovModel[currentState].Add(nextState, 1);
                markovModel[currentState][nextState] = 1;
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

        foreach(var key in markovModel.Keys)
        {
            var member = markovModel[key];
            float total = 0;
            //get the total frequency of this key, taking in 
            foreach(var pair in markovModel[key])
            {
                total += pair.Value;
            }
            //Set the final value for each nested dictionary to its value divided by the total value
            //I can't modify the value
            foreach (var pair in markovModel[key])
            {
                //markovModel[key][pair.Key] = markovModel[key][pair.Key] / total;
                markovModel[key][pair.Key] = pair.Value / total;
            }
        }

        return markovModel;
    }

    /*private private Dictionary<string, Dictionary<string, float>> CopyMarkov()
    {

    }*/
}
