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
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator; 

    [Header("Prefabs")]
    [SerializeField] private TaskMenu taskMenuPrefab;

    [Header("Waypoint Path")]
    [SerializeField] private Material taskPathMat;
    [SerializeField] private Material trapPathMat;
    [SerializeField] private LineRenderer waypointPathPrefab;

    [SyncVar] private string sync_displayName = "foo";
    [SyncVar] private int sync_characterId;
    [SyncVar] private float sync_taskPoints = 0;

    private InputActions _input;
    private NavMeshAgent _agent;
    private TaskPointsBar _taskBar;

    private TaskObject _activeTaskObject;
    private TaskMenu _activeTaskMenu;
    private bool _isDoingTask;
    private float _questCooldown;

    private List<QuestData> _activeQuests = new List<QuestData>();
    private Dictionary<QuestData, QuestPanel> _activeQuestPanels = new Dictionary<QuestData, QuestPanel>();
    private Queue<TaskWaypoint> _taskWaypoints = new Queue<TaskWaypoint>();
    private Dictionary<int, GameObject> _waypointPaths = new Dictionary<int, GameObject>();

    public string DisplayName => sync_displayName;
    public float TaskPoints => sync_taskPoints;
    public CharacterData Character => gameData.GetCharacterById(sync_characterId);

    public struct TaskWaypoint
    {
        public TaskWaypoint(Vector3 position, int taskId, bool placeTrap)
        {
            Position = position;
            TaskId = taskId;
            PlaceTrap = placeTrap;
        }

        public Vector3 Position { get; private set; }
        public int TaskId { get; private set; }
        public bool PlaceTrap { get; private set; }
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
        //spriteRenderer.color = Character.Color;
        
        animator.runtimeAnimatorController = Character.PlayerAnimatorController;

        _taskBar = GameManager.Singleton.PlayerProgressBars.GetAvailableTaskPointsBar();
        _taskBar.Initialize(Character.Color);
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

    public List<TaskData> GetTasksFromActiveQuests()
    {
        List<TaskData> tasks = new List<TaskData>();
        foreach (QuestData quest in _activeQuests)
            foreach (TaskData task in quest.Tasks)
                tasks.Add(task);

        return tasks;
    }

    public bool TryFindQuestOfTask(TaskData taskToFind, out QuestData activeQuest)
    {
        foreach (QuestData quest in _activeQuests)
            foreach (TaskData task in quest.Tasks)
                if (task == taskToFind)
                {
                    activeQuest = quest;
                    return true;
                }

        activeQuest = null;
        return false;
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

            TaskMenu.OnDoTask += AddTaskWaypoint;
            TaskMenu.OnPlaceTrap += AddTrapWaypoint;
        }
    }

    private void DestroyTaskMenu()
    {
        if (_activeTaskMenu)
        {
            _activeTaskMenu.ExecuteMouseInput();

            // Clear task menu and unsubcribe from buttons
            TaskMenu.OnDoTask -= AddTaskWaypoint;
            TaskMenu.OnPlaceTrap -= AddTrapWaypoint;
            Destroy(_activeTaskMenu.gameObject);
            _activeTaskMenu = null;
        }
    }

    [Client]
    private void AddTaskWaypoint(TaskObject taskObject)
    {
        Debug.Log($"Added Task {taskObject.Task.TaskName}".Color("yellow"));

        if (gameData.TryGetTaskId(taskObject.Task, out int taskId))
            CmdTryAddWaypoint(taskObject.TaskPosition, taskId, false);
    }

    [Client]
    private void AddTrapWaypoint(TaskObject taskObject)
    {
        Debug.Log($"Placed Trap {taskObject.Task.TaskName}".Color("yellow"));

        if (gameData.TryGetTaskId(taskObject.Task, out int taskId))
            CmdTryAddWaypoint(taskObject.TaskPosition, taskId, true);
    }

    [Command]
    private void CmdTryAddWaypoint(Vector3 targetPos, int taskId, bool placeTrap)
    {
        // Dont Add Waypoint if is already waypoint
        foreach (var item in _taskWaypoints)
            if (item.TaskId == taskId) return;

        _taskWaypoints.Enqueue(new TaskWaypoint(targetPos, taskId, placeTrap));

        // Calculate path no new waypoint
        NavMeshPath path = new NavMeshPath();
        TaskWaypoint[] taskWaypoints = _taskWaypoints.ToArray();
        Vector3 startPosition = taskWaypoints.Length > 1 ? taskWaypoints[taskWaypoints.Length - 2].Position : _agent.transform.position;
        NavMesh.CalculatePath(startPosition, targetPos, NavMesh.AllAreas, path);

        RpcSpawnWaypointPath(connectionToClient, path.corners, taskId, placeTrap);
    }

    [TargetRpc]
    private void RpcSpawnWaypointPath(NetworkConnection target, Vector3[] pathCorners, int taskId, bool placeTrap)
    {
        LineRenderer line = Instantiate(waypointPathPrefab);
        line.positionCount = pathCorners.Length;
        line.SetPositions(pathCorners);
        line.material = placeTrap ? trapPathMat : taskPathMat;

        _waypointPaths.Add(taskId, line.gameObject);
    }

    [TargetRpc]
    private void RpcRemoveWaypoint(NetworkConnection target, int taskId)
    {
        _waypointPaths.TryGetValue(taskId, out GameObject waypointPath);
        Destroy(waypointPath);
        _waypointPaths.Remove(taskId);
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        // Getting quests
        if (_questCooldown >= 0)
        {
            _questCooldown -= Time.fixedDeltaTime;
            if (_questCooldown <= 0)
            {
                if (gameData.TryGetNewQuest(_activeQuests.ToArray(), out QuestData newQuest))
                    AddQuest(newQuest);
                _questCooldown = Random.Range(gameData.NewQuestTimerRange.x, gameData.NewQuestTimerRange.y);
            }
        }

        // Movement
        if (_taskWaypoints.Count > 0 && _agent.remainingDistance < 1f)
        {
            if (Vector3.Distance(_agent.transform.position, _taskWaypoints.Peek().Position) < 3f)
            {
                // Task waypoint reached, remove waypoint from list and client
                TaskWaypoint waypoint = _taskWaypoints.Dequeue();

                RpcRemoveWaypoint(connectionToClient, waypoint.TaskId);

                StartCoroutine(TryDoTask(waypoint.TaskId));
            }
            else if (!_isDoingTask)
            {
                NavMesh.SamplePosition(_taskWaypoints.Peek().Position, out NavMeshHit hit, 10, NavMesh.AllAreas);
                _agent.SetDestination(hit.position);
            }
        }
    }

    [Server]
    private void CompleteTask(int taskId)
    {
        //TODO Copleting tasks and adding their TaskPoint value
        TaskData task = gameData.GetTaskById(taskId);
        if (TryFindQuestOfTask(task, out QuestData quest) && _activeQuestPanels.TryGetValue(quest, out QuestPanel questPanel))
            questPanel.TryCompleteTask(task);

        sync_taskPoints += 0.1f;

        NetworkManagerHousework.Singleton.UpdatedPlayerTaskPoints(connectionToClient.connectionId);
        RpcUpdateTaskPoints(sync_taskPoints);
    }

    [Server]
    private void CompleteTrap(int taskId)
    {
        gameData.GetTaskById(taskId).taskObject.ActivateTrap();
    }

    [ClientRpc]
    private void RpcUpdateTaskPoints(float taskPoints)
    {
        _taskBar?.SetTaskPoints(taskPoints);
    }

    private void AddQuest(QuestData quest)
    {
        _activeQuests.Add(quest);
        QuestPanel questPanel = GameManager.Singleton.QuestMenu.AddQuest(quest);
        _activeQuestPanels.Add(quest, questPanel);

        Debug.Log("ADD QUEST");
    }

    private IEnumerator TryDoTask(int taskId)
    {


        _isDoingTask = true;
        yield return new WaitForSeconds(gameData.GetTaskById(taskId).WorkingTime);

        CompleteTask(taskId);
        _isDoingTask = false;
    }

    private IEnumerator TryPlaceTrap(int taskId)
    {
        _isDoingTask = true;
        yield return new WaitForSeconds(gameData.GetTaskById(taskId).WorkingTime);

        CompleteTrap(taskId);
        _isDoingTask = false;
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
