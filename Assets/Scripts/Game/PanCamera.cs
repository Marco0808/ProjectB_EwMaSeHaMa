using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCamera : MonoBehaviour
{
    [SerializeField] private Vector2 maxOffset = new Vector2(10f, 5f);
    [SerializeField] private float mouseCameraSpeed = 0.02f;
    [SerializeField] private float keyboardCameraSpeed = 25f;

    private InputActions _input;
    private Vector2 _lastMousePos;

    private void Awake()
    {
        _input = new InputActions();
        _input.Enable();
    }

    private void Update()
    {
        Vector2 mousePosition = _input.Game.MousePosition.ReadValue<Vector2>();

        // If RMB hold move camera by mouse
        if (_input.Game.CameraGrab.ReadValue<float>() > 0)
        {
            AddToCameraPosition((_lastMousePos - mousePosition) * mouseCameraSpeed);
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
}
