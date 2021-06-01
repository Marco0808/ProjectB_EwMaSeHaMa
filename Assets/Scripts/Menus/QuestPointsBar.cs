using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPointsBar : MonoBehaviour
{
    [SerializeField] private RectTransform progressBarTrans;
    [SerializeField] private Image progressBarImage;
    [SerializeField] private Image goalIndicator;
    [SerializeField] private Image playerHeadIcon;

    private RectTransform _progressBarLayout;
    private float _maxLenght;

    public RectTransform ProgressBarTrans => progressBarTrans;
    public Image GoalIndicator => goalIndicator;
    public float MaxLenght { get => _maxLenght; set => _maxLenght = value; }

    private void Awake()
    {
        _progressBarLayout = transform.parent.GetComponentInParent<RectTransform>();
    }

    public void Initialize(Color barColor, Sprite playerHead)
    {
        progressBarImage.color = barColor;
        goalIndicator.color = barColor;
        playerHeadIcon.sprite = playerHead;

        SetQuestPoints(0);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetQuestPoints(float questPoints)
    {
        RectTransform taskBarTrans = progressBarTrans;
        float normalizedQuestPoints = (float)questPoints / NetworkManagerHW.Singleton.GameData.MaxQuestPoints;
        taskBarTrans.sizeDelta = new Vector2(_maxLenght * normalizedQuestPoints, taskBarTrans.sizeDelta.y);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_progressBarLayout);
    }
}
