using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private PlayerProgressBars playerProgressBars;
    [SerializeField] private QuestMenu questMenu;
    [Space]
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMP_Text endScreenText;

    public static GameMenu Singleton { get; private set; }

    public static event Action OnLeaveGameButtonPressed;

    public PlayerProgressBars PlayerProgressBars => playerProgressBars;
    public QuestMenu QuestMenu => questMenu;


    private void Awake()
    {
        if (Singleton) Destroy(gameObject);
        else Singleton = this;

        HideEndScreen();
    }

    public void ShowEndScreen(string message, Color messageColor)
    {
        endScreen.SetActive(true);
        endScreenText.text = message;
        endScreenText.color = messageColor;
    }

    public void HideEndScreen()
    {
        endScreen.SetActive(false);
    }

    public void LeaveGamePressed()
    {
        OnLeaveGameButtonPressed?.Invoke();
    }
}
