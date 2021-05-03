using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private RectTransform lobbyPlayerParent;

    public static Button StartGameButton { get; private set; }
    public static Button ReadyButton { get; private set; }
    public static RectTransform LobbyPlayerParent { get; private set; }

    public static event Action OnStartGameButtonPressed;
    public static event Action OnReadyButtonPressed;

    private void Awake()
    {
        StartGameButton = startGameButton;
        ReadyButton = readyButton;
        LobbyPlayerParent = lobbyPlayerParent;
    }

    public void ReadyUp()
    {
        OnReadyButtonPressed?.Invoke();
    }

    public void StartGame()
    {
        OnStartGameButtonPressed?.Invoke();
    }

    public void LeaveLobby()
    {
        NetworkClient.Disconnect();
    }
}
