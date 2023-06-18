using JS.ECS;
using JS.ECS.Tags;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryContextMenu : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemText;

    [Space]

    [SerializeField] private Button equipButton;
    [SerializeField] private Button eatButton;
    [SerializeField] private Button readButton;
    [SerializeField] private Button favoriteButton;
    [SerializeField] private Button junkButton;
    [SerializeField] private Button inspectButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button closeButton;

    public void OnItemSelected(Entity entity)
    {
        gameObject.SetActive(true);
        //Set itemImage to its image
        //set the text to the item's text
        EntityManager.TryGetComponent<ObjectStack>(entity, out var stack);

        #region - Food -
        EntityManager.TryGetComponent<Food>(entity, out var food);

        if (food == null) eatButton.gameObject.SetActive(false);
        else
        {
            eatButton.gameObject.SetActive(true);
            eatButton.onClick.AddListener(delegate
            {
                //consume the item. If it was destroyed in the process (count reduced to 0) close panel
                if (StomachSystem.OnItemConsumed(food, EntityManager.Player)) gameObject.SetActive(false);
            });
        }
        #endregion

        #region - Favorite -
        EntityManager.TryGetTag<Favorite>(entity, out var favorite);
        if (favorite != null) //Item is marked as favorite
        {
            favoriteButton.GetComponentInChildren<TMP_Text>().text = "Unmark Favorite";
            favoriteButton.onClick.AddListener(delegate
            {
                EntityManager.RemoveTag(entity, favorite);
                favoriteButton.GetComponentInChildren<TMP_Text>().text = "Mark Favorite";
            });
        }
        else
        {
            favoriteButton.GetComponentInChildren<TMP_Text>().text = "Mark Favorite";
            favoriteButton.onClick.AddListener(delegate
            {
                EntityManager.AddTag(entity, new Favorite());
                favoriteButton.GetComponentInChildren<TMP_Text>().text = "Unmark Favorite";
            });
        }
        #endregion

        #region - Junk -
        EntityManager.TryGetTag<Junk>(entity, out var junk);
        if (junk != null) //Item is marked as junk
        {
            favoriteButton.GetComponentInChildren<TMP_Text>().text = "Unmark Junk";
            favoriteButton.onClick.AddListener(delegate
            {
                EntityManager.RemoveTag(entity, junk);
                favoriteButton.GetComponentInChildren<TMP_Text>().text = "Mark Junk";
            });
        }
        else
        {
            favoriteButton.GetComponentInChildren<TMP_Text>().text = "Mark Junk";
            favoriteButton.onClick.AddListener(delegate
            {
                EntityManager.AddTag(entity, new Junk());
                favoriteButton.GetComponentInChildren<TMP_Text>().text = "Unmark Junk";
            });
        }
        #endregion

        closeButton.onClick.AddListener(delegate
        {
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        equipButton.onClick.RemoveAllListeners();
        eatButton.onClick.RemoveAllListeners();
        readButton.onClick.RemoveAllListeners();
        favoriteButton.onClick.RemoveAllListeners();
        junkButton.onClick.RemoveAllListeners();
        inspectButton.onClick.RemoveAllListeners();
        dropButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }
}
