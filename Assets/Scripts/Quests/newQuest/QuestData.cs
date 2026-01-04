using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum NewQuestType
{
    KillEnemies,
    CollectItems,
    TalkToNPC,
    KillBoss,
    FinalQuest,
    Unfreeze
}
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questName = "New Quest";
    public string questDescription = "Quest Description";
    public string questShortDescription = "Short Description";
    public NewQuestType questType;
    public bool isCompleted = false;
    public int targetAmount = 0;
    public int goldReward = 0;
    public int expReward = 0;
    public List<DialogueData> questDialogue;
    public int currentDialogueIndex = 0;

}
