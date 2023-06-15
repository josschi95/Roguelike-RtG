using UnityEngine;
using UnityEngine.UI;
using JS.ECS;
using TMPro;

public class UICharacterMenu : MonoBehaviour
{
    [Header("Name/Race/Level")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text raceText;
    [SerializeField] private TMP_Text levelText;

    [Header("Name/Race/Level")]
    [SerializeField] private TMP_Text HP;
    [SerializeField] private TMP_Text SP;
    [SerializeField] private TMP_Text MP;
    [SerializeField] private Image HPBar, SPBar, MPBar;
    [SerializeField] private GameObject ManaDisplay;

    [Header("Attributes")]
    [SerializeField] private TMP_Text STR;
    [SerializeField] private TMP_Text AGI, VIT, KNO, WIL, CHA;


    [Header("Miscellaneous")]
    [SerializeField] private TMP_Text SPD;
    [SerializeField] private TMP_Text AV, DV;
    [SerializeField] private TMP_Text Hunger, Thirst;
    [SerializeField] private TMP_Text Deity, Piety;
    [SerializeField] private TMP_Text WALK, SWIM, FLY;
    [SerializeField] private GameObject stomachParent;

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
        UpdateValues();
    }

    private void UpdateValues()
    {
        nameText.text = Player.Name;

        EntityManager.TryGetStat(Player, "HP", out var hp);
        EntityManager.TryGetStat(Player, "SP", out var sp);
        ManaDisplay.SetActive(EntityManager.TryGetStat(Player, "MP", out var mp));
        EntityManager.TryGetStat(Player, "AV", out var av);
        EntityManager.TryGetStat(Player, "DV", out var dv);

        EntityManager.TryGetStat(Player, "STR", out var str);
        EntityManager.TryGetStat(Player, "AGI", out var agi);
        EntityManager.TryGetStat(Player, "VIT", out var vit);
        EntityManager.TryGetStat(Player, "KNO", out var kno);
        EntityManager.TryGetStat(Player, "WIL", out var wil);
        EntityManager.TryGetStat(Player, "CHA", out var cha);

        EntityManager.TryGetStat(Player, "SPD", out var spd);
        EntityManager.TryGetStat(Player, "WALK", out var walk);
        EntityManager.TryGetStat(Player, "SWIM", out var swim);
        EntityManager.TryGetStat(Player, "FLY", out var fly);

        HP.text = "HP: " + hp.CurrentValue.ToString() + "/" + hp.Value.ToString();
        SP.text = "SP: " + sp.CurrentValue.ToString() + "/" + sp.Value.ToString();
        //Characters without the aptitude to cast magic simply don't have mana
        if (mp != null) MP.text = "MP: " + mp.CurrentValue.ToString() + "/" + mp.Value.ToString();
        
        STR.text = str.Value.ToString();
        AGI.text = agi.Value.ToString();
        VIT.text = vit.Value.ToString();
        KNO.text = kno.Value.ToString();
        WIL.text = wil.Value.ToString();
        CHA.text = cha.Value.ToString();

        AV.text = "AV: " + av.Value.ToString();
        DV.text = "DV: " + dv.Value.ToString();

        SPD.text = "SPD: " + spd.Value.ToString();
        WALK.text = "WALK: " + walk.Value.ToString();
        SWIM.text = "SWIM: " + swim.Value.ToString();
        FLY.text = "FLY: " + fly.Value.ToString();

        if (EntityManager.TryGetComponent<Stomach>(Player, out var stomach))
        {
            stomachParent.SetActive(true);
            Hunger.text = stomach.Hunger.ToString();
            Thirst.text = stomach.Thirst.ToString();
        }
        else stomachParent.SetActive(false);
    }
}
