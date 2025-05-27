using UnityEngine;

public class Quest_CollectItemTrigger : MonoBehaviour
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
        if (currentQuest != null && currentQuest.questType == QuestType.CollectItems)
        {
            distanceLeft = Vector3.Distance(transform.position, GameObject.Find("Player").transform.position);
            if (distanceLeft < 2.5f)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Player is close enough to talk to the NPC.");
                    QuestManager.Instance.currentQuest.currentAmount = currentQuest.targetAmount;
                    questManager.CheckQuestProgress(currentQuest);
                    // CollectItem();
                    Destroy(this.gameObject);
                }
        
            }
        }
    }
}