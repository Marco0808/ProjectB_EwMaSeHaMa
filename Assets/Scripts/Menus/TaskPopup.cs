using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskPopup : MonoBehaviour
{
    [SerializeField] private Color taskNormalColor;
    [SerializeField] private Color taskHighlightedColor;
    [SerializeField] private Color trapNormalColor;
    [SerializeField] private Color trapHighlightedColor;

    [SerializeField] private TMP_Text taskNameText;
    [SerializeField] private Image trapButtonImage;
    [SerializeField] private Image taskButtonImage;

    private TaskObject _taskObject;
    private InputActions _input;
    private Vector2 _menuCenterPoint;

    public static event Action<TaskObject> OnDoTask;
    public static event Action<TaskObject> OnPlaceTrap;


    private void Start()
    {
        _input = new InputActions();
        _input.Enable();

        _menuCenterPoint = Camera.main.WorldToViewportPoint(transform.position);
    }

    private void OnDestroy()
    {
        _input.Disable();
    }

    private void Update()
    {
        Vector2 mousePoint = Camera.main.ScreenToViewportPoint(_input.Game.MousePosition.ReadValue<Vector2>());

        if (mousePoint.y > _menuCenterPoint.y)
        {
            bool isMouseLeft = mousePoint.x < _menuCenterPoint.x;
            trapButtonImage.color = isMouseLeft ? trapHighlightedColor : trapNormalColor;
            taskButtonImage.color = isMouseLeft ? taskNormalColor : taskHighlightedColor;
        }
        else
        {
            trapButtonImage.color = trapNormalColor;
            taskButtonImage.color = taskNormalColor;
        }
    }

    public void Initialize(TaskObject taskObject)
    {
        _taskObject = taskObject;
        taskNameText.text = _taskObject.Task.TaskName;
    }

    public void ExecuteMouseInput()
    {
        Vector2 mousePoint = Camera.main.ScreenToViewportPoint(_input.Game.MousePosition.ReadValue<Vector2>());

        if (mousePoint.y > _menuCenterPoint.y && _taskObject)
        {
            if (mousePoint.x < _menuCenterPoint.x)
                OnPlaceTrap?.Invoke(_taskObject);
            else OnDoTask?.Invoke(_taskObject);
        }
    }
}
