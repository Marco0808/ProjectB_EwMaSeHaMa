using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPointsBar : MonoBehaviour
{
    [SerializeField] private Image pointBar;
    [SerializeField] private Image goalIndicator;

    private float _maxLenght;

    public Image PointBar => pointBar;
    public Image GoalIndicator => goalIndicator;
    public float MaxLenght { get => _maxLenght; set => _maxLenght = value; }

    public void Initialize(Color barColor)
    {
        pointBar.color = barColor;
        goalIndicator.color = barColor;

        SetTaskPoints(0);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetTaskPoints(float taskBarValue)
    {
        RectTransform taskBarTrans = pointBar.rectTransform;
        taskBarTrans.sizeDelta = new Vector2(_maxLenght * taskBarValue, taskBarTrans.sizeDelta.y);
    }
}
