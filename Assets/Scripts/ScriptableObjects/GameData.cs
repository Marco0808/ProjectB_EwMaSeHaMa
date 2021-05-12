using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameData", menuName = "Housework/Game Data")]
public class GameData : ScriptableObject
{
    [Header("Balancing Values")]
    [Tooltip("The time range after which a new quest come up for the player.")]
    [SerializeField] private Vector2 newQuestTimerRange = new Vector2(30f, 60f);
    [SerializeField] private float trapTaskDelayTime = 5;

    [Header("Data accessible by ID")]
    [SerializeField] private CharacterData[] characters;
    [SerializeField] private TaskData[] tasks;
    [SerializeField] private QuestData[] quests;

    public Vector2 NewQuestTimerRange => newQuestTimerRange;
    public float TrapTaskDelayTime => trapTaskDelayTime;

    public CharacterData[] Characters => characters;
    public TaskData[] Tasks => tasks;
    public QuestData[] Quests => quests;


    public bool TryGetNewQuest(QuestData[] activeQuests, out QuestData newQuest)
    {
        //TODO Get new quest randomly, not in oder
        foreach (QuestData quest in quests)
            if (activeQuests.Length > 0)
            {
                foreach (QuestData activeQuest in activeQuests)
                    if (quest != activeQuest)
                    {
                        newQuest = quest;
                        return true;
                    }
            }
            else
            {
                newQuest = quest;
                return true;
            }

        newQuest = null;
        return false;
    }

    public CharacterData GetCharacterById(int id) { return characters[id]; }
    public TaskData GetTaskById(int id) { return tasks[id]; }
    public QuestData GetQuestById(int id) { return quests[id]; }

    public bool TryGetCharacterId(CharacterData character, out int id) { return TryGetIdFrom(characters, character, out id); }
    public bool TryGetTaskId(TaskData task, out int id) { return TryGetIdFrom(tasks, task, out id); }
    public bool TryGetQuestId(QuestData quest, out int id) { return TryGetIdFrom(quests, quest, out id); }

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
