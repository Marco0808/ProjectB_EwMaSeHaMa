using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab;

    private static List<Transform> _spawnPoints = new List<Transform>();

    private int _nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        _spawnPoints.Add(transform);
        _spawnPoints = _spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform)
    {
        _spawnPoints.Remove(transform);
    }

    public override void OnStartServer()
    {
        NetworkManagerHousework.OnServerReadied += SpawnPlayer;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        NetworkManagerHousework.OnServerReadied -= SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = _spawnPoints.ElementAtOrDefault(_nextIndex);
        if (spawnPoint == null)
        {
            Debug.LogError("Missing spawn point for player " + _nextIndex);
            return;
        }

        // spawn and authorize new player object
        NetworkGamePlayer gamePlayer = Instantiate(gamePlayerPrefab);
        gamePlayer.transform.position = _spawnPoints[_nextIndex].position;
        gamePlayer.gameObject.name = $"{gamePlayerPrefab.name} [connId={conn.connectionId}]";

        NetworkManagerHousework.Singleton.LobbyPlayers.TryGetValue(conn.connectionId, out NetworkLobbyPlayer lobbyPlayer);
        gamePlayer.SetupPlayer(lobbyPlayer.DisplayName, lobbyPlayer.CharacterId);

        // Destroy and replace connection's old player object if it still exist, otherwise add new player
        if (conn.identity != null)
        {
            NetworkServer.Destroy(conn.identity.gameObject);
            NetworkServer.ReplacePlayerForConnection(conn, gamePlayer.gameObject);
        }
        else NetworkServer.AddPlayerForConnection(conn, gamePlayer.gameObject);

        _nextIndex++;
    }
}
