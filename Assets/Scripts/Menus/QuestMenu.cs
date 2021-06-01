using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestMenu : MonoBehaviour
{
    [SerializeField] private Image handInterfaceImage;
    [SerializeField] private Transform questContainer;
    [SerializeField] private QuestPanel questPanelPrefab;
    [SerializeField] private QuestOverflowPanel wastedQuestPanelPrefab;

    public void SetHandInterface(Sprite handImage)
    {
        handInterfaceImage.sprite = handImage;
    }

    public QuestPanel AddQuest(QuestData quest)
    {
        QuestPanel questPanel = Instantiate(questPanelPrefab, questContainer);
        questPanel.Initialize(quest);
        return questPanel;
    }

    public QuestOverflowPanel AddQuestOverflowPanel()
    {
        return Instantiate(wastedQuestPanelPrefab, questContainer);
    }
}
