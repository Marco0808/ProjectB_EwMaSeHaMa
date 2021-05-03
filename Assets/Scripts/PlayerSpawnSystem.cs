using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;

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

        GameObject player = Instantiate(playerPrefab, _spawnPoints[_nextIndex].position, _spawnPoints[_nextIndex].rotation);
        NetworkServer.Spawn(player, conn);

        _nextIndex++;
    }
}
