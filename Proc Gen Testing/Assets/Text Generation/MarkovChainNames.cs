using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

public class MarkovChainNames : MonoBehaviour
{
    [SerializeField] private int syllables = 2;
    [SerializeField] private bool cleanText;
    [Space]

    [SerializeField] private TextAsset[] textFiles;
    [SerializeField] private bool debugDictionary;

    private string[] charactersToRemove = { "[", "]", "{", "}", "(", ")", ".", ",", "!", "?", ";", ":", " " };
    private string vowels = "aeiouy";

    [ContextMenu("Generate")]
    public void Generate()
    {
        Debug.Log(GetName());
    }

    private string GetName()
    {
        var markovModel = MakeMarkovModel();
        int n = 0;

        string currentState = markovModel.ElementAt(Random.Range(0, markovModel.Count)).Key;
        string nextState;
        string output = "";
        output += currentState;
        output = output.Trim();

        while(n < syllables)
        {
            if (!markovModel.ContainsKey(currentState)) break;

            int index = Random.Range(0, markovModel[currentState].Count);
            nextState = markovModel[currentState][index];
            nextState = nextState.Trim();
            output += nextState;

            currentState = nextState;

            n++;
        }

        return output.Replace(" ", "");
    }

    private Dictionary<string, List<string>> MakeMarkovModel()
    {
        string[] names = GetText();

        var markovModel = new Dictionary<string, List<string>>();

        for (int i = 0; i < names.Length; i++)
        {
            var syllables = GetSyllables(names[i]);
            if (syllables.Count <= 1)
            {
                Debug.Log("Removing " + names[i]);
                continue;
            }

            for (int j = 0; j < syllables.Count - 1; j++)
            {
                string prefix = syllables[j];
                string suffix = syllables[j + 1];

                if (!markovModel.ContainsKey(prefix))
                {
                    markovModel[prefix] = new List<string>();
                }
                markovModel[prefix].Add(suffix);
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
        string[] names = body.Trim().Split(" ");

        //Debug.Log(words.Length);

        return names;
    }

    private string[] GetSyllables()
    {
        var names = GetText();
        var list = new List<string>();

        for (int i = 0; i < names.Length; i++)
        {
            list.AddRange(GetSyllables(names[i]));
        }

        return list.ToArray();
    }

    private List<string> GetSyllables(string word)
    {
        word = word.ToLower().Trim();
        var syllableList = new List<string>();
        bool lastWasVowel = false;

        //StringBuilder currSyllable = new StringBuilder();
        string currSyllable = "";
        foreach (char c in word)
        {
            if (vowels.Contains(c))
            {
                if (!lastWasVowel)
                {
                    lastWasVowel = true;

                    // Finish this syllable and add to the list
                    //syllableList.Add(currSyllable.ToString());
                    syllableList.Add(currSyllable);
                    currSyllable = "";//currSyllable.Clear();
                }
            }
            else
            {
                lastWasVowel = false;
            }

            // Add this character to the current syllable
            currSyllable += c;
            //currSyllable.Append(c);
        }

        if ((word.EndsWith("e") || (word.EndsWith("es") || word.EndsWith("ed"))) && !word.EndsWith("le"))
        {
            // Remove the last syllable?
            //syllableList.RemoveAt(syllableList.Count - 1);
        }

        for (int i = syllableList.Count - 1; i >= 0; i--)
        {
            syllableList[i] = syllableList[i].Trim();
            if (syllableList[i].Length <= 1) syllableList.RemoveAt(i);
        }

        return syllableList;
    }

}
