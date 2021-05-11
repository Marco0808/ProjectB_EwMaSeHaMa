using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Mirror;
using Steamworks;
using TMPro;
using System.Collections;

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

    [Header("Prefabs")]
    [SerializeField] private TaskMenu taskMenuPrefab;

    [SyncVar] private string sync_displayName = "foo";
    [SyncVar] private int sync_characterId;
    [SyncVar] private float sync_taskPoints = 0;

    private InputActions _input;
    private NavMeshAgent _agent;
    private TaskPointsBar _taskBar;

    private TaskObject _activeTaskObject;
    private TaskMenu _activeTaskMenu;
    private bool _isDoingTask;

    private Vector3 _previousCorner = Vector3.zero;


    private Queue<TaskWaypoint> _taskWaypoints = new Queue<TaskWaypoint>();

    public string DisplayName => sync_displayName;
    public float TaskPoints => sync_taskPoints;
    public CharacterData Character => gameData.GetCharacterById(sync_characterId);

    public struct TaskWaypoint
    {
        public TaskWaypoint(Vector3 position, string taskName)
        {
            Position = position;
            TaskName = taskName;
        }

        public Vector3 Position { get; private set; }
        public string TaskName { get; private set; }
    }

    public override void OnStartAuthority()
    {

        _input = new InputActions();
        _input.Game.LeftMouseButton.started += _ => SelectTaskObject();
        _input.Game.LeftMouseButton.canceled += _ => DestroyTaskMenu();
        _input.Enable();

        GameManager.Singleton.SetLocalPlayerColor(gameData.GetCharacterById(sync_characterId).Color);

        GameManager.OnLeaveGameButtonPressed += LeaveGame;

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

        _taskBar = GameManager.Singleton.PlayerProgressBars.GetAvailableTaskPointsBar();
    }

    public override void OnStopClient()
    {
        _taskBar?.Hide();
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
        if (hasAuthority && !_activeTaskMenu)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(_input.Game.MousePosition.ReadValue<Vector2>());
            Debug.DrawRay(mouseRay.origin, mouseRay.direction * maxMouseRayDistance, Color.green);

            if (Physics.Raycast(mouseRay, out RaycastHit newMouseHit, maxMouseRayDistance, clickableLayers))
            {
                // If another object was hit as before, set the new one highlighted and the old one not
                if (TryGetTaskObjectFromHit(newMouseHit, out TaskObject newTaskObject) && newTaskObject != _activeTaskObject)
                {
                    _activeTaskObject?.SetHighlighted(false);
                    newTaskObject.SetHighlighted(true);

                    _activeTaskObject = newTaskObject;
                }
            }
            // Stop highlighting previous TaskObject and clear it if nothing was hit
            else if (_activeTaskObject)
            {
                _activeTaskObject.SetHighlighted(false);
                _activeTaskObject = null;
            }
        }
    }

    private bool TryGetTaskObjectFromHit(RaycastHit hit, out TaskObject taskObject)
    {
        taskObject = null;
        if (hit.collider == null) return false;
        return hit.collider.CompareTag("TaskObject") && hit.collider.TryGetComponent(out taskObject);
    }

    private void SelectTaskObject()
    {
        // Create new task menu if none is open
        if (_activeTaskObject && !_activeTaskMenu)
        {
            _activeTaskObject.SetHighlighted(true);

            // Instantiate TaskMenu at active task object's menu root and rotate it to facie the camera
            _activeTaskMenu = Instantiate(taskMenuPrefab, _activeTaskObject.TaskMenuRoot.position, Camera.main.transform.rotation);
            _activeTaskMenu.Initialize(_activeTaskObject);

            TaskMenu.OnDoTask += AddTask;
            TaskMenu.OnPlaceTrap += PlaceTrap;
        }
    }

    private void DestroyTaskMenu()
    {
        if (_activeTaskMenu)
        {
            _activeTaskMenu.ExecuteMouseInput();

            // Clear task menu and unsubcribe from buttons
            TaskMenu.OnDoTask -= AddTask;
            TaskMenu.OnPlaceTrap -= PlaceTrap;
            Destroy(_activeTaskMenu.gameObject);
            _activeTaskMenu = null;
        }
    }

    private void AddTask(TaskObject taskObject)
    {
        Debug.Log($"Added Task {taskObject.TaskData.TaskName}".Color("yellow"));
        AddTaskWaypoint(taskObject.TaskPosition, taskObject.TaskData.TaskName);
    }

    private void PlaceTrap(TaskObject taskObject)
    {
        Debug.Log($"Placed Trap {taskObject.TaskData.TaskName}".Color("yellow"));
    }

    [Command]
    private void AddTaskWaypoint(Vector3 position, string taskName)
    {
        _taskWaypoints.Enqueue(new TaskWaypoint(position, taskName));
        UpdateTaskSchedule(connectionToClient, GetCurrentTaskSchedule());
    }

    private string[] GetCurrentTaskSchedule()
    {
        List<string> currentTaskNames = new List<string>();
        foreach (var waypoint in _taskWaypoints)
            currentTaskNames.Add(waypoint.TaskName);

        return currentTaskNames.ToArray();
    }

    [TargetRpc]
    private void UpdateTaskSchedule(NetworkConnection target, string[] scheduledTaskNames)
    {
        string taskSchedule = "Task Schedule: \n";
        for (int i = 0; i < scheduledTaskNames.Length; i++)
        {
            taskSchedule += $"{i + 1}. {scheduledTaskNames[i]} \n";
        }

        // GameManager.Singleton.TaskScheduleText.text = taskSchedule;
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        if (_taskWaypoints.Count > 0 && _agent.remainingDistance < 1f)
        {
            if (Vector3.Distance(_agent.transform.position, _taskWaypoints.Peek().Position) < 2f)
            {

                _taskWaypoints.Dequeue();
                UpdateTaskSchedule(connectionToClient, GetCurrentTaskSchedule());
                StartCoroutine(DoTask());
            }
            else if (!_isDoingTask)
            {
                _agent.SetDestination(_taskWaypoints.Peek().Position);
            }
        }

        //TODO Task pathing
        foreach (var corner in _agent.path.corners)
        {
            Debug.DrawLine(_previousCorner, corner, Color.blue, 20);
            _previousCorner = corner;
        }
    }

    [Server]
    private void CompleteTask()
    {
        //TODO Copleting tasks and adding their TaskPoint value
        sync_taskPoints += 0.1f;
        NetworkManagerHousework.Singleton.UpdatedPlayerTaskPoints(connectionToClient.connectionId);
        RpcUpdateTaskPoints(sync_taskPoints);
    }

    [ClientRpc]
    private void RpcUpdateTaskPoints(float taskPoints)
    {
        _taskBar?.SetTaskPoints(taskPoints);
    }

    private IEnumerator DoTask()
    {
        _isDoingTask = true;
        //TODO Task wait time
        yield return new WaitForSeconds(3);

        //TODO Complete task
        CompleteTask();
        _isDoingTask = false;
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
