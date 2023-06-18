using JS.ECS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryMenu : MonoBehaviour
{
    [System.Serializable]
    private class InventoryCategory
    {
        public string name;
        public string defaultTitle;
        public Button button;
        public TMP_Text header;
        public bool isShown;
    }

    [SerializeField] private UISelectionElement element;
    [SerializeField] private RectTransform contentRect;

    [SerializeField] private InventoryCategory[] categories;

    [SerializeField] private Button showAll, hideAll;

    [Space]
    [SerializeField] private GameObject contextMenu;

    private void OnEnable()
    {
        SetButtons();
        PopulateTable();
    }

    private void OnDisable()
    {
        ClearButtons();
        ClearTable();
    }

    private void SetButtons()
    {
        for (int i = 0; i < categories.Length; i++)
        {
            int index = i;
            categories[index].button.onClick.AddListener(delegate
            {
                categories[index].isShown = !categories[index].isShown;
                FoldCategory(categories[index].button.transform.parent, categories[index].isShown);
            });
        }
        showAll.onClick.AddListener(ShowAll);
        hideAll.onClick.AddListener(CollapseAll);
    }

    private void ClearButtons()
    {
        for (int i = 0; i < categories.Length; i++) 
        {
            categories[i].button.onClick.RemoveAllListeners();
        }
        showAll.onClick.RemoveAllListeners();
        hideAll.onClick.RemoveAllListeners();
    }

    private void PopulateTable()
    {
        EntityManager.TryGetComponent<Inventory>(EntityManager.Player, out var inventory);

        foreach (var item in inventory.Contents)
        {
            var newElement = Instantiate(element, FindCategory(item));
            newElement.gameObject.SetActive(ShowElement(item));

            newElement.Text.text = item.Name;
            newElement.Button.onClick.AddListener(delegate
            {
                Debug.Log("selecting " +  item.Name);
                contextMenu.SetActive(true);
            });
        }

        foreach(var category in categories)
        {
            var parent = category.button.transform.parent;
            //Enable/disable category if it is empty or has children
            if (parent.childCount - 1 <= 0) parent.gameObject.SetActive(false);
            else parent.gameObject.SetActive(true);

            //change the collapse indicator based on if it is shown or not
            if (category.isShown) category.header.text = "[-] ";
            else category.header.text = "[+] ";
            category.header.text += category.defaultTitle;

            //Add an indicator for how many items are in that category
            category.header.text += " (" + (parent.childCount - 1) + ")";
        }
    }

    private void ClearTable()
    {
        for (int i = 0; i < categories.Length; i++)
        {
            var parent = categories[i].button.transform.parent;
            for (int j = parent.transform.childCount - 1; j >= 1; j--)
            {
                Destroy(parent.transform.GetChild(j).gameObject);
            }
        }
    }

    private UnityEngine.Transform FindCategory(Entity item)
    {
        EntityManager.TryGetComponent<JS.ECS.Physics>(item, out var phys);
        if (phys == null) return categories[categories.Length - 1].button.transform.parent;

        for (int i = 0; i < categories.Length; i++)
        {
            if (phys.Category.Equals(categories[i].name))
                return categories[i].button.transform.parent;
        }
        return categories[categories.Length - 1].button.transform.parent;
    }

    private bool ShowElement(Entity item)
    {
        EntityManager.TryGetComponent<JS.ECS.Physics>(item, out var phys);
        if (phys == null) return categories[categories.Length - 1].isShown;

        for (int i = 0; i < categories.Length; i++)
        {
            if (phys.Category.Equals(categories[i].name))
                return categories[i].isShown;
        }
        return categories[categories.Length - 1].isShown;

        /*return phys.Category switch
        {
            "Projectile" => showAmmo,
            "Armor" => showArmor,
            "Book" => showBooks,
            "Corpse" => showCorpse,
            "Food" => showFood,
            "Melee Weapon" => showMelee,
            "Missile Weapon" => showMissile,
            "Potion" => showPotion,
            "Scroll" => showScrolls,
            "Shield" => showShield,
            "Tool" => showTool,
            "Valuable" => showValuables,
            _ => showMisc,
        };*/
    }

    private void ShowAll()
    {
        for (int i = 0; i < categories.Length; i++)
        {
            int index = i;
            if (!categories[i].isShown) 
            {
                categories[i].isShown = true;
                FoldCategory(categories[i].button.transform.parent, true);
            }
        }

        /*if (!showAmmo) ToggleAmmo();
        if (!showArmor) ToggleArmor();
        if (!showBooks) ToggleBooks();
        if (!showCorpse) ToggleCorpse();
        if (!showFood) ToggleFood();
        if (!showMelee) ToggleMelee();
        if (!showMissile) ToggleMissile();
        if (!showPotion) TogglePotion();
        if (!showScrolls) ToggleScrolls();
        if (!showShield) ToggleShield();
        if (!showTool) ToggleTool();
        if (!showValuables) ToggleValuables();*/
    }

    private void CollapseAll()
    {
        for (int i = 0; i < categories.Length; i++)
        {
            int index = i;
            if (categories[i].isShown)
            {
                categories[i].isShown = false;
                FoldCategory(categories[i].button.transform.parent, false);
            }
        }
    }

    private void FoldCategory(UnityEngine.Transform parent, bool show)
    {
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(show);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }
}