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
    [SerializeField] private GameData gameData;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private CharacterSelectionButton characterSelectionButtonPrefab;
    [SerializeField] private GameObject characterSelectionPrefab;

    [SyncVar] private bool sync_isLeader = false;
    [SyncVar] private bool sync_isReady = false;

    [SyncVar(hook = nameof(DisplayNameChanged))] private string sync_displayName = "foo";
    [SyncVar(hook = nameof(CharacterIdChanged))] private int sync_characterId;

    private LobbyMenu _lobbyMenu;
    private GameObject _currentCharacterSelection;

    public RectTransform LobbyPlayerPanel => lobbyPlayerPanel;
    public bool IsLeader => sync_isLeader;
    public bool IsReady => sync_isReady;
    public string DisplayName => sync_displayName;
    public int CharacterId => sync_characterId;
    public Button StartGameButton => _lobbyMenu.StartGameButton;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _lobbyMenu = FindObjectOfType<LobbyMenu>();
    }

    private void OnDestroy()
    {
        if (lobbyPlayerPanel) Destroy(lobbyPlayerPanel.gameObject);
    }

    public override void OnStartClient()
    {
        if (_lobbyMenu) lobbyPlayerPanel.SetParent(_lobbyMenu.PlayerPanelContainer);
        lobbyPlayerPanel.localScale = Vector3.one;

        // Update lobby player UI if we dont have autority
        if (!hasAuthority)
        {
            displayNameText.color = sync_isReady ? Color.green : Color.yellow;
            leaderIcon.SetActive(sync_isLeader);
        }
    }

    public override void OnStopClient()
    {
        if (lobbyPlayerPanel) Destroy(lobbyPlayerPanel.gameObject);
    }

    public override void OnStartAuthority()
    {
        LobbyMenu.OnStartButtonPressed += CmdStartGame;
        LobbyMenu.OnReadyButtonPressed += ToggleReadyState;
        LobbyMenu.OnLeaveLobbyButtonPressed += LeaveLobby;

        CmdCheckIfLeader();
        CmdSetReadyState(false);
        CmdSetDisplayName(SteamManager.Initialized ? SteamFriends.GetPersonaName() : null);
        characterSelectionButton.interactable = true;
    }

    public override void OnStartServer()
    {
        NetworkManagerHW.Singleton.LobbyPlayers.Add(connectionToClient.connectionId, this);
    }

    public override void OnStopServer()
    {
        NetworkManagerHW.Singleton.LobbyPlayers.Remove(connectionToClient.connectionId);
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
        RpcUpdateReadyState(isReady);
        NetworkManagerHW.Singleton.PlayerChangedReadyState();
    }

    [ClientRpc]
    private void RpcUpdateReadyState(bool isReady)
    {
        // Set state of client's ready button if this is their player object
        if (hasAuthority) _lobbyMenu.SetReadyButtonState(isReady);

        displayNameText.color = isReady ? Color.green : Color.yellow;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        if (displayName == null)
            displayName = $"Player{connectionToClient.connectionId}";
        sync_displayName = displayName;
    }

    private void DisplayNameChanged(string oldName, string newName)
    {
        displayNameText.text = sync_displayName;
    }

    [Command]
    public void CmdStartGame()
    {
        if (sync_isLeader) NetworkManagerHW.Singleton.StartGame();
    }

    public void LeaveLobby()
    {
        // stop host if is host
        if (NetworkServer.active && NetworkClient.isConnected)
            NetworkManagerHW.Singleton.StopHost();

        // stop client if client-only
        if (NetworkClient.isConnected)
            NetworkManagerHW.Singleton.StopClient();
    }

    public void ToggleCharacterSelection()
    {
        if (_currentCharacterSelection == null)
        {
            // Open character selection
            _currentCharacterSelection = Instantiate(characterSelectionPrefab, lobbyPlayerPanel);
            _currentCharacterSelection.transform.localPosition = new Vector3(_currentCharacterSelection.GetComponent<RectTransform>().sizeDelta.x, 0, 0);

            GridLayoutGroup buttonGridContainer = _currentCharacterSelection.GetComponentInChildren<GridLayoutGroup>();

            // Poppulate grid with a button for each available character
            for (int i = 0; i < gameData.Characters.Length; i++)
                Instantiate(characterSelectionButtonPrefab, buttonGridContainer.transform).SetCharacter(gameData.GetCharacterById(i));

            CharacterSelectionButton.OnCharacterSelected += CharacterSelected;
        }
        else CloseCharacterSelection();
    }

    public void CloseCharacterSelection()
    {
        Destroy(_currentCharacterSelection.gameObject);
        _currentCharacterSelection = null;

        CharacterSelectionButton.OnCharacterSelected -= CharacterSelected;
    }

    private void CharacterSelected(CharacterData character)
    {
        if (gameData.TryGetCharacterId(character, out int characterId))
            CmdSetCharacterById(characterId);
        CloseCharacterSelection();
    }

    [Command]
    private void CmdSetCharacterById(int characterId) => sync_characterId = characterId;

    private void CharacterIdChanged(int oldId, int newId) => characterPortrait.sprite = gameData.GetCharacterById(newId).Portrait;
}
