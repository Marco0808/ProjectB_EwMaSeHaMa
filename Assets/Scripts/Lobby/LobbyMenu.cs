using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class LobbyMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform playerPanelContainer;
    [SerializeField] private Transform invitableFriendsContainer;
    [SerializeField] private InvitableSteamFriend invitableFriendPrefab;

    [Header("Ready and Start Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TMP_Text readyButtonText;
    [SerializeField] private Image readyButtonColorImage;
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color readyUpColor = Color.yellow;

    public static event Action OnStartButtonPressed;
    public static event Action OnReadyButtonPressed;
    public static event Action OnLeaveLobbyButtonPressed;

    public RectTransform PlayerPanelContainer => playerPanelContainer;
    public Button StartGameButton => startGameButton;


    private void Start()
    {
        PoppulateInvitableFriends();
    }

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
        readyButtonColorImage.color = isReady ? readyColor : readyUpColor;
        readyButtonText.text = isReady ? "Ready" : "Ready Up";
    }

    public void LeaveLobbyPressed()
    {
        OnLeaveLobbyButtonPressed?.Invoke();
    }

    public void PoppulateInvitableFriends()
    {
        if (SteamLobby.LobbyId.m_SteamID == 0) return;

        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

        for (int i = 0; i < friendCount; i++)
        {
            CSteamID friendId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);

            if (SteamFriends.GetFriendPersonaState(friendId) == EPersonaState.k_EPersonaStateOffline) continue;

            Instantiate(invitableFriendPrefab, invitableFriendsContainer).Initalize(friendId);
        }
    }

    public void InviteFriend()
    {
        //TODO
    }
}
