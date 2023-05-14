using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Class Tier")]
    public class ClassTier : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
    }
}