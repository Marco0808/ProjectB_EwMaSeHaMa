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
    [SerializeField] private TMP_Text displayNameText;

    [SyncVar(hook = nameof(DisplayNameChanged))]
    private string sync_displayName = "foo";

    private InputActions _input;
    private NavMeshAgent _agent;

    public string DisplayName => sync_displayName;

    public override void OnStartAuthority()
    {
        _input = new InputActions();
        _input.Game.RightMouseButton.performed += _ => MoveToClick();
        _input.Enable();

        CmdEnableNavAgent();
    }

    public override void OnStopAuthority()
    {
        _input.Disable();
    }

    public override void OnStartClient()
    {
        NetworkManagerHousework.Singleton.GamePlayers.Add(connectionToClient.connectionId, this);

        UpdateDisplayName();
    }

    public override void OnStopClient()
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

    private void DisplayNameChanged(string oldValue, string newValue)
    {
        UpdateDisplayName();
    }

    private void UpdateDisplayName()
    {
        displayNameText.text = sync_displayName;
    }

    [Server]
    public void SetDisplayName(string displayname)
    {
        sync_displayName = displayname;
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
    }
}
