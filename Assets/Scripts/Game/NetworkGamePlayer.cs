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

    [Header("Prefabs")]
    [SerializeField] private TaskMenu taskPopupPrefab;

    [Header("Waypoint Path")]
    [SerializeField] private LineRenderer waypointPathPrefab;
    [SerializeField] private TaskTimeIndicator taskTimeIndicatorPrefab;

    [SyncVar] private string sync_displayName = "foo";
    [SyncVar] private int sync_characterId;
    [SyncVar] private int sync_questPoints = 0;
    [SyncVar] private int sync_insanityPoints = 0;

    private InputActions _input;
    private NavMeshAgent _agent;
    private QuestPointsBar _taskBar;

    private TaskObject _activeTaskObject;
    private TaskMenu _activeTaskMenu;
    private bool _isDoingTask;
    private float _questCooldown;

    private List<QuestData> _activeQuests = new List<QuestData>();
    private Dictionary<QuestData, QuestPanel> _activeQuestPanels = new Dictionary<QuestData, QuestPanel>();
    private Queue<TaskWaypoint> _taskWaypoints = new Queue<TaskWaypoint>();
    private Dictionary<int, GameObject> _waypointPaths = new Dictionary<int, GameObject>();

    public string DisplayName => sync_displayName;
    public float QuestPoints => sync_questPoints;
    public float InsanityPoints => sync_insanityPoints;
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
        spriteRenderer.color = Character.Color;

        _taskBar = GameManager.Singleton.PlayerProgressBars.GetAvailablequestPointsBar();
        _taskBar.Initialize(Character.Color);
    }

    public override void OnStopClient()
    {
        _taskBar.Hide();
    }

    public override void OnStartServer()
    {
        NetworkManagerHW.Singleton.GamePlayers.Add(connectionToClient.connectionId, this);
    }

    public override void OnStopServer()
    {
        NetworkManagerHW.Singleton.GamePlayers.Remove(connectionToClient.connectionId);
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

    public List<TaskData> GetAllActiveTasks()
    {
        List<TaskData> tasks = new List<TaskData>();
        foreach (QuestData quest in _activeQuests)
            foreach (TaskData task in quest.Tasks)
                tasks.Add(task);

        return tasks;
    }

    public void FindQuestsIncludingTask(TaskData targetTask, out QuestData[] questsIncludingTask)
    {
        List<QuestData> quests = new List<QuestData>();

        foreach (QuestData quest in _activeQuests)
            foreach (TaskData task in quest.Tasks)
                if (task == targetTask)
                    quests.Add(quest);

        questsIncludingTask = quests.ToArray();
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
            _activeTaskMenu = Instantiate(taskPopupPrefab, _activeTaskObject.TaskMenuRoot.position, Camera.main.transform.rotation);
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

        // Set path color dependent on action
        Color pathColor = placeTrap ? gameData.InsanityColor : gameData.TaskColor;
        pathColor.a = line.material.color.a;
        line.material.color = pathColor;

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
                AddNewQuest();
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

                StartCoroutine(TryDoTaskOrTrap(waypoint.TaskId, waypoint.PlaceTrap));
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
        TaskData task = gameData.GetTaskById(taskId);

        // Complete current task for all quests
        FindQuestsIncludingTask(task, out QuestData[] questsIncludingTask);
        foreach (QuestData quest in questsIncludingTask)
            if (_activeQuestPanels.TryGetValue(quest, out QuestPanel questPanel))
            {
                questPanel.TryCompleteTask(task, out bool questCompletetd);

                if (questCompletetd)
                {
                    AddQuestPoints(quest.QuestPoints);

                    questPanel.CompleteQuest();
                    _activeQuests.Remove(quest);
                    _activeQuestPanels.Remove(quest);
                }
            }
    }

    [Server]
    private void AddQuestPoints(int value)
    {
        //TODO Give player correct amount of quest points
        sync_questPoints += value;
        NetworkManagerHW.Singleton.UpdatedPlayerQuestPoints(connectionToClient.connectionId);
        RpcUpdateQuestPoints(sync_questPoints);
    }

    [Server]
    private void AddInsanityPoints(int value)
    {
        //TODO Give player correct amount of insanity points
        sync_insanityPoints += value;
        NetworkManagerHW.Singleton.UpdatedPlayerInsanityPoints(connectionToClient.connectionId);
        RpcUpdateInsanityPoints(sync_insanityPoints);
    }

    [Server]
    private void CompleteTrap(int taskId)
    {
        gameData.GetTaskById(taskId).TaskObject.ActivateTrap(connectionToClient);
        AddInsanityPoints(gameData.PlaceTrapInsanityPoints);
    }

    [ClientRpc]
    private void RpcUpdateQuestPoints(int questPoints)
    {
        _taskBar.SetQuestPoints(questPoints);
    }

    [ClientRpc]
    private void RpcUpdateInsanityPoints(int insanityPoints)
    {
        GameManager.Singleton.PlayerProgressBars.SetInsanityPoints(insanityPoints);
    }

    private void AddNewQuest()
    {
        //TODO Insanity for too many quests
        if (_activeQuests.Count > gameData.MaxQuestCount)
        {
            AddInsanityPoints(gameData.TooManyQuestsInsanityPoints);
            return;
        }

        if (gameData.TryGetNewQuest(_activeQuests.ToArray(), out QuestData quest))
        {
            _activeQuests.Add(quest);
            QuestPanel questPanel = GameManager.Singleton.QuestMenu.AddQuest(quest);
            _activeQuestPanels.Add(quest, questPanel);
        }
    }

    [Server]
    private IEnumerator TryDoTaskOrTrap(int taskId, bool placeTrap)
    {
        TaskData task = gameData.GetTaskById(taskId);

        // if task is not needed, to not work on task
        if (!placeTrap)
        {
            //TODO Also break if active tasks are already completed
            FindQuestsIncludingTask(task, out QuestData[] questsIncludingTask);
            if (questsIncludingTask.Length == 0)
                yield break;
        }

        _isDoingTask = true;

        // Calculate time to work on task, including any trap delays
        float workingTime;
        if (placeTrap) workingTime = gameData.PlaceTrapWorkingTime;
        else
        {
            task.TaskObject.CheckForTrap(out float trapDelayTime);
            workingTime = task.WorkingTime + trapDelayTime;
        }

        TaskTimeIndicator timeIndicator = Instantiate(taskTimeIndicatorPrefab, task.TaskObject.TaskMenuRoot.position, Camera.main.transform.rotation);
        timeIndicator.InitializeTimer(workingTime, placeTrap);

        yield return new WaitForSeconds(workingTime);

        if (placeTrap) CompleteTrap(taskId);
        else CompleteTask(taskId);

        timeIndicator.DestroyIndicator();
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
            NetworkManagerHW.Singleton.StopHost();

        // stop client if client-only
        if (NetworkClient.isConnected)
            NetworkManagerHW.Singleton.StopClient();
    }
}
