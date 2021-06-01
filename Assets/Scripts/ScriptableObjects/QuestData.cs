using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New QuestData", menuName = "Housework/Quest Data")]
public class QuestData : ScriptableObject
{
    [SerializeField] private string title = "Quest";
    [SerializeField, ResizableTextArea] private string caption = "Lorem ipsum dolor sit amet consectetur adipisicing elit.";
    [SerializeField] private int questPoints = 100;
    [SerializeField] private bool completeTasksInOrder = true;
    [SerializeField, OnValueChanged("DisplayTaskIconInInspector")] private TaskData[] tasksToComplete;

    [SerializeField, ShowAssetPreview, ReadOnly] private Sprite[] debugTaskIconsDisplay;

    public string Title => title;
    public string Caption => caption;
    public int QuestPoints => questPoints;
    public bool CompleteTasksInOrder => completeTasksInOrder;
    public TaskData[] Tasks => tasksToComplete;

    private void DisplayTaskIconInInspector()
    {
        debugTaskIconsDisplay = new Sprite[tasksToComplete.Length];
        for (int i = 0; i < tasksToComplete.Length; i++)
            debugTaskIconsDisplay[i] = tasksToComplete[i]?.Icon;
    }
}
