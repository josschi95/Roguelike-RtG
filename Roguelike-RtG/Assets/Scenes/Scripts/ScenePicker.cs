using UnityEngine;
using JS.Architecture.EventSystem;

namespace JS.Architecture.SceneManagement
{
    [CreateAssetMenu(menuName = "Scene Management/Scene Picker")]
    public class ScenePicker : ScriptableObject
    {
        /// <summary>
        /// Event to call after scene has finished loading.
        /// </summary>
        public GameEvent SceneFinishedLoadingEvent;

        [HideInInspector] public string ScenePath;
    }
}