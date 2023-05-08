using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Sub Race")]
    public class CharacterSubRace : ScriptableObject
    {
        [field: SerializeField] public string SubRaceName { get; private set; }

        [field: SerializeField] public CharacterRace ParentRace { get; private set; }

        [field: SerializeField] public SizeCategory SizeOverride { get; private set; }

        [field: SerializeField] public StatField Stats { get; private set; }
    }
}