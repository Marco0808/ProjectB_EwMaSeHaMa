using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestPanel : MonoBehaviour
{
    [SerializeField] private TaskIcon taskIconPrefab;

    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text captionText;
    [SerializeField] private TMP_Text taskPointsText;
    [SerializeField] private GameObject completeInOrderIndicator;
    [SerializeField] private Transform taskContainer;

    private QuestData _quest;
    private Dictionary<TaskData, TaskIcon> _taskIcons = new Dictionary<TaskData, TaskIcon>();


    public void Initialize(QuestData quest)
    {
        _quest = quest;

        titleText.text = quest.Title;
        captionText.text = quest.Caption;
        completeInOrderIndicator.SetActive(quest.CompleteTasksInOrder);

        foreach (TaskData task in quest.Tasks)
        {
            TaskIcon taskIcon = Instantiate(taskIconPrefab, taskContainer);
            taskIcon.Initialize(task.Icon);
        }
    }

    public void CompleteTask(TaskData task)
    {
        if (_taskIcons.TryGetValue(task, out TaskIcon taskIcon))
        {
            //TODO Complete task and check if whole quest is completet (or better do this on server?)
            taskIcon.SetCompleted(true);
        }
    }
}
