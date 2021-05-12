using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskMenu : MonoBehaviour
{
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
            SetImageAlpha(trapButtonImage, isMouseLeft ? 0.8f : 0.3f);
            SetImageAlpha(taskButtonImage, isMouseLeft ? 0.3f : 0.8f);
        }
        else
        {
            SetImageAlpha(trapButtonImage, 0.3f);
            SetImageAlpha(taskButtonImage, 0.3f);
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
            {
                Debug.Log("Trap Button Activated");
                OnPlaceTrap?.Invoke(_taskObject);
            }
            else
            {
                Debug.Log("Task Button Activated");
                OnDoTask?.Invoke(_taskObject);
            }
        }
    }

    public void SetImageAlpha(Image image, float alpha)
    {
        if (image.color.a != alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}
