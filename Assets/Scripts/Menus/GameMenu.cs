using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO Game manager back to GameMenu class
public class GameMenu : MonoBehaviour
{
    [SerializeField] private PlayerProgressBars playerProgressBars;
    [SerializeField] private QuestMenu questMenu;
    [SerializeField] private Image localPlayerColor;

    public static GameMenu Singleton { get; private set; }

    public static event Action OnLeaveGameButtonPressed;

    private NetworkGamePlayer[] _gamePlayers = new NetworkGamePlayer[4];

    public PlayerProgressBars PlayerProgressBars => playerProgressBars;
    public QuestMenu QuestMenu => questMenu;
    public NetworkGamePlayer[] GamePlayers => _gamePlayers;


    private void Awake()
    {
        if (Singleton) Destroy(gameObject);
        else Singleton = this;
    }
       
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
