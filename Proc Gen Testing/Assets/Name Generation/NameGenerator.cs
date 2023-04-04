using UnityEngine;

[CreateAssetMenu(fileName = "Name Generator", menuName = "Scriptable Objects/Name Generator")]
public class NameGenerator : ScriptableObject
{
    [SerializeField] private string[] baseWords;
}