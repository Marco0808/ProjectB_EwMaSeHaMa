using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(SpriteRenderer))]
public class TaskObject : NetworkBehaviour
{
    [SerializeField] private Color markTrappedColor;
    [SerializeField] private Transform taskMenuRoot;

    [SyncVar] private bool sync_isTrapActive;
    /// <summary>List of game player connectionId's, that working on this task.</summary>
    private SyncList<int> sync_workingPlayerIDs = new SyncList<int>();

    private TaskData _task;
    private SpriteRenderer _spriteRenderer;
    private Collider _collider;

    public TaskData Task => _task;
    public Transform TaskMenuRoot => taskMenuRoot;
    public Vector3 TaskPosition => transform.position;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _task = GetComponent<TaskObjectUpdater>().Task;
    }

    private void Start()
    {
        _task.TaskObject = this;

        _spriteRenderer.sprite = _task.ObjectSprite;
        StartCoroutine(ResetCollider());
    }

    [Server]
    public bool AddAndGetWorkingPlayer(int addedConnectionId, out int otherConnectionId)
    {
        int[] otherPlayerIDs = new int[sync_workingPlayerIDs.Count];
        sync_workingPlayerIDs.CopyTo(otherPlayerIDs, 0);
        sync_workingPlayerIDs.Add(addedConnectionId);

        if (otherPlayerIDs.Length > 0)
        {
            otherConnectionId = otherPlayerIDs[0];
            return true;
        }
        else
        {
            otherConnectionId = -1;
            return false;
        }
    }

    [Server]
    public void RemoveWorkingPlayer(int connectionId)
    {
        sync_workingPlayerIDs.Remove(connectionId);
    }

    #region Trap System
    /// <param name="trapperConn">Connection to the client who placed the trap.</param>
    [Server]
    public void ActivateTrap(NetworkConnection trapperConn)
    {
        sync_isTrapActive = true;
        RpcMarkAsTrapped(trapperConn);
    }

    [TargetRpc]
    private void RpcMarkAsTrapped(NetworkConnection target) { MarkAsTrapped(true); }

    [Server]
    public bool CheckForTrap(out float timeDelay)
    {
        timeDelay = NetworkManagerHW.Singleton.GameData.TrapTaskDelayTime;

        if (sync_isTrapActive)
        {
            sync_isTrapActive = false;
            RpcClearTrap();
            return true;
        }
        return false;
    }

    [ClientRpc]
    private void RpcClearTrap() { MarkAsTrapped(false); }

    private void MarkAsTrapped(bool isTrapped)
    {
        Color color = isTrapped ? markTrappedColor : Color.white;
        color.a = _spriteRenderer.color.a;
        _spriteRenderer.color = color;
    }
    #endregion

    public void SetHighlighted(bool isHighlighted)
    {
        SetSpriteBrightness(isHighlighted ? 1 : 0.5f);
    }

    /// <param name="brightness">Value between 0 and 1. Default brightness is 0.5</param>
    private void SetSpriteBrightness(float brightness)
    {
        Color color = _spriteRenderer.color;
        color.a = Mathf.Clamp01(brightness);
        _spriteRenderer.color = color;
    }

    IEnumerator ResetCollider()
    {
        if (_collider) Destroy(_collider);
        yield return null;
        _collider = gameObject.AddComponent<BoxCollider>();
        _collider.isTrigger = true;
    }
}
