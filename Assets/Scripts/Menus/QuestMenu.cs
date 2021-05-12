using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestMenu : MonoBehaviour
{
    [SerializeField] private Transform questContainer;
    [SerializeField] private QuestPanel questPanelPrefab;

    public QuestPanel AddQuest(QuestData quest)
    {
        QuestPanel questPanel = Instantiate(questPanelPrefab, questContainer);
        questPanel.Initialize(quest);
        return questPanel;
    }
}
