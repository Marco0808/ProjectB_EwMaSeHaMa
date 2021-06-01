using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab;

    private static Dictionary<int, Transform> _spawnPoints = new Dictionary<int, Transform>();

    public static void AddCharacterSpawnPoint(CharacterData character, Transform transform)
    {
        if (NetworkManagerHW.Singleton.GameData.TryGetCharacterId(character, out int characterId))
            _spawnPoints.Add(characterId, transform);
    }

    public static void RemoveSpawnPoint(CharacterData character)
    {
        if (NetworkManagerHW.Singleton.GameData.TryGetCharacterId(character, out int characterId))
            _spawnPoints.Remove(characterId);
    }

    public override void OnStartServer()
    {
        NetworkManagerHW.OnServerReadied += SpawnPlayer;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        NetworkManagerHW.OnServerReadied -= SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        if (NetworkManagerHW.Singleton.LobbyPlayers.TryGetValue(conn.connectionId, out NetworkLobbyPlayer lobbyPlayer) &&
            _spawnPoints.TryGetValue(lobbyPlayer.CharacterId, out Transform spawnPoint))
        {
            // spawn and authorize new player object
            NetworkGamePlayer gamePlayer = Instantiate(gamePlayerPrefab);
            gamePlayer.transform.position = spawnPoint.position;
            gamePlayer.gameObject.name = $"{gamePlayerPrefab.name} [connId={conn.connectionId}]";

            gamePlayer.SetupPlayer(lobbyPlayer.DisplayName, lobbyPlayer.CharacterId, NetworkManagerHW.Singleton.numPlayers);

            // Destroy and replace connection's old player object if it still exist, otherwise add new player
            if (conn.identity != null)
            {
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayer.gameObject);
            }
            else NetworkServer.AddPlayerForConnection(conn, gamePlayer.gameObject);
        }
        else Debug.LogError("Missing spawn point for player " + conn.connectionId);
    }
}
