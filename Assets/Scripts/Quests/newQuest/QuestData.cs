using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    public Statistics statisticsRequired;
    public string questName = "New Quest";
    public string questDescription = "Quest Description";
    public string questShortDescription = "Short Description";
    public bool isCompleted = false;
    public int goldReward = 0;
    public int expReward = 0;
    public List<DialogueData> questDialogue;
    public int currentDialogueIndex = 0;
    public List<QuestAction> questActionsStart;
    public List<QuestAction> questActionsComplete;
    // public bool startWithDialeogue

}
