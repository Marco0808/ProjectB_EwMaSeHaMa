using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private RectTransform playerPanelContainer;
    [SerializeField] private GameObject playerInviteWindow;
    [SerializeField] private Transform playerInviteContainer;
    [SerializeField] private GameObject invitableFriendPrefab;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TMP_Text readyButtonText;

    public static event Action OnStartButtonPressed;
    public static event Action OnReadyButtonPressed;
    public static event Action OnLeaveLobbyButtonPressed;


    public RectTransform PlayerPanelContainer => playerPanelContainer;
    public GameObject PlayerInviteWindow => playerInviteWindow;
    public Button StartGameButton => startGameButton;


    public void StartButtonPressed()
    {
        OnStartButtonPressed?.Invoke();
    }

    public void ReadyButtonPressed()
    {
        OnReadyButtonPressed?.Invoke();
    }

    public void SetReadyButtonState(bool isReady)
    {
        readyButton.image.color = isReady ? Color.green : Color.yellow;
        readyButtonText.text = isReady ? "Ready" : "Ready Up";
    }

    public void LeaveLobbyPressed()
    {
        OnLeaveLobbyButtonPressed?.Invoke();
    }

    public void PoppulateInvitableFriends()
    {
        //TODO
    }

    public void InviteFriend()
    {
        //TODO
    }
}
