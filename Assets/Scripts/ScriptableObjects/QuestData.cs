using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New QuestData", menuName = "Housework/Quest Data")]

public class QuestData : ScriptableObject
{
    [SerializeField] private string title = "Quest";
    [SerializeField, ResizableTextArea] private string caption = "Lorem ipsum dolor sit amet consectetur adipisicing elit.";
    [SerializeField] private int taskPoints = 100;
    [SerializeField] private bool completeTasksInOrder = true;
    [SerializeField] private TaskData[] tasksToComplete;

    public string Title => title;
    public string Caption => caption;
    public int TaskPoints => taskPoints;
    public bool CompleteTasksInOrder => completeTasksInOrder;
    public TaskData[] Tasks => tasksToComplete;
}
