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
    [SerializeField] private TaskPopup taskPopupPrefab;
    [SerializeField] private EncounterPopup encounterPopupPrefab;

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
    private TaskPopup _activeTaskPopup;
    private TaskTimeIndicator _activeTaskTimeIndicator;
    private EncounterPopup _activeEncounterPopup;
    private int? _encounteredPlayer;
    private Encounter _activeEncounter;
    private bool _isDoingTask;
    private float _questCooldown;

    private Coroutine _taskCoroutine;

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

    public enum EncounterDecision { None, Conflict, Cooperate }

    #region Callbacks & Setup
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

    public override void OnStopAuthority() => _input.Disable();

    public override void OnStartClient()
    {
        displayNameText.text = sync_displayName;
        //spriteRenderer.color = Character.Color;

        animator.runtimeAnimatorController = Character.PlayerAnimatorController;

        _taskBar = GameManager.Singleton.PlayerProgressBars.GetAvailablequestPointsBar();
        _taskBar.Initialize(Character.Color);
    }

    public override void OnStopClient() => _taskBar.Hide();

    public override void OnStartServer()
    {
        NetworkManagerHW.Singleton.GamePlayers.Add(connectionToClient.connectionId, this);
    }

    public override void OnStopServer()
    {
        NetworkManagerHW.Singleton.GamePlayers.Remove(connectionToClient.connectionId);
    }

    [ServerCallback]
    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateQuestTimer();
    }

    [ClientCallback]
    private void Update()
    {
        UpdateTaskObjectSelection();
    }

    [Server]
    public void SetupPlayer(string displayname, int characterId)
    {
        sync_displayName = displayname;
        sync_characterId = characterId;
    }
    #endregion

    #region Waypoints & Movement    
    [Server]
    private void UpdateMovement()
    {
        if (_taskWaypoints.Count > 0 && _agent.remainingDistance < 1f)
            if (Vector3.Distance(_agent.transform.position, _taskWaypoints.Peek().Position) < 3f)
            {
                TaskWaypointReached(_taskWaypoints.Peek());
            }
            else if (!_isDoingTask)
            {
                TaskWaypointCompleted();
            }
    }

    private void TaskWaypointCompleted()
    {
        NavMesh.SamplePosition(_taskWaypoints.Peek().Position, out NavMeshHit hit, 10, NavMesh.AllAreas);
        _agent.SetDestination(hit.position);
    }

    [Command]
    private void CmdEnableNavAgent()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = true;
        _agent.updateRotation = false;
    }

    [Client]
    private void AddTaskWaypoint(TaskObject taskObject)
    {
        if (gameData.TryGetTaskId(taskObject.Task, out int taskId))
            CmdTryAddWaypoint(taskObject.TaskPosition, taskId, false);
    }

    [Client]
    private void AddTrapWaypoint(TaskObject taskObject)
    {
        if (gameData.TryGetTaskId(taskObject.Task, out int taskId))
            CmdTryAddWaypoint(taskObject.TaskPosition, taskId, true);
    }

    [Command]
    private void CmdTryAddWaypoint(Vector3 targetPos, int taskId, bool placeTrap)
    {
        // Dont Add Waypoint if task already is a waypoint
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
    private void RpcDestroyWaypointPath(NetworkConnection target, int taskId)
    {
        _waypointPaths.TryGetValue(taskId, out GameObject waypointPath);
        Destroy(waypointPath);
        _waypointPaths.Remove(taskId);
    }
    #endregion

    #region Task Object System
    [Server]
    private void TaskWaypointReached(TaskWaypoint waypoint)
    {
        // remove waypoint from list and client
        RpcDestroyWaypointPath(connectionToClient, waypoint.TaskId);

        //TODO Encounter
        TaskData task = gameData.GetTaskById(waypoint.TaskId);

        if (task.TaskObject.PlayerAlreadyAssigned(connectionToClient.connectionId))
            return;

        if (task.TaskObject.AssignPlayerToTask(connectionToClient.connectionId, out int otherPlayerId))
        {
            // trigger encounter with other player at the task
            TriggerEncounter(otherPlayerId, waypoint);
        }
        else
        {
            // do task if no encounter was triggered, ie if only player at task
            _taskCoroutine = StartCoroutine(TryDoTaskOrTrap(waypoint.TaskId, waypoint.PlaceTrap));
        }
    }

    [Server]
    private void CompleteTask(int taskId)
    {
        TaskData task = gameData.GetTaskById(taskId);

        // Complete current task for all quests
        FindQuestsIncludingTask(task, out QuestData[] questsIncludingTask);
        foreach (QuestData quest in questsIncludingTask)
        {
            gameData.TryGetQuestId(quest, out int questId);
            RpcUpdateQuestPanel(connectionToClient, questId, taskId);
        }
    }

    [Server]
    private void CompleteTrap(int taskId)
    {
        gameData.GetTaskById(taskId).TaskObject.ActivateTrap(connectionToClient);
        AddInsanityPoints(gameData.PlaceTrapInsanityPoints);
    }

    [Server]
    private IEnumerator TryDoTaskOrTrap(int taskId, bool isPlaceingTrap, float additionalWorkingTime = 0f)
    {
        TaskData task = gameData.GetTaskById(taskId);

        // check if task is even available
        if (!isPlaceingTrap)
        {
            //TODO Also break if active tasks are already completed
            FindQuestsIncludingTask(task, out QuestData[] questsIncludingTask);
            if (questsIncludingTask.Length == 0)
            {
                task.TaskObject.RemoveAssignedPlayer(connectionToClient.connectionId);
                _taskWaypoints.Dequeue();
                yield break;
            }
        }

        _isDoingTask = true;

        // Calculate time to work on task, including any trap delays
        float workingTime;
        if (isPlaceingTrap) workingTime = gameData.PlaceTrapWorkingTime + additionalWorkingTime;
        else
        {
            task.TaskObject.CheckForTrap(out float trapDelayTime);
            workingTime = task.WorkingTime + trapDelayTime + additionalWorkingTime;
        }

        // spawn time indicator
        SpawnTaskTimeIndicator(connectionToClient, task.TaskObject.TaskMenuRoot.position, workingTime, isPlaceingTrap);

        Debug.Log($"Task {task.TaskName} will be done in {workingTime} seconds".Color("yellow"));
        yield return new WaitForSeconds(workingTime);

        // complete action
        if (isPlaceingTrap) CompleteTrap(taskId);
        else CompleteTask(taskId);

        DestroyTaskTimeIndicator(connectionToClient);
        task.TaskObject.RemoveAssignedPlayer(connectionToClient.connectionId);

        _taskWaypoints.Dequeue();
        _isDoingTask = false;
    }

    [TargetRpc]
    private void SpawnTaskTimeIndicator(NetworkConnection target, Vector3 position, float duration, bool isPlacingTrap)
    {
        _activeTaskTimeIndicator = Instantiate(taskTimeIndicatorPrefab, position, Camera.main.transform.rotation);
        _activeTaskTimeIndicator.InitializeTimer(duration, isPlacingTrap);
    }

    [TargetRpc]
    private void DestroyTaskTimeIndicator(NetworkConnection target)
    {
        if (_activeTaskTimeIndicator)
        {
            _activeTaskTimeIndicator.DestroyIndicator();
            _activeTaskTimeIndicator = null;
        }
    }

    private bool TryGetTaskObjectFromHit(RaycastHit hit, out TaskObject taskObject)
    {
        taskObject = null;
        if (hit.collider == null) return false;
        return hit.collider.CompareTag("TaskObject") && hit.collider.TryGetComponent(out taskObject);
    }
    #endregion

    #region Encounter System
    [Server]
    private void TriggerEncounter(int otherPlayerId, TaskWaypoint waypoint)
    {
        // execute encounter behavior for self and other player
        if (NetworkManagerHW.Singleton.GamePlayers.TryGetValue(otherPlayerId, out NetworkGamePlayer otherPlayer))
        {
            Encounter encounter = new Encounter(connectionToClient.connectionId, otherPlayerId);

            otherPlayer.ExecuteEncounter(encounter, waypoint.TaskId);

            ExecuteEncounter(encounter, waypoint.TaskId);

            StartCoroutine(TimedEncounter());
        }
    }

    [Server]
    public void ExecuteEncounter(Encounter encounter, int taskId)
    {
        // stop current task
        if (_taskCoroutine != null) StopCoroutine(_taskCoroutine);
        DestroyTaskTimeIndicator(connectionToClient);

        _activeEncounter = encounter;

        RpcShowEncounterPopup(connectionToClient, taskId);
    }

    [TargetRpc]
    private void RpcShowEncounterPopup(NetworkConnection target, int taskId)
    {
        TaskData task = gameData.GetTaskById(taskId);
        _activeEncounterPopup = Instantiate(encounterPopupPrefab, task.TaskObject.TaskMenuRoot.position, Camera.main.transform.rotation);

        EncounterPopup.OnEncounterSolution += ApplyEncounterDecision;
    }

    [Client]
    private void ApplyEncounterDecision(bool cooperate)
    {
        EncounterPopup.OnEncounterSolution -= ApplyEncounterDecision;

        CmdSetEncounterDecision(cooperate);
    }

    [TargetRpc]
    private void RpcDestroyEncounterPopup(NetworkConnection target)
    {
        if (_activeEncounterPopup)
        {
            Destroy(_activeEncounterPopup.gameObject);
            _activeEncounterPopup = null;
        }
    }

    [Command]
    public void CmdSetEncounterDecision(bool cooperate)
    {
        RpcDestroyEncounterPopup(connectionToClient);

        if (_activeEncounter.ThisPlayerId == connectionToClient.connectionId)
            _activeEncounter.ThisPlayerDecision = cooperate ? Encounter.Decision.Cooperate : Encounter.Decision.Conflict;
        if (_activeEncounter.OtherPlayerId == connectionToClient.connectionId)
            _activeEncounter.OtherPlayerDecision = cooperate ? Encounter.Decision.Cooperate : Encounter.Decision.Conflict;
    }

    private IEnumerator TimedEncounter()
    {
        float timer = 0;

        while (timer < gameData.MaxEncounterDuration)
        {
            // end encounter if both players are decided
            if (_activeEncounter.OtherPlayerDecision != Encounter.Decision.None &&
                _activeEncounter.ThisPlayerDecision != Encounter.Decision.None)
            {
                EndEncounterOnInitiator();
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // if max conflict time is over
        // declare as conflict if a player has not set their conflict decision
        if (_activeEncounter.OtherPlayerDecision == Encounter.Decision.None)
            _activeEncounter.OtherPlayerDecision = Encounter.Decision.Conflict;
        if (_activeEncounter.ThisPlayerDecision == Encounter.Decision.None)
            _activeEncounter.ThisPlayerDecision = Encounter.Decision.Conflict;
        EndEncounterOnInitiator();
    }

    [Server]
    private void EndEncounterOnInitiator()
    {
        if (NetworkManagerHW.Singleton.GamePlayers.TryGetValue(_activeEncounter.OtherPlayerId, out NetworkGamePlayer otherPlayer))
        {
            int thisPlayerId = connectionToClient.connectionId;
            int otherPlayerId = otherPlayer.connectionToClient.connectionId;

            otherPlayer.EncounterOutcome(EvaluateEncounterWorkingTimeOutcome(otherPlayerId, thisPlayerId));

            EncounterOutcome(EvaluateEncounterWorkingTimeOutcome(thisPlayerId, otherPlayerId));
        }
    }

    [Server]
    public void EncounterOutcome(float addedWorkingTime)
    {
        _activeEncounter = null;
        RpcDestroyEncounterPopup(connectionToClient);

        TaskWaypoint waypoint = _taskWaypoints.Peek();
        _taskCoroutine = StartCoroutine(TryDoTaskOrTrap(waypoint.TaskId, waypoint.PlaceTrap, addedWorkingTime));
    }

    private float EvaluateEncounterWorkingTimeOutcome(int thisPlayerId, int otherPlayerId)
    {
        Encounter e = _activeEncounter;

        // self cooperate, other conflict
        if (e.IsDecisionCooperate(thisPlayerId) && !e.IsDecisionCooperate(otherPlayerId)) return 0 * gameData.EncounterWorkingTimeMultiplier;

        // self conflict, other cooperate
        if (!e.IsDecisionCooperate(thisPlayerId) && e.IsDecisionCooperate(otherPlayerId)) return 3 * gameData.EncounterWorkingTimeMultiplier;

        // both conflict
        if (!e.IsDecisionCooperate(thisPlayerId) && !e.IsDecisionCooperate(otherPlayerId)) return 1 * gameData.EncounterWorkingTimeMultiplier;

        // both cooperate
        if (e.IsDecisionCooperate(thisPlayerId) && e.IsDecisionCooperate(otherPlayerId)) return 2 * gameData.EncounterWorkingTimeMultiplier;

        return 0;
    }
    #endregion

    #region Quest System
    [Server]
    private void UpdateQuestTimer()
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
    }

    [Server]
    private void AddNewQuest()
    {
        if (_activeQuests.Count > gameData.MaxQuestCount)
        {
            AddInsanityPoints(gameData.TooManyQuestsInsanityPoints);
            return;
        }

        if (gameData.TryGetNewQuest(_activeQuests.ToArray(), out QuestData quest))
        {
            _activeQuests.Add(quest);
            gameData.TryGetQuestId(quest, out int questId);
            RpcAddQuestPanel(connectionToClient, questId);
        }
    }

    [Command]
    private void CmdCompleteQuest(int questId)
    {
        QuestData quest = gameData.GetQuestById(questId);
        AddQuestPoints(quest.QuestPoints);
        _activeQuestPanels.Remove(quest);
    }

    [TargetRpc]
    private void RpcAddQuestPanel(NetworkConnection target, int questId)
    {
        QuestData quest = gameData.GetQuestById(questId);
        QuestPanel questPanel = GameManager.Singleton.QuestMenu.AddQuest(quest);
        _activeQuestPanels.Add(quest, questPanel);
    }

    [TargetRpc]
    private void RpcUpdateQuestPanel(NetworkConnection target, int questId, int taskId)
    {
        QuestData quest = gameData.GetQuestById(questId);
        if (_activeQuestPanels.TryGetValue(quest, out QuestPanel questPanel))
        {
            questPanel.TryCompleteTask(gameData.GetTaskById(taskId), out bool questCompletetd);

            if (questCompletetd)
            {
                questPanel.CompleteQuest();
                _activeQuests.Remove(quest);
                CmdCompleteQuest(questId);
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
    #endregion

    #region Quest & Insanity Points
    [Server]
    private void AddQuestPoints(int value)
    {
        sync_questPoints += value;
        NetworkManagerHW.Singleton.UpdatedPlayerQuestPoints(connectionToClient.connectionId);
        RpcUpdateQuestPoints(sync_questPoints);

        Debug.Log($"Added {value} QuestPoints, now {sync_questPoints}".Color("yellow"));
    }

    [Server]
    private void AddInsanityPoints(int value)
    {
        sync_insanityPoints += value;
        NetworkManagerHW.Singleton.UpdatedPlayerInsanityPoints(connectionToClient.connectionId);
        RpcUpdateInsanityPoints(sync_insanityPoints);

        Debug.Log($"Added {value} InsanityPoints, now {sync_insanityPoints}".Color("yellow"));
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
    #endregion

    #region Task Menu
    private void UpdateTaskObjectSelection()
    {
        // Run mouse input behavior if we have authority
        if (hasAuthority && !_activeTaskPopup)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(_input.Game.MousePosition.ReadValue<Vector2>());
            Debug.DrawRay(mouseRay.origin, mouseRay.direction * maxMouseRayDistance, Color.green);

            // try to select task object if anything was hit
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

    private void SelectTaskObject()
    {
        // Create new task menu if none is open
        if (_activeTaskObject && !_activeTaskPopup && !_activeEncounterPopup)
        {
            _activeTaskObject.SetHighlighted(true);

            // Instantiate TaskMenu at active task object's menu root and rotate it to facie the camera
            _activeTaskPopup = Instantiate(taskPopupPrefab, _activeTaskObject.TaskMenuRoot.position, Camera.main.transform.rotation);
            _activeTaskPopup.Initialize(_activeTaskObject);

            TaskPopup.OnDoTask += AddTaskWaypoint;
            TaskPopup.OnPlaceTrap += AddTrapWaypoint;
        }
    }

    private void DestroyTaskMenu()
    {
        if (_activeTaskPopup)
        {
            _activeTaskPopup.ExecuteMouseInput();

            // Clear task menu and unsubcribe from buttons
            TaskPopup.OnDoTask -= AddTaskWaypoint;
            TaskPopup.OnPlaceTrap -= AddTrapWaypoint;
            Destroy(_activeTaskPopup.gameObject);
            _activeTaskPopup = null;
        }
    }
    #endregion

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

public class Encounter
{
    public enum Decision { None, Conflict, Cooperate }

    public int ThisPlayerId { get; set; }
    public int OtherPlayerId { get; set; }
    public Decision ThisPlayerDecision { get; set; }
    public Decision OtherPlayerDecision { get; set; }

    public Encounter(int thisPlayerId, int otherPlayerId)
    {
        ThisPlayerId = thisPlayerId;
        OtherPlayerId = otherPlayerId;
        ThisPlayerDecision = Decision.None;
        OtherPlayerDecision = Decision.None;
    }

    public bool IsDecisionCooperate(int playerId)
    {
        if (playerId == ThisPlayerId) return ThisPlayerDecision == Decision.Cooperate;
        if (playerId == OtherPlayerId) return OtherPlayerDecision == Decision.Cooperate;
        return false;
    }
}

