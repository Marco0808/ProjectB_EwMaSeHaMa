using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] CharacterData characterToSpawn;

    private void Awake()
    {
        PlayerSpawnSystem.AddCharacterSpawnPoint(characterToSpawn, transform);
    }

    private void OnDestroy()
    {
        PlayerSpawnSystem.RemoveSpawnPoint(characterToSpawn);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
