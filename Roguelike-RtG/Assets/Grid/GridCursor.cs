using UnityEngine;
using UnityEngine.InputSystem;
using JS.WorldMap;

public class GridCursor : MonoBehaviour
{
    [SerializeField] private InputActionProperty mousePosition;
    [SerializeField] private WorldData worldMap;
    [SerializeField] private NodeDisplay nodeDisplay;
    [SerializeField] private Camera cam;

    private Vector2 mousePos;

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
        SetCursorPosition();
        CheckNode();
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
        //if (WorldMap.instance == null) return;
        //var node = WorldMap.instance.GetNode(transform.position);
        var node = worldMap.GetNode(transform.position);
        nodeDisplay.DisplayNodeValues(node);

        if (Input.GetMouseButtonDown(0))
        {
            var dis = FindObjectOfType<MapDisplay>();
            dis.HighlightNode(node);
        }
    }
}
