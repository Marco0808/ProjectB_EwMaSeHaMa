using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using TMPro;

public class NetworkLobbyPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private GameObject leaderIcon;

    [Header("Character Selection")]
    [SerializeField] private Button characterSelectionButton;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private GameObject characterSelectionPrefab;

    [SyncVar] private bool _isReady;
    [SyncVar] private bool _isLeader;

    private LobbyMenu _lobbyMenu;

    public string DisplayName { get; private set; }
    public bool IsReady => _isReady;
    public Button StartGameButton => _lobbyMenu.StartGameButton;


    private void Awake()
    {
        _lobbyMenu = FindObjectOfType<LobbyMenu>();
    }

    private void Start()
    {
        transform.SetParent(_lobbyMenu.PlayerPanelContainer);
        transform.localScale = Vector3.one;
    }

    public override void OnStartAuthority()
    {
        LobbyMenu.OnStartButtonPressed += CmdStartGame;
        LobbyMenu.OnReadyButtonPressed += ToggleReadyState;
        LobbyMenu.OnLeaveLobbyButtonPressed += LeaveLobby;

        // Setup lobby UI according to player status
        CmdCheckIfLeader(NetworkServer.active && NetworkClient.isConnected);
        CmdSetDisplayName(SteamManager.Initialized ? SteamFriends.GetPersonaName() : "Foo");
        CmdSetReadyState(false);
        characterSelectionButton.interactable = true;
    }

    [Command]
    private void CmdCheckIfLeader(bool isHost)
    {
        // Become leader if we are the host
        _isLeader = isHost;

        RpcShowLeaderUI(connectionToClient, _isLeader);
        RpcSetLeaderIcon(_isLeader);
    }

    [TargetRpc]
    private void RpcShowLeaderUI(NetworkConnection target, bool isLeader)
    {
        _lobbyMenu.StartGameButton.gameObject.SetActive(isLeader);
        _lobbyMenu.PlayerInviteWindow.SetActive(isLeader);
    }

    [ClientRpc]
    private void RpcSetLeaderIcon(bool isLeader)
    {
        leaderIcon.SetActive(isLeader);
    }

    private void ToggleReadyState()
    {
        CmdSetReadyState(!_isReady);
    }

    [Command]
    private void CmdSetReadyState(bool isReady)
    {
        _isReady = isReady;

        RpcSetReadyButtonState(connectionToClient, _isReady);
        RpcSetPlayerPanelReady(_isReady);
        NetworkManagerHousework.Singleton.PlayerChangedReadyState();
    }

    [TargetRpc]
    private void RpcSetReadyButtonState(NetworkConnection target, bool isReady)
    {
        _lobbyMenu.SetReadyButtonState(isReady);
    }

    [ClientRpc]
    private void RpcSetPlayerPanelReady(bool isready)
    {
        displayNameText.color = _isReady ? Color.green : Color.yellow;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        //TODO Validate players display name
        DisplayName = displayName;
        RpcSetDisplayName(displayName);
    }

    [ClientRpc]
    private void RpcSetDisplayName(string displayName)
    {
        displayNameText.text = displayName;
    }

    [Command]
    public void CmdStartGame()
    {
        if (_isLeader) NetworkManagerHousework.Singleton.StartGame();
    }

    public void LeaveLobby()
    {
        // stop host if is host
        if (NetworkServer.active && NetworkClient.isConnected)
            NetworkManagerHousework.Singleton.StopHost();

        // stop client if client-only
        if (NetworkClient.isConnected)
            NetworkManagerHousework.Singleton.StopClient();
    }

    public override void OnStartClient()
    {
        NetworkManagerHousework.Singleton.LobbyPlayers.Add(this);
    }

    public override void OnStopClient()
    {
        NetworkManagerHousework.Singleton.LobbyPlayers.Remove(this);
    }

    public void ToggleCharacterSelection()
    {
        //TODO character selection
    }

    /* public void ShowCharacterSelection()
    {
        if (_currentPlayerSelection != null) return;

        _currentPlayerSelection = Instantiate(characterSelectionPrefab, LobbyMenu.LobbyPlayerParent.anchoredPosition, Quaternion.identity, LobbyMenu.LobbyPlayerParent);
        Transform layoutParent = _currentPlayerSelection.GetComponentInChildren<GridLayout>().transform;

        foreach (CharacterData character in gameData.AvailableCharacters)
            Instantiate(characterButtonPrefab, layoutParent);
    }

    public void CloseCharacterSelection()
    {
        Destroy(_currentPlayerSelection);
        _currentPlayerSelection = null;
    }

    private void OnCharacterSelected(CharacterData character)
    {
        // TODO Select character for player
    } */
}
