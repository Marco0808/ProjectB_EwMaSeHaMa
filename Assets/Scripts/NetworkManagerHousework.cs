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

    public static NetworkManagerHousework _singleton;
    public static NetworkManagerHousework Singleton
    {
        get
        {
            if (_singleton != null) return _singleton;
            return _singleton = NetworkManager.singleton as NetworkManagerHousework;
        }
    }

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    public List<NetworkLobbyPlayer> LobbyPlayers { get; } = new List<NetworkLobbyPlayer>();
    public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();

    public override void OnStartServer()
    {
        // Load spawnable prefabs for server
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
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

        // Disconnect new client if game is already in progress
        if (SceneManager.GetActiveScene().name != lobbyScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // If inside lobby, spawn and assign LobbyPlayer for new client
        if (SceneManager.GetActiveScene().name == lobbyScene)
        {
            NetworkLobbyPlayer lobbyPlayer = Instantiate(lobbyPlayerPrefab);
            lobbyPlayer.Initialize(this, LobbyPlayers.Count == 0);

            NetworkServer.AddPlayerForConnection(conn, lobbyPlayer.gameObject);
        }
        else base.OnServerAddPlayer(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            NetworkLobbyPlayer player = conn.identity.GetComponent<NetworkLobbyPlayer>();
            LobbyPlayers.Remove(player);
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

    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);

        Debug.Log($"Server change scene to {newSceneName}".Color("green"));

        // Change scene from lobby to game
        if (SceneManager.GetActiveScene().name == lobbyScene && newSceneName == gameScene)
        {
            for (int i = LobbyPlayers.Count - 1; i >= 0; i--)
            {
                NetworkConnection conn = LobbyPlayers[i].connectionToClient;
                NetworkGamePlayer gamePlayer = Instantiate(gamePlayerPrefab);
                gamePlayer.SetDisplayName(LobbyPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayer.gameObject);
            }
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log("Scene changed".Color("green"));

        if (sceneName == gameScene)
        {
            GameObject spawnSystem = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystem);
        }
    }

    public void PlayerChangedReadyState()
    {
        LobbyPlayers[0].StartGameButton.interactable = IsReadyToStart();
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers)
            return false;

        foreach (NetworkLobbyPlayer player in LobbyPlayers)
            if (!player.isReady)
                return false;

        return true;
    }

    public void LoadLobby()
    {
        ServerChangeScene(lobbyScene);
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == lobbyScene)
            if (IsReadyToStart())
                ServerChangeScene(gameScene);
            else return;
    }
}
