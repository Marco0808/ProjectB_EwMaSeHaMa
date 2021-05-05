using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Mirror;
using Steamworks;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class NetworkGamePlayer : NetworkBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private MeshRenderer meshRenderer;

    [SyncVar] private string sync_displayName = "foo";
    [SyncVar] private int sync_characterId;

    private InputActions _input;
    private GameMenu _gameMenu;
    private NavMeshAgent _agent;

    public string DisplayName => sync_displayName;
    public CharacterData Character => gameData.GetCharacterById(sync_characterId);


    public override void OnStartAuthority()
    {

        _input = new InputActions();
        _input.Game.RightMouseButton.performed += _ => MoveToClick();
        _input.Enable();

        _gameMenu = FindObjectOfType<GameMenu>();
        _gameMenu.SetLocalPlayerColor(gameData.GetCharacterById(sync_characterId).Color);

        GameMenu.OnLeaveGameButtonPressed += LeaveGame;

        CmdEnableNavAgent();
    }

    public override void OnStopAuthority()
    {
        _input.Disable();
    }

    public override void OnStartClient()
    {
        displayNameText.text = sync_displayName;
        meshRenderer.material.color = Character.Color;
    }

    public override void OnStartServer()
    {
        NetworkManagerHousework.Singleton.GamePlayers.Add(connectionToClient.connectionId, this);
    }

    public override void OnStopServer()
    {
        NetworkManagerHousework.Singleton.GamePlayers.Remove(connectionToClient.connectionId);
    }

    public void MoveToClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(_input.Game.MousePosition.ReadValue<Vector2>());

        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 2);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo))
            CmdRequestMoveTo(hitInfo.point);
    }

    [Command]
    private void CmdRequestMoveTo(Vector3 position)
    {
        _agent.destination = position;
    }

    [Command]
    private void CmdEnableNavAgent()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        _agent.updateRotation = false;
    }

    [Server]
    public void SetupPlayer(string displayname, int characterId)
    {
        sync_displayName = displayname;
        sync_characterId = characterId;
    }

    public void LeaveGame()
    {
        // stop host if is host
        if (NetworkServer.active && NetworkClient.isConnected)
            NetworkManagerHousework.Singleton.StopHost();

        // stop client if client-only
        if (NetworkClient.isConnected)
            NetworkManagerHousework.Singleton.StopClient();
    }
}
