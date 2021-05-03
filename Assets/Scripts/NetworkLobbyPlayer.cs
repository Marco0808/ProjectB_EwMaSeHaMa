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

    [Header("Character Selection")]
    [SerializeField] private GameData gameData;
    [SerializeField] private GameObject characterSelectionPrefab;
    [SerializeField] private GameObject characterButtonPrefab;

    private GameObject _currentPlayerSelection;
    private bool _isLeader;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string displayName;

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool isReady;

    public bool IsLeader
    {
        get => _isLeader;
        set
        {
            _isLeader = value;
            LobbyMenu.StartGameButton.gameObject.SetActive(true);
        }
    }

    private NetworkManagerHousework _lobby;
    private NetworkManagerHousework Lobby
    {
        get
        {
            if (_lobby != null) return _lobby;
            return _lobby = NetworkManager.singleton as NetworkManagerHousework;
        }
    }

    private void Awake()
    {
        Debug.Log("Awake".Color("yellow"));
        LobbyMenu.OnReadyButtonPressed += CmdReadyUp;
        LobbyMenu.OnStartGameButtonPressed += CmdStartGame;

        CharacterSelectionButton.OnCharacterSelected += OnCharacterSelected;
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(SteamManager.Initialized ? SteamFriends.GetPersonaName() : "Foo");
    }

    public override void OnStartClient()
    {
        Lobby.LobbyPlayers.Add(this);
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Lobby.LobbyPlayers.Remove(this);
        UpdateDisplay();
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) { UpdateDisplay(); }
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) { UpdateDisplay(); }

    private void UpdateDisplay()
    {
        // if UpdateDisplay on a NetworkLobbyPlayer that is not the local one, find the local one and try again
        if (!hasAuthority)
        {
            foreach (NetworkLobbyPlayer player in Lobby.LobbyPlayers)
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            return;
        }

        displayNameText.text = displayName;
        displayNameText.color = isReady ? Color.green : Color.red;
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (_isLeader)
            LobbyMenu.StartGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        Debug.Log("Readying up".Color("cyan"));
        isReady = !isReady;
        Lobby.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        Debug.Log("Trying to start game".Color("cyan"));
        // Check if we are the leader
        if (Lobby.LobbyPlayers[0].connectionToClient == connectionToClient)
        {
            Lobby.StartGame();
        }
    }

    // OWN METHODS
    public void ShowCharacterSelection()
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

    private void SpawnLobbyPlayerUI()
    {
        //TODO
    }

    private void OnCharacterSelected(CharacterData character)
    {
        // TODO Select character for player
    }
}
