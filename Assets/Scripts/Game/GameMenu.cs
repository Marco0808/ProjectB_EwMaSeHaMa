using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private Image localPlayerColor;

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
