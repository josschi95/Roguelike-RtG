using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MarkovChainNames))]
public class MarkovNameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);

        MarkovChainNames markov = (MarkovChainNames)target;

        if (GUILayout.Button("Generate Models")) markov.GenerateModels();
        GUILayout.Space(10);
        if (GUILayout.Button("Get Name")) markov.GetName();
        if (GUILayout.Button("Get Names")) markov.GetNames();
    }
}
