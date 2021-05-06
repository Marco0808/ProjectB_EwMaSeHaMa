using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private Image localPlayerColor;
    [SerializeField] private TMP_Text taskScheduleText;

    public TMP_Text TaskScheduleText => taskScheduleText;

    public static event Action OnLeaveGameButtonPressed;

    public void SetLocalPlayerColor(Color color)
    {
        color.a = 1;
        localPlayerColor.color = color;
    }

    public void LeaveGamePressed()
    {
        OnLeaveGameButtonPressed?.Invoke();
    }
}
