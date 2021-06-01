using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PanCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Vector2 maxOffset = new Vector2(10f, 5f);
    [SerializeField] private float mouseCameraSpeed = 2f;
    [SerializeField] private float keyboardCameraSpeed = 25f;

    [Header("Debug")]
    [SerializeField] private float zoomStepsSize = 1;
    [SerializeField] private int screenCaptureSizeMult = 2;

    private InputActions _input;
    private Camera _camera;
    private Vector2 _lastMousePos;
    private float _defaultZoomSize;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _defaultZoomSize = _camera.orthographicSize;

        _input = new InputActions();
        _input.Game.CameraZoomIn.performed += _ => CameraZoom(false);
        _input.Game.CameraZoomOut.performed += _ => CameraZoom(true);
        _input.Game.CameraZoomReset.performed += _ => CameraZoomReset();
        _input.Game.ScreenCapture.performed += _ => CaptureScreen();
        _input.Enable();
    }

    private void Update()
    {
        Vector2 mousePosition = _input.Game.MousePosition.ReadValue<Vector2>();

        // If RMB hold move camera by mouse
        if (_input.Game.CameraGrab.ReadValue<float>() > 0)
        {
            AddToCameraPosition((_lastMousePos - mousePosition) * (mouseCameraSpeed / 100));
            _lastMousePos = mousePosition;
        }
        // If RMB not hold, accept keyboard input
        else
        {
            Vector2 input = _input.Game.WASD.ReadValue<Vector2>();
            AddToCameraPosition(input * keyboardCameraSpeed * Time.deltaTime);
        }
        _lastMousePos = mousePosition;
    }

    private void AddToCameraPosition(Vector2 positionAdd)
    {
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x + positionAdd.x, -maxOffset.x, maxOffset.x),
            Mathf.Clamp(transform.localPosition.y + positionAdd.y, -maxOffset.y, maxOffset.y));
    }

    private void CameraZoom(bool zoomOut)
    {
        float newZoomSize = _camera.orthographicSize;
        newZoomSize += zoomOut ? zoomStepsSize : -zoomStepsSize;
        _camera.orthographicSize = Mathf.Clamp(newZoomSize, 3, 30);
    }

    private void CameraZoomReset()
    {
        _camera.orthographicSize = _defaultZoomSize;
    }

    private void CaptureScreen()
    {
        string capturePath = $"ScreenCaptures/ScreenCapture_{Time.frameCount % 10000}.png";
        ScreenCapture.CaptureScreenshot(capturePath, screenCaptureSizeMult);
        Debug.Log($"ScreenCapture '{capturePath}' taken!".Color(Color.magenta));
    }
}
