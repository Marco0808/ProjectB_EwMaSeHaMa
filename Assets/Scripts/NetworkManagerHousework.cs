using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkManagerHousework : NetworkManager
{
    [Header("Lobby")]
    [SerializeField] private int minPlayers = 0;
    [Scene, SerializeField] private string lobbyScene;
    [SerializeField] private NetworkLobbyPlayer lobbyPlayerPrefab;

    [Header("Game")]
    [Scene, SerializeField] private string gameScene;
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab;
    [SerializeField] private GameObject playerSpawnSystem;

    private static NetworkManagerHousework _singleton;
    public static NetworkManagerHousework Singleton
    {
        get
        {
            if (_singleton != null) return _singleton;
            return _singleton = singleton as NetworkManagerHousework;
        }
    }

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    /// <summary>Dictionary of all lobby players, with connectionId as key</summary>
    public Dictionary<int, NetworkLobbyPlayer> LobbyPlayers { get; set; } = new Dictionary<int, NetworkLobbyPlayer>();

    /// <summary>Dictionary of all game players, with connectionId as key</summary>
    public Dictionary<int, NetworkGamePlayer> GamePlayers => new Dictionary<int, NetworkGamePlayer>();
    public bool IsGameInProgress { get; private set; }

    public override void OnStartServer()
    {
        //TODO Loading spawnables for server needed or not?
        //spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        IsGameInProgress = false;
    }

    public override void OnStartClient()
    {
        // Load spawnable prefabs for client
        foreach (GameObject prefab in Resources.LoadAll<GameObject>("SpawnablePrefabs"))
            NetworkClient.RegisterPrefab(prefab);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        // Disconnect new client if lobby is full
        if (numPlayers >= maxConnections)
        {
            //TODO Send server full message
            conn.Disconnect();
            return;
        }

        //TODO Disconnect new client if game is already in progress
        if (IsGameInProgress)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // If inside lobby, spawn and assign LobbyPlayer for new client
        if (!IsGameInProgress)
        {
            NetworkLobbyPlayer lobbyPlayer = Instantiate(lobbyPlayerPrefab);
            lobbyPlayer.gameObject.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            lobbyPlayer.LobbyPlayerPanel.name = $"{playerPrefab.name}Panel [connId={conn.connectionId}]";

            NetworkServer.AddPlayerForConnection(conn, lobbyPlayer.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            LobbyPlayers.Remove(conn.connectionId);
            PlayerChangedReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        LobbyPlayers.Clear();
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log($"Server changed scene to {sceneName}".Color("green"));
    }

    public void PlayerChangedReadyState()
    {
        // check if everyone is ready and set the host's start game button accordingly
        foreach (var item in LobbyPlayers)
            if (item.Value.IsLeader)
                item.Value.StartGameButton.interactable = IsReadyToStart();
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers)
            return false;

        foreach (var item in LobbyPlayers)
            if (!item.Value.IsReady)
                return false;

        return true;
    }

    public void StartGame()
    {
        if (!IsGameInProgress && IsReadyToStart())
        {
            Debug.Log("Starting game".Color("cyan"));
            IsGameInProgress = true;

            ServerChangeScene(gameScene);
        }
        else return;
    }
}
