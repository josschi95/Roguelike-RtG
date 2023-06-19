using JS.ECS;
using JS.ECS.Tags;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryContextMenu : MonoBehaviour
{
    [SerializeField] private UIInventoryMenu inventoryMenu;

    [Space]

    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemText;

    [Space]

    [SerializeField] private Button equipButton;
    [SerializeField] private Button eatButton;
    [SerializeField] private Button readButton;
    [SerializeField] private Button favoriteButton;
    [SerializeField] private Button junkButton;
    [SerializeField] private Button inspectButton;
    [SerializeField] private Button dropSingleButton;
    [SerializeField] private Button dropStackButton;
    [SerializeField] private Button closeButton;

    private TMP_Text favoriteText, junkText, dropStackText;

    private void Awake()
    {
        favoriteText = favoriteButton.GetComponentInChildren<TMP_Text>();
        junkText = junkButton.GetComponentInChildren<TMP_Text>();
        dropStackText = dropStackButton.GetComponentInChildren<TMP_Text>();
    }

    public void OnItemSelected(Entity item)
    {
        gameObject.SetActive(true);
        //Debug.Log("selecting " + item.Name);

        //Set itemImage to its image

        itemText.text = item.Name; //set the text to the item's display name
        //Later also add in basic stats, so AV/DV for armor, dmg/type for weapons, hydrate/satiate for food, etc.
        EntityManager.TryGetComponent<ObjectStack>(item, out var stack);

        #region - Food -
        EntityManager.TryGetComponent<Food>(item, out var food);
        eatButton.gameObject.SetActive(food != null);
        if (food != null)
        {
            eatButton.onClick.AddListener(delegate
            {
                //consume the item. If it was destroyed in the process (count reduced to 0) close panel
                if (StomachSystem.OnItemConsumed(food, EntityManager.Player)) gameObject.SetActive(false);
            });
        }
        #endregion

        #region - Favorite -
        EntityManager.TryGetTag<Favorite>(item, out var favorite);
        if (favorite != null) favoriteText.text = "Unmark Favorite";
        else favoriteText.text = "Mark Favorite";

        favoriteButton.onClick.AddListener(delegate
        {
            ToggleFavorite(item);
        });
        #endregion

        #region - Junk -
        EntityManager.TryGetTag<Junk>(item, out var junk);
        if (junk != null) junkText.text = "Unmark Junk";
        else junkText.text = "Mark Junk";

        junkButton.onClick.AddListener(delegate
        {
            ToggleJunk(item);
        });
        #endregion

        #region - Drop -
        if (stack != null)
        {
            if (stack.Count == 1) dropStackText.text = "Drop";
            else dropStackText.text = "Drop (" + stack.Count + ")";
            dropStackButton.onClick.AddListener(delegate
            {
                DropItem(item, stack.Count);
            });
        }
        else
        {
            dropStackText.text = "Drop";
            dropStackButton.onClick.AddListener(delegate
            {
                DropItem(item, 1);
            });
        }
        //Only enable DropSingle if the ObjectStack exists and there is more than 1 item
        dropSingleButton.gameObject.SetActive(stack != null && stack.Count > 1);
        if (stack != null && stack.Count > 1)
        {
            dropSingleButton.onClick.AddListener(delegate
            {
                DropItem(item, 1);
            });
        }
        #endregion

        closeButton.onClick.AddListener(delegate { gameObject.SetActive(false); });
    }

    private void ToggleFavorite(Entity item)
    {
        EntityManager.TryGetTag<Favorite>(item, out var favorite);
        if (favorite != null)
        {
            Debug.Log("Unmarking " + item.Name + " as favorite");
            EntityManager.RemoveTag(item, favorite);
            favoriteText.text = "Mark Favorite";
        }
        else
        {
            Debug.Log("Marking " + item.Name + " as favorite");
            EntityManager.AddTag(item, new Favorite());
            favoriteText.text = "Unmark Favorite";
        }
    }

    private void ToggleJunk(Entity item)
    {
        EntityManager.TryGetTag<Junk>(item, out var junk);
        if (junk != null)
        {
            Debug.Log("Unmarking " + item.Name + " as junk");
            EntityManager.RemoveTag(item, junk);
            junkText.text = "Mark Junk";
        }
        else
        {
            Debug.Log("Marking " + item.Name + " as junk");
            EntityManager.AddTag(item, new Junk());
            junkText.text = "Unmark Junk";
        }
    }

    private void DropItem(Entity item, int count)
    {
        InventorySystem.DropItem(EntityManager.GetComponent<Inventory>(EntityManager.Player), item, count);
        gameObject.SetActive(false);
        inventoryMenu.RefreshDisplay();
    }

    private void OnDisable()
    {
        equipButton.onClick.RemoveAllListeners();
        eatButton.onClick.RemoveAllListeners();
        readButton.onClick.RemoveAllListeners();
        favoriteButton.onClick.RemoveAllListeners();
        junkButton.onClick.RemoveAllListeners();
        inspectButton.onClick.RemoveAllListeners();
        dropSingleButton.onClick.RemoveAllListeners();
        dropStackButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
}
