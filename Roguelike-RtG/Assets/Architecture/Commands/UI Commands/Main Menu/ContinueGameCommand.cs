using JS.Architecture.EventSystem;
using JS.Architecture.Primitives;
using JS.World.Map;
using UnityEngine;

namespace JS.Architecture.CommandSystem
{
    /// <summary>
    /// Checks if the world map file has been loaded with save data and loads world map or world gen as appropriate
    /// </summary>
    [CreateAssetMenu(menuName = "Command System/Continue Game Command")]
    public class ContinueGameCommand : CommandBase
    {
        [Space] [Space]

        [SerializeField] private WorldData worldData;
        [SerializeField] private GameEvent loadPlayerSavedSceneCommand; //placeholder, for now this will just load in world map, //but obviously later it will take in player's grid coordinates and load appropriate scene/map
        [SerializeField] private GameEvent loadWorldGenScene; //placeholder, for now this will just load in world map, //but obviously later it will take in player's grid coordinates and load appropriate scene/map

        [Space]
        
        [SerializeField] private BoolVariable AutoGenSavedWorld;

        protected override bool ExecuteCommand()
        {
            return CheckForWorldLoaded();
        }

        private bool CheckForWorldLoaded()
        {
            if (!worldData.SaveExists) return false;

            if (worldData.IsLoaded)
            {
                //Debug.Log("IsLoaded");
                //All save data is already loaded in, can go directly to next scene
                AutoGenSavedWorld.Value = false;
                loadPlayerSavedSceneCommand?.Invoke();
                return true;
            }
            else
            {
                //Debug.Log("IsNotLoaded");
                //The save data has not been loaded in, need to first go to world gen to recreate it
                AutoGenSavedWorld.Value = true;
                loadWorldGenScene?.Invoke();
            }

            return true;
        }
    }
}