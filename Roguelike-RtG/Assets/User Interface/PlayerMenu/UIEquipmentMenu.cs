using UnityEngine;
using JS.ECS;

public class UIEquipmentMenu : MonoBehaviour
{
    [SerializeField] private RectTransform elementParent;
    [SerializeField] private UIEquipSlotElement element;

    private void OnEnable() => PopulateList();
    private void OnDisable() => ClearList();

    public void RefreshDisplay()
    {
        ClearList();
        PopulateList();
    }

    //Selecting a slot will either unequip an equipped item or bringup a menu of items which can be equipped there
    private void PopulateList()
    {
        EntityManager.TryGetComponent<Body>(EntityManager.Player, out var body);

        //Hands
        foreach(var grasper in body.BodyParts)
        {
            if (!grasper.IsGrasper) continue;
            var newElement = Instantiate(element, elementParent);

            newElement.slotText.text = grasper.Laterality.ToString() + " " + grasper.Name;
            if (grasper.HeldItem != null) newElement.itemText.text = grasper.HeldItem.Name;
            else newElement.itemText.text = grasper.DefaultBehavior.Name;

            newElement.Button.onClick.AddListener(delegate
            {
                Debug.Log("Selecting " + grasper.Laterality.ToString() + " " + grasper.Name + " slot");
            });
        }
        //Missile Weapon
        var missile = Instantiate(element, elementParent);
        missile.slotText.text = "Missile Weapon";
        if (body.MissileWeapon != null) missile.itemText.text = body.MissileWeapon.Name;
        else missile.itemText.text = "-";
        missile.Button.onClick.AddListener(delegate
        {
            Debug.Log("Selecting Missile slot");
        });

        //Projectiles
        var projectile = Instantiate(element, elementParent);
        projectile.slotText.text = "Ammunition";
        if (body.Projectiles != null) projectile.itemText.text = body.Projectiles.Name;
        else projectile.itemText.text = "-";
        projectile.Button.onClick.AddListener(delegate
        {
            Debug.Log("Selecting Projectile slot");
        });

        //Armor
        for (int i = 0; i < body.ArmorSlots.Count; i++)
        {
            var newElement = Instantiate(element, elementParent);
            newElement.slotText.text = body.ArmorSlots[i].Slot.ToString();
            if (body.ArmorSlots[i].Armor != null) newElement.itemText.text = body.ArmorSlots[i].Armor.Name;
            else newElement.itemText.text = "-";

            newElement.Button.onClick.AddListener(delegate
            {
                Debug.Log("Selecting " + body.ArmorSlots[i].Slot.ToString() + " slot");
                BodySystem.UnequipArmor(body.ArmorSlots[i]);
            });
        }
    }

    private void ClearList()
    {
        for (int i = elementParent.childCount - 1; i >= 0; i--)
        {
            Destroy(elementParent.GetChild(i).gameObject);
        }
    }
}
