using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    public static MessageSystem instance;
    [SerializeField] private MessageLog log;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        log.ClearLog();
    }

    public static void NewMessage(string message)
    {
        instance.BroadcastNewMessage(message);
    }

    private void BroadcastNewMessage(string message)
    {
        log.Log += "\n" + message;
    }
}