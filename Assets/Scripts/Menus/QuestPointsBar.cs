using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPointsBar : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private Image goalIndicator;

    private float _maxLenght;

    public Image ProgressBar => progressBar;
    public Image GoalIndicator => goalIndicator;
    public float MaxLenght { get => _maxLenght; set => _maxLenght = value; }

    public void Initialize(Color barColor)
    {
        progressBar.color = barColor;
        goalIndicator.color = barColor;

        SetQuestPoints(0);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetQuestPoints(float questPoints)
    {
        RectTransform taskBarTrans = progressBar.rectTransform;
        float normalizedQuestPoints = (float)questPoints / NetworkManagerHW.Singleton.GameData.MaxQuestPoints;
        taskBarTrans.sizeDelta = new Vector2(_maxLenght * normalizedQuestPoints, taskBarTrans.sizeDelta.y);
    }
}
