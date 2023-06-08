using JS.EventSystem;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable Objects/Message Log")]
public class MessageLog : ScriptableObject
{
    [TextArea(3,20)]
    [SerializeField] private string log;
    [SerializeField] private GameEvent messageLogChangeEvent;

    public string Log
    {
        get => log;
        set
        {
            log = value;
            messageLogChangeEvent?.Invoke();
        }
    }

    public void ClearLog()
    {
        log = string.Empty;
        messageLogChangeEvent?.Invoke();
    }
}
