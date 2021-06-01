using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestOverflowPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text questPointsText;
    [SerializeField] private AnimationClip hidePanelAnimation;
    [SerializeField] private Animator animator;

    public void Show()
    {
        animator.SetTrigger("ShowPanel");
    }

    public void Hide()
    {
        StartCoroutine(DestroyAfterAnimation());
    }

    public void WasteQuest(QuestData quest)
    {
        titleText.text = quest.Title;
        questPointsText.text = quest.QuestPoints.ToString();

        animator.SetTrigger("WasteQuest");
    }

    private IEnumerator DestroyAfterAnimation()
    {
        animator.SetTrigger("HidePanel");
        yield return new WaitForSeconds(hidePanelAnimation.length);
        Destroy(gameObject);
    }
}