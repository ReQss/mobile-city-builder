using UnityEngine;

public class TalkToQuestTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    QuestManager questManager;
    public Quest currentQuest;
    public float distanceLeft = 0f;
    void Start()
    {
        questManager = QuestManager.Instance;
        if (questManager == null)
        {
            Debug.LogError("QuestManager instance not found. Make sure it is initialized in the scene.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
          currentQuest = questManager.GetCurrentQuest();
        if (currentQuest != null && currentQuest.questType == QuestType.TalkToNPC)
        {
            distanceLeft = Vector3.Distance(transform.position, GameObject.Find("Player").transform.position);
            if (distanceLeft < 5f)
            {
                Debug.Log("Player is close enough to talk to the NPC.");
                QuestManager.Instance.currentQuest.currentAmount = currentQuest.targetAmount; // Mark quest as finished
                questManager.CheckQuestProgress(currentQuest);     // Optionally trigger quest completion logic
            }
        }
    }
}
