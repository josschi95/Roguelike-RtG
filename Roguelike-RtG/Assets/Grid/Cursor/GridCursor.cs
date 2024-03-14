using UnityEngine;
using UnityEngine.InputSystem;
using JS.World.Map;

public class GridCursor : MonoBehaviour
{
    [SerializeField] private InputActionProperty mousePosition;
    [SerializeField] private WorldData worldMap;
    [SerializeField] private NodeDisplay nodeDisplay;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Camera cam;

    private Vector2 mousePos;
    private float inactiveTimer;
    private float timeToDeactive = 5;
    private void Start()
    {
        mousePosition.action.performed += i => mousePos = i.ReadValue<Vector2>();
    }

    private void OnDestroy()
    {
        mousePosition.action.performed -= i => mousePos = i.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        bool isActive = CheckActivity();
        spriteRenderer.enabled = isActive;
        if (!isActive)
        {
            nodeDisplay.HideDisplay();
            return;
        }

        SetCursorPosition();
        CheckNode();
    }

    private bool CheckActivity()
    {
        if (mousePosition.action.WasPerformedThisFrame())
        {
            inactiveTimer = 0;
            return true;
        }
        else
        {
            inactiveTimer += Time.deltaTime;
            if (inactiveTimer >= timeToDeactive) return false;
        }
        return true;
    }

    private void SetCursorPosition()
    {
        var pos = cam.ScreenToWorldPoint(mousePos);

        pos.x = Mathf.RoundToInt(pos.x);
        pos.y = Mathf.RoundToInt(pos.y);
        pos.z = 0;
        transform.position = pos;
    }

    private void CheckNode()
    {
        var node = worldMap.GetNode(transform.position);
        spriteRenderer.enabled = node != null;
        nodeDisplay.DisplayNodeValues(node);

        if (Input.GetMouseButtonDown(0))
        {
            var dis = FindObjectOfType<MapDisplay>();
            dis.HighlightNode(node);
        }
    }
}

public enum CursorControlMethod
{
    Mouse,
    Keys,
}