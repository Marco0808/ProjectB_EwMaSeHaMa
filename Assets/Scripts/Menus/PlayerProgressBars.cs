using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class PlayerProgressBars : MonoBehaviour
{
    [Header("Task Points")]
    [SerializeField] private float maxQuestBarLenght = 150;
    [SerializeField] private QuestPointsBar[] questPointBars;
    [SerializeField] private RectTransform teamGoalIndicator;
    [SerializeField] private RectTransform questBarBackground;
    [SerializeField] private GameData gameData;

    [Header("Insanity Points")]
    [SerializeField] private RectTransform insanityBarTrans;

    float _maxInsanityBarLenght;

    private void Start()
    {
        _maxInsanityBarLenght = insanityBarTrans.sizeDelta.x;

        InitQuestProgressBar();
        foreach (QuestPointsBar taskBar in questPointBars) taskBar.Hide();

        SetInsanityPoints(0);
    }

    public QuestPointsBar GetAvailablequestPointsBar()
    {
        foreach (QuestPointsBar taskBar in questPointBars)
            if (!taskBar.gameObject.activeSelf)
                return taskBar;
        return null;
    }

    public void SetInsanityPoints(int insanityPoints)
    {
        float normalizedInsanityPoints = (float)insanityPoints / NetworkManagerHW.Singleton.GameData.MaxInsanityPoints;
        insanityBarTrans.sizeDelta = new Vector2(_maxInsanityBarLenght * normalizedInsanityPoints, insanityBarTrans.sizeDelta.y);
    }

    public void InitTeamGoalAndBackground(int playerCount)
    {
        // Team QuestPoint goal indicator
        float teamGoalDistance = maxQuestBarLenght * playerCount * gameData.TeamWinPointPercentage;
        teamGoalIndicator.anchoredPosition = new Vector2(teamGoalDistance, 0);

        // Progress bar background 
        float maxWinningLenght = maxQuestBarLenght * playerCount;
        questBarBackground.sizeDelta = new Vector2(maxWinningLenght, questBarBackground.sizeDelta.y);
    }

    [Button("Update Progress Bar", EButtonEnableMode.Editor)]
    private void InitQuestProgressBar()
    {
        // Quest progress bars
        foreach (QuestPointsBar bar in questPointBars)
        {
            bar.MaxLenght = maxQuestBarLenght;
            bar.ProgressBarTrans.sizeDelta = new Vector2(maxQuestBarLenght, bar.ProgressBarTrans.sizeDelta.y);
            RectTransform goalTrans = bar.GoalIndicator.rectTransform;
            goalTrans.anchoredPosition = new Vector2(maxQuestBarLenght, 0);
        }

        if (!Application.isPlaying) InitTeamGoalAndBackground(4);
    }
}
