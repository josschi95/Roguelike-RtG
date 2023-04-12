using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputActionProperty mousePosition;
    [SerializeField] private InputActionProperty rightMouseButton;
    [SerializeField] private InputActionProperty mouseScroll;

    private Camera cam;
    private PixelPerfectCamera pixelCam;

    private Vector3 dragOrigin;
    private Coroutine panCoroutine;
    private bool isPanning;

    private void Start()
    {
        cam = GetComponent<Camera>();
        pixelCam = GetComponent<PixelPerfectCamera>();

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
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(mousePosition.action.ReadValue<Vector2>());
            transform.position += difference;
            yield return null;
        }
    }

    private void ZoomCamera(float zoom)
    {
        switch (pixelCam.assetsPPU)
        {
            case 8:
                if (zoom > 0) pixelCam.assetsPPU = 16;
                break;
            case 16:
                if (zoom > 0) pixelCam.assetsPPU = 32;
                else pixelCam.assetsPPU = 8;
                break;
            case 32:
                if (zoom > 0) pixelCam.assetsPPU = 64;
                else pixelCam.assetsPPU = 16;
                break;
            case 64:
                if (zoom < 0) pixelCam.assetsPPU = 32;
                break;
            default:
                pixelCam.assetsPPU = 32;
                break;
        }
    }
}