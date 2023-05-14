using UnityEngine;
using UnityEditor;

namespace JS.CharacterSystem
{
    [CustomEditor(typeof(LifeExpectancy))]
    public class LifeExpectancyEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI ();

            LifeExpectancy lifeExpectancy = (LifeExpectancy)target;
            GUIStyle fixedWidth = new GUIStyle(GUI.skin.label)
            {
                fixedWidth = 200,
            };

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Minimum Life Expectancy", fixedWidth);
            GUILayout.Label(lifeExpectancy.MinLifeExpectancy.ToString());
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Maximum Life Expectancy", fixedWidth);
            GUILayout.Label(lifeExpectancy.MaxLifeExpectancy.ToString());
            GUILayout.Space(100);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Child Age", fixedWidth);
            GUILayout.Label(lifeExpectancy.ChildAge.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Adolescent Age", fixedWidth);
            GUILayout.Label(lifeExpectancy.AdolescentAge.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Young Adult Age", fixedWidth);
            GUILayout.Label(lifeExpectancy.YoungAdultAge.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Middle Age", fixedWidth);
            GUILayout.Label(lifeExpectancy.MiddleAge.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Old Age", fixedWidth);
            GUILayout.Label(lifeExpectancy.OldAge.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Venerable Age", fixedWidth);
            GUILayout.Label(lifeExpectancy.VenerableAge.ToString());
            GUILayout.EndHorizontal();
        }
    }
}