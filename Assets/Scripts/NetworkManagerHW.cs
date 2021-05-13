using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkManagerHW : NetworkManager
{
    [Header("Lobby")]
    [SerializeField] private int minPlayers = 0;
    [Scene, SerializeField] private string lobbyScene;
    [SerializeField] private NetworkLobbyPlayer lobbyPlayerPrefab;

    [Header("Game")]
    [Scene, SerializeField] private string gameScene;
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab;
    [SerializeField] private GameObject spawnSystemPrefab;

    [Header("References")]
    [SerializeField] private GameData gameData;

    public static event Action<NetworkConnection> OnServerReadied;

    private static NetworkManagerHW _singleton;
    public static NetworkManagerHW Singleton
    {
        get
        {
            if (_singleton != null) return _singleton;
            return _singleton = singleton as NetworkManagerHW;
        }
    }

    private bool _isGameInProgress = false;

    /// <summary>Dictionary of all lobby players, with connectionId as key</summary>
    public Dictionary<int, NetworkLobbyPlayer> LobbyPlayers { get; set; } = new Dictionary<int, NetworkLobbyPlayer>();

    /// <summary>Dictionary of all game players, with connectionId as key</summary>
    public Dictionary<int, NetworkGamePlayer> GamePlayers { get; set; } = new Dictionary<int, NetworkGamePlayer>();
    public bool IsGameInProgress => _isGameInProgress;
    public GameData GameData => gameData;


    public override void OnStartServer()
    {
    }

    public override void OnStopServer()
    {
        LobbyPlayers.Clear();
    }

    public override void OnStartClient()
    {
        // Load spawnable prefabs for client
        foreach (GameObject prefab in Resources.LoadAll<GameObject>("SpawnablePrefabs"))
            NetworkClient.RegisterPrefab(prefab);
    }

    #region Winning and Losing Conditions
    public void TeamWin()
    {
        Debug.Log("The whole team has won!".Color("cyan"));
    }

    public void PlayerWin(int connectionId)
    {
        GamePlayers.TryGetValue(connectionId, out var player);
        Debug.Log($"The player {player.DisplayName} has won!".Color("cyan"));
    }

    public void PlayerDied(int connectionId)
    {
        GamePlayers.TryGetValue(connectionId, out var player);
        Debug.Log($"The player {player.DisplayName} died!".Color("cyan"));
    }

    public void UpdatedPlayerQuestPoints(int connectionId)
    {
        if (GamePlayers.TryGetValue(connectionId, out var player))
        {
            // check for team win
            float teamquestPoints = 0;
            foreach (NetworkGamePlayer p in GamePlayers.Values)
                teamquestPoints += p.QuestPoints;
            if (teamquestPoints >= gameData.MaxQuestPoints * 4 * gameData.TeamWinPointPercentage)
                TeamWin();

            // check for player solo win
            if (player.QuestPoints >= gameData.MaxQuestPoints)
                PlayerWin(connectionId);
        }
    }

    public void UpdatedPlayerInsanityPoints(int connectionId)
    {
        if (GamePlayers.TryGetValue(connectionId, out var player))
        {
            if (player.InsanityPoints >= gameData.MaxInsanityPoints)
                PlayerDied(connectionId);
        }

    }
    #endregion

    #region Lobby and Connection
    public override void OnServerConnect(NetworkConnection conn)
    {
        // Disconnect new client if lobby is full
        if (numPlayers >= maxConnections)
        {
            //TODO Send server full message
            Debug.Log("New client disconnected. Lobby is full.".Color("green"));
            conn.Disconnect();
            return;
        }

        //Disconnect new client if game is already in progress
        if (_isGameInProgress)
        {
            //TODO Send game in progress message
            Debug.Log("New client disconnected. Game is already in progress.".Color("green"));
            conn.Disconnect();
            return;
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

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        OnServerReadied?.Invoke(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // If inside lobby, spawn and assign LobbyPlayer for new client
        if (!_isGameInProgress)
        {
            NetworkLobbyPlayer lobbyPlayer = Instantiate(lobbyPlayerPrefab);
            lobbyPlayer.gameObject.name = $"{lobbyPlayerPrefab.name} [connId={conn.connectionId}]";
            lobbyPlayer.LobbyPlayerPanel.name = $"{lobbyPlayerPrefab.name}Panel [connId={conn.connectionId}]";

            NetworkServer.AddPlayerForConnection(conn, lobbyPlayer.gameObject);
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log($"Server changed scene to {sceneName}".Color("green"));

        if (sceneName == gameScene)
        {
            GameObject spawnSystem = Instantiate(spawnSystemPrefab);
            NetworkServer.Spawn(spawnSystem);
        }
    }

    [Server]
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

    [Server]
    public void StartGame()
    {
        if (!_isGameInProgress && IsReadyToStart())
        {
            Debug.Log("Starting game".Color("cyan"));
            _isGameInProgress = true;

            ServerChangeScene(gameScene);
        }
        else return;
    }
    #endregion
}
