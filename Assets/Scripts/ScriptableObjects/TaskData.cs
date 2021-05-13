using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New TaskData", menuName = "Housework/Task Data")]
public class TaskData : ScriptableObject
{
    [SerializeField] private string taskName = "Task";
    [SerializeField] private int maxPlayersAtTask = 2;
    [SerializeField] private float workingTime = 3;
    [SerializeField, ShowAssetPreview] private Sprite objectSprite;
    [SerializeField, ShowAssetPreview] private Sprite icon;

    public TaskObject TaskObject { get; set; }

    public string TaskName => taskName;
    public int MaxPlayersAtTask => maxPlayersAtTask;
    public float WorkingTime => workingTime;
    public Sprite ObjectSprite => objectSprite;
    public Sprite Icon => icon;
}
