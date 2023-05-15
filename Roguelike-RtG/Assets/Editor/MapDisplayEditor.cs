using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapDisplay))]
public class MapDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapDisplay display = (MapDisplay)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Clear Info")) display.ClearInfoMap();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Heat Map")) display.DisplayHeatMap();
        if (GUILayout.Button("Moisture Map")) display.DisplayMoistureMap();
        if (GUILayout.Button("Wind Map")) display.DisplayWindMap();
        GUILayout.EndHorizontal();


        GUILayout.Space(10);

        int index = EditorGUILayout.Popup("Highlight Biome", BiomeIndex(), BiomeDisplayOptions());
        display.biomeToHighlight = display.biomes[index];

        if (GUILayout.Button("Find " + display.biomeToHighlight.name.Replace("_", " ") + "s")) display.HighlightBiome();

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

    private int BiomeIndex()
    {
        MapDisplay display = (MapDisplay)target;
        for (int i = 0; i < display.biomes.Length; i++)
        {
            if (display.biomeToHighlight == display.biomes[i]) return i;
        }
        return 0;
    }

    private string[] BiomeDisplayOptions()
    {
        MapDisplay display = (MapDisplay)target;
        var options = new string[display.biomes.Length];
        for (int i = 0; i < options.Length; i++)
        {
            options[i] = display.biomes[i].name;
        }
        return options;
    }
}