using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskTimeIndicator : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    public void InitializeTimer(float duration, bool isPlacingTrap)
    {
        timerImage.color = isPlacingTrap ? NetworkManagerHW.Singleton.GameData.InsanityColor : NetworkManagerHW.Singleton.GameData.TaskColor;
        StartCoroutine(Timer(duration));
    }

    public void DestroyIndicator()
    {
        Destroy(gameObject);
    }

    private IEnumerator Timer(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            timerImage.fillAmount = time / duration;
            time += Time.deltaTime;
            yield return null;
        }
    }
}
