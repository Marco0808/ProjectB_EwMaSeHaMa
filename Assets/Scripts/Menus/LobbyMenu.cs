using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private Transform playerPanelContainer;
    [SerializeField] private GameObject playerInviteWindow;
    [SerializeField] private Transform playerInviteContainer;
    [SerializeField] private GameObject invitableFriendPrefab;
    [SerializeField] private Button startGameButton;

    public static event Action OnStartButtonPressed;
    public static event Action<GameObject> OnReadyButtonPressed;
    public static event Action OnLeaveLobbyButtonPressed;


    public Transform PlayerPanelContainer => playerPanelContainer;
    public GameObject PlayerInviteWindow => playerInviteWindow;
    public Button StartGameButton => startGameButton;


    public void StartButtonPressed()
    {
        OnStartButtonPressed?.Invoke();
    }

    public void ReadyButtonPressed(Button readyButton)
    {
        OnReadyButtonPressed?.Invoke(readyButton.gameObject);
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
