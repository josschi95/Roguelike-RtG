using JS.ECS;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPlayerMenu : MonoBehaviour
{
    [SerializeField] private InputActionProperty tab;
    [SerializeField] private GameObject menuPanel;
    private bool menuOpen = false;

    [Space]

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text HP, SP, MP, AV, DV;
    [SerializeField] private TMP_Text STR, AGI, VIT, KNO, WIL, CHA;
    [SerializeField] private TMP_Text SPD, AP, WALK, SWIM, FLY;

    [SerializeField] private TMP_Text Hunger, Thirst;

    private Entity playerEntity;
    private Entity Player
    {
        get
        {
            if (playerEntity == null)
            {
                playerEntity = EntityManager.Player;
            }
            return playerEntity;
        }
    }

    private void OnEnable()
    {
        tab.action.performed += ToggleMenu;   
    }

    private void OnDisable()
    {
        tab.action.performed -= ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext obj)
    {
        if (menuOpen) CloseMenu();
        else OpenMenu();
    }

    private void OpenMenu()
    {
        menuOpen = true;
        UpdateValues();
        menuPanel.SetActive(true);
    }

    private void CloseMenu()
    {
        menuOpen = false;
        menuPanel.SetActive(false);
    }

    private void UpdateValues()
    {
        nameText.text = Player.Name;

        EntityManager.TryGetStat(Player, "HP", out var hp);
        EntityManager.TryGetStat(Player, "SP", out var sp);
        EntityManager.TryGetStat(Player, "MP", out var mp);
        EntityManager.TryGetStat(Player, "AV", out var av);
        EntityManager.TryGetStat(Player, "DV", out var dv);

        EntityManager.TryGetStat(Player, "STR", out var str);
        EntityManager.TryGetStat(Player, "AGI", out var agi);
        EntityManager.TryGetStat(Player, "VIT", out var vit);
        EntityManager.TryGetStat(Player, "KNO", out var kno);
        EntityManager.TryGetStat(Player, "WIL", out var wil);
        EntityManager.TryGetStat(Player, "CHA", out var cha);

        EntityManager.TryGetStat(Player, "SPD", out var spd);
        EntityManager.TryGetStat(Player, "AP", out var ap);
        EntityManager.TryGetStat(Player, "WALK", out var walk);
        EntityManager.TryGetStat(Player, "SWIM", out var swim);
        EntityManager.TryGetStat(Player, "FLY", out var fly);

        HP.text = "HP: " + hp.Value.ToString();
        SP.text = "SP: " + sp.Value.ToString();
        MP.text = "MP: " + mp.Value.ToString();
        AV.text = "AV: " + av.Value.ToString();
        DV.text = "DV: " + dv.Value.ToString();

        STR.text = "STR: " + str.Value.ToString();
        AGI.text = "AGI: " + agi.Value.ToString();
        VIT.text = "VIT: " + vit.Value.ToString();
        KNO.text = "KNO: " + kno.Value.ToString();
        WIL.text = "WIL: " + wil.Value.ToString();
        CHA.text = "CHA: " + cha.Value.ToString();

        SPD.text = "SPD: " + spd.Value.ToString();
        AP.text = "AP: " + ap.Value.ToString();
        WALK.text = "WALK: " + walk.Value.ToString();
        SWIM.text = "SWIM: " + swim.Value.ToString();
        FLY.text = "FLY: " + fly.Value.ToString();

        if (EntityManager.TryGetComponent<Stomach>(Player, out var stomach))
        {
            Hunger.text = stomach.Hunger.ToString();
            Thirst.text = stomach.Thirst.ToString();
        }
        else
        {
            Hunger.text = "";
            Thirst.text = "";
        }
    }
}
