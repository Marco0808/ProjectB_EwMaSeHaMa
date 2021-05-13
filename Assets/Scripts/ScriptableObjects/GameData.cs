using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New GameData", menuName = "Housework/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Balancing Values")]
    [Tooltip("The time range after which a new quest come up for the player.")]
    [SerializeField] private Vector2 newQuestTimerRange = new Vector2(30f, 60f);
    [SerializeField] private int maxQuestCount = 5;
    [SerializeField] private float trapTaskDelayTime = 3;
    [SerializeField] private float placeTrapWorkingTime = 5;

    [Tooltip("Self cooperate, other conflict = x0\nSelf conflict, other cooperate = x3\nBoth conflict = x1\nBoth cooperate = x2")]
    [SerializeField] private float encounterWorkingTimeMultiplier = 2;
    [SerializeField] private float maxEncounterDuration = 5;
    [Space]
    [SerializeField] private int placeTrapInsanityPoints = 50;
    [SerializeField] private int tooManyQuestsInsanityPoints = 100;
    [SerializeField] private int maxInsanityPoints = 1000;
    [SerializeField] private int maxQuestPoints = 1000;
    [SerializeField, Range(0, 1)] private float teamWinPointPercentage = 0.7f;

    [Header("Properties")]
    [SerializeField] private Color taskColor = Color.cyan;
    [SerializeField] private Color insanityColor = Color.yellow;

    [Header("Data accessible by ID")]
    [SerializeField] private CharacterData[] characters;

    [SerializeField, ReadOnly] private List<TaskData> _tasks;
    [SerializeField, ReadOnly] private List<QuestData> _quests;

    public Vector2 NewQuestTimerRange => newQuestTimerRange;
    public int MaxQuestCount => maxQuestCount;
    public float TrapTaskDelayTime => trapTaskDelayTime;
    public float PlaceTrapWorkingTime => placeTrapWorkingTime;
    public float EncounterWorkingTimeMultiplier => encounterWorkingTimeMultiplier;
    public float MaxEncounterDuration => maxEncounterDuration;

    public int PlaceTrapInsanityPoints => placeTrapInsanityPoints;
    public int TooManyQuestsInsanityPoints => tooManyQuestsInsanityPoints;
    public int MaxInsanityPoints => maxInsanityPoints;
    public int MaxQuestPoints => maxQuestPoints;
    public float TeamWinPointPercentage => teamWinPointPercentage;

    public Color TaskColor => taskColor;
    public Color InsanityColor => insanityColor;

    public CharacterData[] Characters => characters;
    public TaskData[] Tasks => _tasks.ToArray();
    public QuestData[] Quests => _quests.ToArray();


    private void OnEnable()
    {
        _tasks = new List<TaskData>();
        _tasks.AddRange(Resources.LoadAll<TaskData>("TaskData"));

        _quests = new List<QuestData>();
        _quests.AddRange(Resources.LoadAll<QuestData>("QuestData"));
    }

    public bool TryGetNewQuest(QuestData[] activeQuests, out QuestData newQuest)
    {
        List<QuestData> questPool = new List<QuestData>(_quests);

        foreach (QuestData activeQuest in activeQuests)
            questPool.Remove(activeQuest);

        if (questPool.Count > 0)
        {
            newQuest = questPool[UnityEngine.Random.Range(0, questPool.Count - 1)];
            return true;
        }
        else
        {
            newQuest = null;
            return false;
        }
    }

    public CharacterData GetCharacterById(int id) { return characters[id]; }
    public TaskData GetTaskById(int id) { return _tasks[id]; }
    public QuestData GetQuestById(int id) { return _quests[id]; }

    public bool TryGetCharacterId(CharacterData character, out int id) { return TryGetIdFrom(characters, character, out id); }
    public bool TryGetTaskId(TaskData task, out int id) { return TryGetIdFrom(Tasks, task, out id); }
    public bool TryGetQuestId(QuestData quest, out int id) { return TryGetIdFrom(Quests, quest, out id); }

    private bool TryGetIdFrom(Array source, object target, out int id)
    {
        id = 0;
        for (int i = 0; i < source.Length; i++)
            if (source.GetValue(i) == target)
            {
                id = i;
                return true;
            }
        return false;
    }
}
