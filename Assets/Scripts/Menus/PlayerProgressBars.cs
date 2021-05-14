using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class PlayerProgressBars : MonoBehaviour
{
    [Header("Task Points")]
    [SerializeField, OnValueChanged("UpdateTaskBarSize")] private float maxTaskBarLenght = 150;
    [SerializeField] private QuestPointsBar[] taskPointBars;
    [SerializeField] private RectTransform taskBarBackground;
    [SerializeField] private GameData gameData;

    [Header("Insanity Points")]
    [SerializeField] private RectTransform insanityBarTrans;

    float _maxInsanityBarLenght;

    private void Start()
    {
        _maxInsanityBarLenght = insanityBarTrans.sizeDelta.x;

        UpdateTaskBarSize();
        foreach (QuestPointsBar taskBar in taskPointBars) taskBar.Hide();

        SetInsanityPoints(0);
    }

    public QuestPointsBar GetAvailablequestPointsBar()
    {
        foreach (QuestPointsBar taskBar in taskPointBars)
            if (!taskBar.gameObject.activeSelf)
                return taskBar;
        return null;
    }

    public void SetInsanityPoints(int insanityPoints)
    {
        float normalizedInsanityPoints = (float)insanityPoints / NetworkManagerHW.Singleton.GameData.MaxInsanityPoints;
        insanityBarTrans.sizeDelta = new Vector2(_maxInsanityBarLenght * normalizedInsanityPoints, insanityBarTrans.sizeDelta.y);
    }

    private void UpdateTaskBarSize()
    {
        foreach (QuestPointsBar bar in taskPointBars)
        {
            bar.MaxLenght = maxTaskBarLenght;
            bar.ProgressBar.rectTransform.sizeDelta = new Vector2(maxTaskBarLenght, bar.ProgressBar.rectTransform.sizeDelta.y);
            RectTransform goalTrans = bar.GoalIndicator.rectTransform;
            goalTrans.localPosition = new Vector2(maxTaskBarLenght - goalTrans.sizeDelta.x / 2, 0);
        }

        UpdateTaskBarBackground();
    }

    private void UpdateTaskBarBackground()
    {
        float maxWinningLenght = ((maxTaskBarLenght + 5) * 4) * gameData.TeamWinPointPercentage;
        taskBarBackground.sizeDelta = new Vector2(maxWinningLenght, taskBarBackground.sizeDelta.y);
    }
}
