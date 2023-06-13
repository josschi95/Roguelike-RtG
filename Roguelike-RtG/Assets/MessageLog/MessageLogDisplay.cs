using UnityEngine;
using TMPro;

public class MessageLogDisplay : MonoBehaviour
{
    [SerializeField] private MessageLog log;
    [SerializeField] private TMP_Text displayText;

    //Called from GameEventListener attached to same object
    public void UpdateLogDisplay()
    {
        displayText.text = log.Log;
    }
}
