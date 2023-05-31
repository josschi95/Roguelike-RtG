using JS.WorldMap;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputActionProperty mousePosition;
    [SerializeField] private InputActionProperty rightMouseButton;
    [SerializeField] private InputActionProperty mouseScroll;
    [SerializeField] private WorldData worldData;
    private Camera cam;
    private PixelPerfectCamera pixelCam;

    private Vector3 dragOrigin;
    private Coroutine panCoroutine;
    private bool isPanning;

    private Coroutine smoothingCoroutine;

    private float minX = -0.5f, minY = -0.5f, maxX = 200.5f, maxY = 200.5f;

    private void Start()
    {
        cam = GetComponent<Camera>();
        pixelCam = GetComponent<PixelPerfectCamera>();
        maxX = worldData.Width + 0.5f;
        maxY = worldData.Height + 0.5f;
    }

    private void OnEnable()
    {
        rightMouseButton.action.performed += BeginPanCamera;
        //rightMouseButton.action.performed += i => BeginPanCamera();
        rightMouseButton.action.canceled += i => isPanning = false;
        mouseScroll.action.performed += i => ZoomCamera(i.ReadValue<Vector2>().y);
    }

    private void OnDisable()
    {
        rightMouseButton.action.performed -= BeginPanCamera;
        //rightMouseButton.action.performed -= i => BeginPanCamera();
        rightMouseButton.action.canceled -= i => isPanning = false;
        mouseScroll.action.performed -= i => ZoomCamera(i.ReadValue<Vector2>().y);
    }

    private void BeginPanCamera(InputAction.CallbackContext obj)
    {
        dragOrigin = cam.ScreenToWorldPoint(mousePosition.action.ReadValue<Vector2>());
        if (panCoroutine != null) StopCoroutine(panCoroutine);
        panCoroutine = StartCoroutine(PanCamera());
    }

    private void BeginPanCamera()
    {
        dragOrigin = cam.ScreenToWorldPoint(mousePosition.action.ReadValue<Vector2>());
        if (panCoroutine != null) StopCoroutine(panCoroutine);
        panCoroutine = StartCoroutine(PanCamera());
    }

    private IEnumerator PanCamera()
    {
        isPanning = true;
        while (isPanning)
        {
            if (!mousePosition.action.WasPerformedThisFrame()) yield return null;

            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(mousePosition.action.ReadValue<Vector2>());
            transform.position += difference;
            KeepCameraInBounds();
            yield return null;
        }
    }

    private void KeepCameraInBounds()
    {
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        transform.position = new Vector3
            (
                Mathf.Clamp(transform.position.x, minX + width, maxX - width),
                Mathf.Clamp(transform.position.y, minY + height, maxY - height),
                transform.position.z
            );
    }

    public void SmoothToPosition(Vector2Int pos)
    {
        if (smoothingCoroutine != null) StopCoroutine(smoothingCoroutine);
        smoothingCoroutine = StartCoroutine(SmoothCam(pos));
    }

    private IEnumerator SmoothCam(Vector2Int pos)
    {
        float t = 0, timeToMove = 0.2f;
        var newPos = new Vector3(pos.x, pos.y, transform.position.z);
        while(t < timeToMove)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, t / timeToMove);
            KeepCameraInBounds();
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = newPos;
        KeepCameraInBounds();
    }

    private void ZoomCamera(float zoom)
    {
        switch (pixelCam.assetsPPU)
        {
            case 8:
                if (zoom > 0) pixelCam.assetsPPU = 16;
                break;
            case 16:
                if (zoom < 0) pixelCam.assetsPPU = 8;
                else pixelCam.assetsPPU = 32;
                break;
            case 32:
                if (zoom < 0) pixelCam.assetsPPU = 16;
                else pixelCam.assetsPPU = 64;
                break;
            case 64:
                if (zoom < 0) pixelCam.assetsPPU = 32;
                else pixelCam.assetsPPU = 128;
                break;
                case 128:
                if (zoom < 0) pixelCam.assetsPPU = 64;
                break;
            default:
                pixelCam.assetsPPU = 32;
                break;
        }
        KeepCameraInBounds();
    }
}