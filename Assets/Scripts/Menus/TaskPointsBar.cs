using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPointsBar : MonoBehaviour
{
    [SerializeField] private Image pointBar;
    [SerializeField] private Image goalIndicator;

    public Image PointBar => pointBar;
    public Image GoalIndicator => goalIndicator;
    public float MaxLenght { get; set; }

    public void Show(Color barColor)
    {
        pointBar.color = barColor;
        goalIndicator.color = barColor;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetTaskPoints(float taskBarValue)
    {
        RectTransform taskBarTrans = pointBar.rectTransform;
        taskBarTrans.sizeDelta = new Vector2(MaxLenght * taskBarValue, taskBarTrans.sizeDelta.y);
    }
}
