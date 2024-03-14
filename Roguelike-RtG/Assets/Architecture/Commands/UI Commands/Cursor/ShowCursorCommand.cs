using UnityEngine;

namespace JS.Architecture.CommandSystem
{
    /// <summary>
    /// A general command that triggers events but doesn't have specific functionality of its own.
    /// </summary>
    [CreateAssetMenu(menuName = "Command System/Cursor/Show Cursor Command")]
    public class ShowCursorCommand : CommandBase
    {
        protected override bool ExecuteCommand()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return true;
        }
    }
}