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
    [Header("Input")]
    [SerializeField] private LayerMask clickableLayers;
    [SerializeField] private float maxMouseRayDistance = 50;

    [Header("References")]
    [SerializeField] private GameData gameData;
    [SerializeField] private TMP_Text displayNameText;
    [SerializeField] private MeshRenderer meshRenderer;

    [SyncVar] private string sync_displayName = "foo";
    [SyncVar] private int sync_characterId;

    private InputActions _input;
    private GameMenu _gameMenu;
    private NavMeshAgent _agent;

    private TaskObject _highlightedTaskObject;

    public string DisplayName => sync_displayName;
    public CharacterData Character => gameData.GetCharacterById(sync_characterId);


    public override void OnStartAuthority()
    {

        _input = new InputActions();
        _input.Game.RightMouseButton.performed += _ => TryMoveToTaskObject();
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

    [ClientCallback]
    public void Update()
    {
        // Run mouse input behavior if we have authority
        if (hasAuthority)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(_input.Game.MousePosition.ReadValue<Vector2>());
            Debug.DrawRay(mouseRay.origin, mouseRay.direction * maxMouseRayDistance, Color.green);

            if (Physics.Raycast(mouseRay, out RaycastHit newMouseHit, maxMouseRayDistance, clickableLayers))
            {
                // If another object was hit as before, set the new one highlighted and the old one not
                if (TryGetTaskObjectFromHit(newMouseHit, out TaskObject newTaskObject) && newTaskObject != _highlightedTaskObject)
                {
                    _highlightedTaskObject?.SetHighlighted(false);
                    newTaskObject.SetHighlighted(true);

                    _highlightedTaskObject = newTaskObject;
                }
            }
            // Stop highlighting previous TaskObject and clear it if nothing was hit
            else if (_highlightedTaskObject)
            {
                _highlightedTaskObject.SetHighlighted(false);
                _highlightedTaskObject = null;
            }
        }
    }

    private bool TryGetTaskObjectFromHit(RaycastHit hit, out TaskObject taskObject)
    {
        taskObject = null;
        if (hit.collider == null) return false;
        return hit.collider.CompareTag("TaskObject") && hit.collider.TryGetComponent(out taskObject);
    }

    private void TryMoveToTaskObject()
    {
        if (_highlightedTaskObject)
            CmdSetPlayerDestination(_highlightedTaskObject.TaskPosition);
    }

    [Command]
    private void CmdSetPlayerDestination(Vector3 position)
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
