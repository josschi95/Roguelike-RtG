using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private static CombatSystem instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }


}
