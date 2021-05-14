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
    [SerializeField] private TMP_Text questPointsText;
    [SerializeField] private Image completeInOrderIndicator;
    [SerializeField] private Transform taskContainer;

    private Dictionary<TaskData, TaskInterface> _taskIcons = new Dictionary<TaskData, TaskInterface>();

    public void Initialize(QuestData quest)
    {
        titleText.text = quest.Title;
        captionText.text = quest.Caption;
        questPointsText.text = quest.QuestPoints.ToString();
        //completeInOrderIndicator.enabled = quest.CompleteTasksInOrder;

        foreach (TaskData task in quest.Tasks)
        {
            TaskIcon taskIcon = Instantiate(taskIconPrefab, taskContainer);
            taskIcon.Initialize(task.Icon);
            _taskIcons.Add(task, new TaskInterface(taskIcon, false));
        }
    }

    public void CompleteQuest()
    {
        Destroy(gameObject);
    }

    public bool TryCompleteTask(TaskData task, out bool questCompleted)
    {
        if (_taskIcons.TryGetValue(task, out TaskInterface taskInterface))
        {
            //TODO Complete task and check if whole quest is completet (or better do this on server?)
            taskInterface.Icon.SetCompleted(true);
            taskInterface.IsCompleted = true;

            foreach (var item in _taskIcons)
                if (!item.Value.IsCompleted)
                {
                    questCompleted = false;
                    return true;
                }

            questCompleted = true;
            return true;
        }
        questCompleted = false;
        return false;
    }
}

public class TaskInterface
{
    public TaskInterface(TaskIcon icon, bool isCompleted)
    {
        Icon = icon;
        IsCompleted = isCompleted;
    }

    public TaskIcon Icon { get; private set; }
    public bool IsCompleted { get; set; }
}
