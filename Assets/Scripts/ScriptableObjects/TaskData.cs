using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TaskData", menuName = "Custom/Task Data")]

public class TaskData : ScriptableObject
{
    [SerializeField] private Sprite objectSprite;
    [SerializeField] private string taskName = "Task";
    [SerializeField] private Sprite icon;
    [SerializeField] private int maxPlayersAtTask = 1;
    [SerializeField] private float workingTime = 1;

    public Sprite ObjectSprite => objectSprite;
    public string TaskName => taskName;
    public Sprite Icon => icon;
    public int MaxPlayersAtTask => maxPlayersAtTask;
    public float WorkingTime => workingTime;
}
