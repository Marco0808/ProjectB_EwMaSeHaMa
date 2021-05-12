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
    [SerializeField, OnValueChanged("UpdateTaskBarBackground"), Range(0.5f, 1)] private float taskPointPercentageNeeded = 0.7f;
    [SerializeField] private TaskPointsBar[] taskPointBars;
    [SerializeField] private RectTransform taskBarBackground;

    [Header("Insanity Points")]
    [SerializeField] private Image insanityBar;

    private void Start()
    {
        UpdateTaskBarSize();
        foreach (TaskPointsBar taskBar in taskPointBars) taskBar.Hide();
    }

    public TaskPointsBar GetAvailableTaskPointsBar()
    {
        foreach (TaskPointsBar taskBar in taskPointBars)
            if (!taskBar.gameObject.activeSelf)
                return taskBar;
        return null;
    }

    private void UpdateTaskBarSize()
    {
        foreach (TaskPointsBar bar in taskPointBars)
        {
            bar.MaxLenght = maxTaskBarLenght;
            bar.PointBar.rectTransform.sizeDelta = new Vector2(maxTaskBarLenght, bar.PointBar.rectTransform.sizeDelta.y);
            RectTransform goalTrans = bar.GoalIndicator.rectTransform;
            goalTrans.localPosition = new Vector2(maxTaskBarLenght - goalTrans.sizeDelta.x / 2, 0);
        }

        UpdateTaskBarBackground();
    }

    private void UpdateTaskBarBackground()
    {
        float maxWinningLenght = (maxTaskBarLenght * 4) * taskPointPercentageNeeded;
        taskBarBackground.sizeDelta = new Vector2(maxWinningLenght, taskBarBackground.sizeDelta.y);
    }
}
