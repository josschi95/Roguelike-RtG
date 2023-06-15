using System.Collections.Generic;
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

    public static void NewHitMessage(string attacker, string target, Dictionary<string, int> Damage)
    {
        string log = attacker + " hit " + target + " for ";
        foreach(var pair in Damage)
        {
            log += pair.Value + " " + pair.Key + " damage,";
        }
        log.TrimEnd();
        NewMessage(log);
    }
}