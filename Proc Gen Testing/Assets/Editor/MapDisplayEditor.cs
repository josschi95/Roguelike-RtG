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
        if (GUILayout.Button("Heat Map")) display.DisplayHeatMap();
        if (GUILayout.Button("Moisture Map")) display.DisplayMoistureMap();
        if (GUILayout.Button("Wind Map")) display.DisplayWindMap();

        GUILayout.Space(10);

        display.highlightedBiome = (BiomeTypes)EditorGUILayout.EnumPopup("Highlight Biome", display.highlightedBiome);
        if (GUILayout.Button(BiomeButtonName(display.highlightedBiome))) display.HighlightBiome();
        if (GUILayout.Button("Highlight Mountains")) display.HighlightMountains();
        if (GUILayout.Button("Highlight Tectonic Plates")) display.HighlightTectonicPlates();

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
