using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator mapGen = (MapGenerator)target;

        //if (mapGen.AutoUpdate) mapGen.GenerateMap();

        //if (GUILayout.Button("Generate")) mapGen.GenerateMap();
        if (GUILayout.Button("Random Seed")) mapGen.SetRandomSeed();

    }
}
