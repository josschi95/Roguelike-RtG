using UnityEngine;
using UnityEditor;

namespace JS.DomainSystem
{
    [CustomEditor(typeof(DomainDatabase))]
    public class DomainDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();

            //GUILayout.Label("XML File");

            DomainDatabase database = (DomainDatabase)target;

            EditorGUI.BeginChangeCheck();
            var file = EditorGUILayout.ObjectField("Domain Database XML", database.XMLRawFile, typeof(TextAsset), false);

            if (EditorGUI.EndChangeCheck())
            {
                database.XMLRawFile = file as TextAsset;
            }

            if (GUILayout.Button("Load XML")) database.LoadXMLFile();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            base.OnInspectorGUI();
        }
    }
}