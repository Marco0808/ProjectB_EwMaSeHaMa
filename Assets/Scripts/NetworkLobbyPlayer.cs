using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using TMPro;

public class NetworkLobbyPlayer : NetworkBehaviour
{
    [SerializeField] private RectTransform lobbyPlayerPanel;
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private GameObject leaderIcon;

    [Header("Character Selection")]
    [SerializeField] private Button characterSelectionButton;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private GameObject characterSelectionPrefab;

    [SyncVar] private bool sync_isLeader;
    [SyncVar] private bool sync_isReady;

    [SyncVar(hook = nameof(DisplayNameChanged))]
    private string sync_displayName;

    private LobbyMenu _lobbyMenu;

    public RectTransform LobbyPlayerPanel => lobbyPlayerPanel;
    public bool IsLeader => sync_isLeader;
    public bool IsReady => sync_isReady;
    public string DisplayName => sync_displayName;
    public Button StartGameButton => _lobbyMenu.StartGameButton;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _lobbyMenu = FindObjectOfType<LobbyMenu>();
    }

    private void Start()
    {
        lobbyPlayerPanel.SetParent(_lobbyMenu.PlayerPanelContainer);
        lobbyPlayerPanel.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        Destroy(lobbyPlayerPanel);
    }

    public override void OnStartAuthority()
    {
        LobbyMenu.OnStartButtonPressed += CmdStartGame;
        LobbyMenu.OnReadyButtonPressed += ToggleReadyState;
        LobbyMenu.OnLeaveLobbyButtonPressed += LeaveLobby;

        CmdCheckIfLeader();
        CmdSetReadyState(false);
        CmdSetDisplayName(SteamManager.Initialized ? SteamFriends.GetPersonaName() : "Foo");
        characterSelectionButton.interactable = true;
    }

    [Command]
    private void CmdCheckIfLeader()
    {
        // Become leader if we are the host
        sync_isLeader = connectionToClient.connectionId == 0;
        UpdateLeaderUI();
    }

    [ClientRpc]
    private void UpdateLeaderUI()
    {
        leaderIcon.SetActive(sync_isLeader);

        if (hasAuthority)
        {
            _lobbyMenu.StartGameButton.gameObject.SetActive(sync_isLeader);
            _lobbyMenu.PlayerInviteWindow.SetActive(sync_isLeader);
        }
    }

    private void ToggleReadyState()
    {
        CmdSetReadyState(!sync_isReady);
    }

    [Command]
    private void CmdSetReadyState(bool isReady)
    {
        sync_isReady = isReady;
        RpcUpdateReadyState();
        NetworkManagerHousework.Singleton.PlayerChangedReadyState();
    }

    [ClientRpc]
    private void RpcUpdateReadyState()
    {
        // Set state of client's ready button if this is their player object
        if (hasAuthority) _lobbyMenu.SetReadyButtonState(sync_isReady);

        displayNameText.color = sync_isReady ? Color.green : Color.yellow;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        sync_displayName = displayName;
    }

    private void DisplayNameChanged(string oldValue, string newValue)
    {
        displayNameText.text = sync_displayName;
    }

    [Command]
    public void CmdStartGame()
    {
        if (sync_isLeader) NetworkManagerHousework.Singleton.StartGame();
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
        NetworkManagerHousework.Singleton.LobbyPlayers.Add(connectionToClient.connectionId, this);
        Debug.Log($"This client's connection ID: {connectionToClient.connectionId}".Color("green"));
    }

    public override void OnStopClient()
    {
        NetworkManagerHousework.Singleton.LobbyPlayers.Remove(connectionToClient.connectionId);
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
