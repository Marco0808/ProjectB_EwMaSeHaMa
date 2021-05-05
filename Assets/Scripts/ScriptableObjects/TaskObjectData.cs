using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TaskObjectData", menuName = "Custom/Task Object Data")]

public class TaskObjectData : ScriptableObject
{
    [SerializeField] private Sprite objectSprite;
    [SerializeField] private string objectName = "Task Object";
    [SerializeField] private int maxPlayersAtTask = 1;

    public Sprite ObjectSprite => objectSprite;
    public string ObjectName => objectName;
    public int MaxPlayersAtTask => maxPlayersAtTask;
}
