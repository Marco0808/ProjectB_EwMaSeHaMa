using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskIcon : MonoBehaviour
{

    [SerializeField] private Image taskIcon;
    [SerializeField] private Image taskState;
    [SerializeField] private Sprite inProgressState;
    [SerializeField] private Sprite completedState;

    public void Initialize(Sprite icon)
    {
        SetCompleted(false);
        taskIcon.sprite = icon;
    }

    public void SetCompleted(bool isCompleted)
    {
        taskState.sprite = isCompleted ? completedState : inProgressState;
    }
}
