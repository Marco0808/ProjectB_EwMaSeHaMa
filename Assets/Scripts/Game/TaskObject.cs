using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(SpriteRenderer))]
public class TaskObject : NetworkBehaviour
{
    [SerializeField, Range(0.5f, 1)] private float highlightValue = 1;
    [SerializeField] private GameObject trapIndicatorPrefab;
    [SerializeField] private Transform taskMenuRoot;

    [SyncVar] private bool sync_isTrapActive;
    /// <summary>List of game player connectionId's, that working on this task.</summary>
    private SyncHashSet<int> sync_workingPlayerIDs = new SyncHashSet<int>();

    private TaskData _task;
    private SpriteRenderer _spriteRenderer;
    private Collider _collider;
    private GameObject _trapIndicator;

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

    /// <summary>Assign a player to a task and return another player that is currently on the task. If there is none, return false.</summary>
    [Server]
    public bool AssignPlayerToTask(int playerConnectionId, out int otherPlayerConnectionId)
    {
        sync_workingPlayerIDs.Add(playerConnectionId);

        // get copy of working players list without self
        List<int> otherPlayerIDs = new List<int>(sync_workingPlayerIDs);
        otherPlayerIDs.Remove(playerConnectionId);

        if (otherPlayerIDs.Count > 0)
        {
            otherPlayerConnectionId = otherPlayerIDs[0];
            return true;
        }
        else
        {
            otherPlayerConnectionId = -1;
            return false;
        }
    }

    public bool PlayerAlreadyAssigned(int playerConnectionId)
    {
        return sync_workingPlayerIDs.Contains(playerConnectionId);
    }

    [Server]
    public void RemoveAssignedPlayer(int connectionId)
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
        if (isTrapped)
        {
            if (_trapIndicator) Destroy(_trapIndicator);
            _trapIndicator = Instantiate(trapIndicatorPrefab, taskMenuRoot.position, Camera.main.transform.rotation);
        }
        else if (_trapIndicator)
        {
            _trapIndicator = null;
            Destroy(_trapIndicator);
        }
    }
    #endregion

    public void SetHighlighted(bool isHighlighted)
    {
        SetSpriteBrightness(isHighlighted ? highlightValue : 0.5f);
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
