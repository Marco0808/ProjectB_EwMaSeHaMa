using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using TMPro;

public class NetworkGamePlayer : NetworkBehaviour
{

    private RectTransform _lobbyPlayerTrans;
    private GameObject _currentPlayerSelection;
    private bool isLeader;

    [SyncVar] private string _displayName;

    private NetworkManagerHousework _lobby;
    private NetworkManagerHousework Lobby
    {
        get
        {
            if (_lobby != null) return _lobby;
            return _lobby = NetworkManager.singleton as NetworkManagerHousework;
        }
    }

    public override void OnStartClient()
    {
        Lobby.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Lobby.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayname)
    {
        _displayName = displayname;
    }
}
