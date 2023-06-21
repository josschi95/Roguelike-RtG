using UnityEngine;
using JS.ECS;

public class UIEquipmentMenu : MonoBehaviour
{
    [SerializeField] private UIInventoryMenu inventoryMenu;
    [SerializeField] private RectTransform elementParent;
    [SerializeField] private UIEquipSlotElement element;

    private void OnEnable() => PopulateList();
    private void OnDisable() => ClearList();

    public void RefreshDisplay()
    {
        ClearList();
        PopulateList();
    }

    private void ClearList()
    {
        for (int i = elementParent.childCount - 1; i >= 0; i--)
        {
            Destroy(elementParent.GetChild(i).gameObject);
        }
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
                OnHandSelected(grasper);
            });
        }

        //Missile Weapon
        var missile = Instantiate(element, elementParent);
        missile.slotText.text = "Missile Weapon";

        if (body.MissileWeapon != null) missile.itemText.text = body.MissileWeapon.Name;
        else missile.itemText.text = "-";

        missile.Button.onClick.AddListener(delegate
        {
            OnMissilesSelected(body, body.MissileWeapon);
        });

        //Projectiles
        var projectile = Instantiate(element, elementParent);
        projectile.slotText.text = "Ammunition";

        if (body.Projectiles != null) projectile.itemText.text = body.Projectiles.Name;
        else projectile.itemText.text = "-";

        projectile.Button.onClick.AddListener(delegate
        {
            OnProjectilesSelected(body, body.Projectiles);
        });

        //Armor
        for (int i = 0; i < body.ArmorSlots.Count; i++)
        {
            var slot = body.ArmorSlots[i];

            var newElement = Instantiate(element, elementParent);
            newElement.slotText.text = slot.Slot.ToString();

            if (slot.Armor != null) newElement.itemText.text = slot.Armor.Name;
            else newElement.itemText.text = "-";

            newElement.Button.onClick.AddListener(delegate
            {
                OnArmorSelected(body, slot);
            });
        }
    }

    private void OnHandSelected(BodyPart bodyPart)
    {
        //Debug.Log("Selecting " + bodyPart.Laterality.ToString() + " " + bodyPart.Name + " slot");
        if (bodyPart.HeldItem != null)
        {
            BodySystem.UnequipItem(EntityManager.Player, bodyPart);
            inventoryMenu.RefreshDisplay();
            RefreshDisplay();
        }
        else
        {
            //Bring up display of all equipment which is valid for this slot
            //If there is none, show a popup message saying so
        }
    }

    private void OnMissilesSelected(Body body, Entity missiles)
    {
        //Debug.Log("Selecting Missile slot");
        if (missiles != null)
        {
            BodySystem.UnequipRanged(body, missiles);
            inventoryMenu.RefreshDisplay();
            RefreshDisplay();
        }
        else
        {
            //Bring up display of all equipment which is valid for this slot
            //If there is none, show a popup message saying so
        }
    }

    private void OnProjectilesSelected(Body body, Entity projectiles)
    {
        //Debug.Log("Selecting Projectile slot");
        if (projectiles != null)
        {
            BodySystem.UnequipRanged(body, projectiles);
            inventoryMenu.RefreshDisplay();
            RefreshDisplay();
        }
        else
        {
            //Bring up display of all equipment which is valid for this slot
            //If there is none, show a popup message saying so
        }
    }

    private void OnArmorSelected(Body body, ArmorSlot slot)
    {
        //Debug.Log("Selecting " + slot.Slot.ToString() + " slot");
        if (slot.Armor != null) //Unequip armor in that slot
        {
            BodySystem.UnequipArmor(body, slot);
            inventoryMenu.RefreshDisplay();
            RefreshDisplay();
        }
        else
        {
            //Bring up display of all equipment which is valid for this slot
            //If there is none, show a popup message saying so
        }
    }
}
