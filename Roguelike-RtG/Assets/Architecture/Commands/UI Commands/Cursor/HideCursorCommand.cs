using UnityEngine;

namespace JS.Architecture.CommandSystem
{
    /// <summary>
    /// A general command that triggers events but doesn't have specific functionality of its own.
    /// </summary>
    [CreateAssetMenu(menuName = "Command System/Cursor/Hide Cursor Command")]
    public class HideCursorCommand : CommandBase
    {
        protected override bool ExecuteCommand()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            return true;
        }
    }
}