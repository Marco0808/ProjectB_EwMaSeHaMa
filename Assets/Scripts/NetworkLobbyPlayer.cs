using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using TMPro;

public class NetworkLobbyPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject characterSelectionPrefab;
    [SerializeField] private LobbyPlayerPanel playerPanel;

    [SyncVar] public bool isReady;

    private LobbyMenu _lobbyMenu;
    private bool _isLeader;

    public string DisplayName { get; private set; }
    public Button StartGameButton => _lobbyMenu.StartGameButton;


    public override void OnStartAuthority()
    {
        _lobbyMenu = FindObjectOfType<LobbyMenu>();
        LobbyMenu.OnStartButtonPressed += CmdStartGame;
        LobbyMenu.OnReadyButtonPressed += CmdToggleReady;
        LobbyMenu.OnLeaveLobbyButtonPressed += LeaveLobby;

        CmdSetDisplayName(SteamManager.Initialized ? SteamFriends.GetPersonaName() : "Foo");
        //CmdSetCharacterSelctionButtonInteractable(true);
    }

    [Server]
    public void Initialize(NetworkManagerHousework networkManager, bool isLeader)
    {
        _isLeader = isLeader;

        if (isLeader) RpcShowLeaderUI(connectionToClient);

        RpcSetCharacterSelectionButtonInteractable(connectionToClient, true);
        RpcSetPlayerPanelParent();
    }

    [ClientRpc]
    private void RpcSetPlayerPanelParent()
    {
        playerPanel.transform.parent = _lobbyMenu.PlayerPanelContainer;
    }

    [Command]
    private void CmdSetCharacterSelctionButtonInteractable(bool interactable)
    {
        RpcSetCharacterSelectionButtonInteractable(connectionToClient, interactable);
    }

    [TargetRpc]
    private void RpcSetCharacterSelectionButtonInteractable(NetworkConnection target, bool interactable)
    {
        playerPanel.CharacterSelectionBtton.interactable = interactable;
    }

    [TargetRpc]
    private void RpcShowLeaderUI(NetworkConnection target)
    {
        _lobbyMenu.StartGameButton.gameObject.SetActive(true);
        _lobbyMenu.PlayerInviteWindow.SetActive(true);
    }

    [ClientRpc]
    private void SetLeaderIcon(bool isLeader)
    {
        playerPanel.LeaderIcon.SetActive(isLeader);
    }

    [Command]
    private void CmdToggleReady(GameObject readyButton)
    {
        isReady = !isReady;

        readyButton.GetComponent<Button>().image.color = isReady ? Color.green : Color.yellow;
        RpcSetReady(isReady);
        NetworkManagerHousework.Singleton.PlayerChangedReadyState();
    }

    [ClientRpc]
    private void RpcSetReady(bool isReady)
    {
        playerPanel.DisplayNameText.color = isReady ? Color.green : Color.green;
    }

    public void LeaveLobby()
    {
        // connectionToClient.Disconnect();
        NetworkClient.Disconnect();
    }

    public override void OnStartClient()
    {
        NetworkManagerHousework.Singleton.LobbyPlayers.Add(this);
    }

    public override void OnStopClient()
    {
        NetworkManagerHousework.Singleton.LobbyPlayers.Remove(this);
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
        playerPanel.DisplayNameText.text = displayName;
    }

    [Command]
    public void CmdStartGame()
    {
        Debug.Log("Trying to start game".Color("cyan"));
        // Check if we are the leader
        if (NetworkManagerHousework.Singleton.LobbyPlayers[0].connectionToClient == connectionToClient)
            NetworkManagerHousework.Singleton.StartGame();
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
