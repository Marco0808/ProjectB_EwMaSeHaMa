using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(SpriteRenderer))]
public class TaskObjectUpdater : MonoBehaviour
{
    [SerializeField, Required, OnValueChanged("UpdateTaskObject")] private TaskData task;

    public TaskData Task => task;

    [Button("Update Task Object", EButtonEnableMode.Always)]
    private void UpdateTaskObject() => GetComponent<SpriteRenderer>().sprite = task.ObjectSprite;
}
