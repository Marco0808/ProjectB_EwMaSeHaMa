using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text taskNameText;

    private TaskObject _taskObject;

    public static event Action<TaskObject> OnDoTask;

    public void Initialize(TaskObject taskObject)
    {
        _taskObject = taskObject;
        taskNameText.text = _taskObject.TaskData.TaskName;
    }

    public void DoTaskPressed()
    {
        if (_taskObject) OnDoTask?.Invoke(_taskObject);
    }
}
