using JS.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryMenu : MonoBehaviour
{
    [SerializeField] private UISelectionElement element;
    [SerializeField] private ContentSizeFitter fitter;

    [SerializeField]
    private Button ammoButton, armorButton, booksButton, corpseButton, foodButton, meleeButton, missileButton, 
        potionButton, scrollsButton, shieldButton, toolButton, valuablesButton, miscButton;

    private bool showAmmo, showArmor, showBooks, showCorpse, showFood, showMelee, showMissile, showPotion, showScrolls,
        showShield, showTool, showValuables, showMisc;

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
        ammoButton.onClick.AddListener(ToggleAmmo);
        armorButton.onClick.AddListener(ToggleArmor);
        booksButton.onClick.AddListener(ToggleBooks);
        corpseButton.onClick.AddListener(ToggleCorpse);
        foodButton.onClick.AddListener(ToggleFood);
        meleeButton.onClick.AddListener(ToggleMelee);
        missileButton.onClick.AddListener(ToggleMissile);
        potionButton.onClick.AddListener(TogglePotion);
        scrollsButton.onClick.AddListener(ToggleScrolls);
        shieldButton.onClick.AddListener(ToggleShield);
        toolButton.onClick.AddListener(ToggleTool);
        valuablesButton.onClick.AddListener(ToggleValuables);
        miscButton.onClick.AddListener(ToggleMisc);
    }

    private void ClearButtons()
    {
        ammoButton.onClick.RemoveAllListeners();
        armorButton.onClick.RemoveAllListeners();
        booksButton.onClick.RemoveAllListeners();
        corpseButton.onClick.RemoveAllListeners();
        foodButton.onClick.RemoveAllListeners();
        meleeButton.onClick.RemoveAllListeners();
        missileButton.onClick.RemoveAllListeners();
        potionButton.onClick.RemoveAllListeners();
        scrollsButton.onClick.RemoveAllListeners();
        shieldButton.onClick.RemoveAllListeners();
        toolButton.onClick.RemoveAllListeners();
        valuablesButton.onClick.RemoveAllListeners();
        miscButton.onClick.RemoveAllListeners();
    }

    private void PopulateTable()
    {
        EntityManager.TryGetComponent<Inventory>(EntityManager.Player, out var inventory);

        foreach (var item in inventory.Contents)
        {
            var newElement = Instantiate(element, FindCategory(item));
            //Also add a check in here to see whether to set it active or not
            newElement.Text.text = item.Name;
            newElement.Button.onClick.AddListener(delegate
            {
                Debug.Log("selecting " +  item.Name);
            });
        }
    }

    private void ClearTable()
    {
        var ammo = ammoButton.transform.parent;
        for (int i = ammo.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(ammo.transform.GetChild(i).gameObject);
        }
        var armor = armorButton.transform.parent;
        for (int i = armor.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(armor.transform.GetChild(i).gameObject);
        }
        var book = booksButton.transform.parent;
        for (int i = book.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(book.transform.GetChild(i).gameObject);
        }
        var corpse = corpseButton.transform.parent;
        for (int i = corpse.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(corpse.transform.GetChild(i).gameObject);
        }
        var food = foodButton.transform.parent;
        for (int i = food.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(food.transform.GetChild(i).gameObject);
        }
        var melee = meleeButton.transform.parent;
        for (int i = melee.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(melee.transform.GetChild(i).gameObject);
        }
        var missile = missileButton.transform.parent;
        for (int i = missile.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(missile.transform.GetChild(i).gameObject);
        }
        var potion = potionButton.transform.parent;
        for (int i = potion.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(potion.transform.GetChild(i).gameObject);
        }
        var scroll = scrollsButton.transform.parent;
        for (int i = scroll.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(scroll.transform.GetChild(i).gameObject);
        }
        var shield = shieldButton.transform.parent;
        for (int i = shield.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(shield.transform.GetChild(i).gameObject);
        }
        var tool = toolButton.transform.parent;
        for (int i = tool.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(tool.transform.GetChild(i).gameObject);
        }
        var valuable = valuablesButton.transform.parent;
        for (int i = valuable.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(valuable.transform.GetChild(i).gameObject);
        }
        var misc = miscButton.transform.parent;
        for (int i = misc.transform.childCount - 1; i >= 1; i--)
        {
            Destroy(misc.transform.GetChild(i).gameObject);
        }
    }

    private UnityEngine.Transform FindCategory(Entity item)
    {
        EntityManager.TryGetComponent<JS.ECS.Physics>(item, out var phys);
        if (phys == null) return miscButton.transform.parent;

        return phys.Category switch
        {
            "Projectile" => ammoButton.transform.parent,
            "Armor" => armorButton.transform.parent,
            "Book" => booksButton.transform.parent,
            "Corpse" => corpseButton.transform.parent,
            "Food" => foodButton.transform.parent,
            "Melee Weapon" => meleeButton.transform.parent,
            "Missile Weapon" => missileButton.transform.parent,
            "Potion" => potionButton.transform.parent,
            "Scroll" => scrollsButton.transform.parent,
            "Shield" => shieldButton.transform.parent,
            "Tool" => toolButton.transform.parent,
            "Valuable" => valuablesButton.transform.parent,
            _ => miscButton.transform.parent,
        };
    }

    private void ShowAll()
    {
        if (!showAmmo) ToggleAmmo();
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
        if (!showValuables) ToggleValuables();
    }

    private void CollapseAll()
    {
        if (showAmmo) ToggleAmmo();
        if (showArmor) ToggleArmor();
        if (showBooks) ToggleBooks();
        if (showCorpse) ToggleCorpse();
        if (showFood) ToggleFood();
        if (showMelee) ToggleMelee();
        if (showMissile) ToggleMissile();
        if (showPotion) TogglePotion();
        if (showScrolls) ToggleScrolls();
        if (showShield) ToggleShield();
        if (showTool) ToggleTool();
        if (showValuables) ToggleValuables();
    }

    private void ToggleAmmo()
    {
        showAmmo = !showAmmo;
        FoldAmmo();
    }

    private void FoldAmmo()
    {
        var parent = ammoButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showAmmo);
        }
    }

    private void ToggleArmor()
    {
        showArmor = !showArmor;
        FoldArmor();
    }

    private void FoldArmor()
    {
        var parent = armorButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showArmor);
        }
    }

    private void ToggleBooks()
    {
        showBooks = !showBooks;
        FoldBooks();
    }

    private void FoldBooks()
    {
        var parent = booksButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showBooks);
        }
    }

    private void ToggleCorpse()
    {
        showCorpse = !showCorpse;
        FoldCorpse();
    }

    private void FoldCorpse()
    {
        var parent = corpseButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showCorpse);
        }
    }

    private void ToggleFood()
    {
        showFood = !showFood;
        FoldFood();
    }

    private void FoldFood()
    {
        var parent = foodButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showFood);
        }
    }

    private void ToggleMelee()
    {
        Debug.Log("ToggleMelee");
        showMelee = !showMelee;
        FoldMelee();
    }

    private void FoldMelee()
    {
        var parent = meleeButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showMelee);
        }
        //Canvas.ForceUpdateCanvases();
        fitter.enabled = false; fitter.enabled = true;
    }

    private void ToggleMissile()
    {
        showMissile = !showMissile;
        FoldMissile();
    }

    private void FoldMissile()
    {
        var parent = missileButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showMissile);
        }
    }

    private void TogglePotion()
    {
        showPotion = !showPotion;
        FoldPotion();
    }

    private void FoldPotion()
    {
        var parent = potionButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showPotion);
        }
    }

    private void ToggleScrolls()
    {
        showScrolls = !showScrolls;
        FoldScrolls();
    }

    private void FoldScrolls()
    {
        var parent = ammoButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showScrolls);
        }
    }

    private void ToggleShield()
    {
        showShield = !showShield; 
        FoldShield();
    }

    private void FoldShield()
    {
        var parent = ammoButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showShield);
        }
    }

    private void ToggleTool()
    {
        showTool = !showTool;
        FoldTool();
    }

    private void FoldTool()
    {
        var parent = toolButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showTool);
        }
    }

    private void ToggleValuables()
    {
        showValuables = !showValuables;
        FoldValuables();
    }

    private void FoldValuables()
    {
        var parent = ammoButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showValuables);
        }
    }

    private void ToggleMisc()
    {
        showMisc = !showMisc;
        FoldMisc();
    }

    private void FoldMisc()
    {
        var parent = miscButton.transform.parent;
        //Start at 1 to skip the button
        for (int i = 1; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.SetActive(showMisc);
        }
    }
}