using UnityEngine;
using UnityEditor;
using UnityEditor.Search;

[CustomEditor(typeof(MapDisplay))]
public class MapDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapDisplay display = (MapDisplay)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Clear Info")) display.ClearDisplay();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Heat Map")) display.DisplayHeatMap();
        if (GUILayout.Button("Moisture Map")) display.DisplayMoistureMap();
        if (GUILayout.Button("Wind Map")) display.DisplayWindMap();
        GUILayout.EndHorizontal();


        GUILayout.Space(10);

        display.highlightedBiome = (BiomeTypes)EditorGUILayout.EnumPopup("Highlight Biome", display.highlightedBiome);
        if (GUILayout.Button(BiomeButtonName(display.highlightedBiome))) display.HighlightBiome();

        GUILayout.Space(10);

        GUILayout.Label("Highlight Features");

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Mountains")) display.HighlightMountains();
        if (GUILayout.Button("Islands")) display.HighlightIslands();
        if (GUILayout.Button("Lakes")) display.HighlightLakes();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Settlements")) display.HighlightSettlements();
        if (GUILayout.Button("Coasts")) display.HighlightCoasts();
        if (GUILayout.Button("Tectonic Plates")) display.HighlightTectonicPlates();
        GUILayout.EndHorizontal();
    }

    private string BiomeButtonName(BiomeTypes biome)
    {
        string s = "Find";
        var strings = SearchUtils.SplitCamelCase(biome.ToString());
        for (int i = 0; i < strings.Length; i++)
        {
            s += " " + strings[i];
        }
        s += "s";
        return s;
    }
}
