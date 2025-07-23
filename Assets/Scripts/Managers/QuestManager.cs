using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable]
public enum QuestType
{
    KillEnemies,
    CollectItems,
    TalkToNPC,
    KillBoss,
    FinalQuest,
    Unfreeze
}

[System.Serializable]
public class Quest
{
    public string questName;
    public string questDescription;
    public bool isCompleted;
    public bool questAccepted = false;
     public QuestType questType;
    public int targetAmount; 
    public int currentAmount;
    public GameObject itemToCollect;
    public int goldReward;
    public GameObject npc;
    public GameObject refToThisNpc;
    public int amountOfHeals = 0;
    public bool disableNpcAfterAcceptedQuest = false;
    
    public List <GameObject> enemiesToUnfreeze = new List<GameObject>();
    public List<GameObject> enemiesToSpawn = new List<GameObject>(); 
    public Quest(string name, string description)
    {
        questName = name;
        questDescription = description;
        isCompleted = false;
    }

    public void CompleteQuest()
    {
        isCompleted = true;
        if(npc!=null)
            npc.GetComponent<DialogueTrigger>().questIndex++;
        GameManager.Instance.coinsCollected += goldReward;
        if (npc != null)
        {
            npc.GetComponent<DialogueTrigger>().ChangeCanvas();

            // Spawn heals nearby NPC in random position
            if (QuestManager.Instance != null)
            {
                if (amountOfHeals > 0)
                    QuestManager.Instance.SpawnHealsNearNPC(npc, amountOfHeals);
            }
            // npc = null;
            //  npc = refToThisNpc;
        }
    }
}
public class QuestManager : MonoBehaviour
{

    public int actIndex = 0;
    public static QuestManager Instance { get; private set; }
    public List<Quest> quests;
    public int currentQuestIndex = 0;
    public Quest currentQuest;
    public Quest givenQuest;
    [SerializeField]
    private ProceduralWeaponPlacement enemyGenerator;

    public TextMeshProUGUI currentQuestDescription;
    public GameUIHandler gameUIHandler;
    public GameObject healthPrefab;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {
        if (currentQuest != null)
        {
            QuestManager.Instance.CheckQuestProgress(QuestManager.Instance.currentQuest);
        }
    }
    IEnumerator SpawnEnemies()
    {
        while (currentQuest != null && !currentQuest.isCompleted)
        {
            enemyGenerator.SpawnObjectsNumberNearbyPlayer(1);
            Debug.Log("Enemy spawned");
            yield return new WaitForSeconds(8f);
        }
    }
    public void AcceptQuest()
    {
        if (givenQuest != null)
        {
            StartQuest(givenQuest);
        }
    }
    public void StartQuest(Quest quest)
    {
        currentQuest = quest;
        currentQuest.questAccepted = true;
        quests.Add(quest);
        UpdateQuestUI();
        givenQuest = null;
        if (quest.questType == QuestType.KillEnemies && quest.enemiesToSpawn.Count > 0)
        {
            enemyGenerator.SpawnEnemiesNumberNearbyPlayerList(quest.targetAmount, quest.enemiesToSpawn);
            GameUIHandler.Instance.PlayBossMusic();
        }
        else if (quest.questType == QuestType.KillEnemies)
        {
            enemyGenerator.SpawnEnemiesNumberNearbyPlayer(quest.targetAmount);
            GameUIHandler.Instance.PlayBattleMusic();
        }
        else if (quest.questType == QuestType.CollectItems)
        {

        }
        else if (quest.questType == QuestType.TalkToNPC)
        {
            StartCoroutine(SpawnEnemies());
            GameUIHandler.Instance.PlayBattleMusic();
        }
        else if (quest.questType == QuestType.KillBoss)
        {
        }
        else if (quest.questType == QuestType.FinalQuest)
        {
            gameUIHandler.FinishActUI();
            GameManager.Instance.UpdateQuestFinishedIndex(actIndex);
        }
        else if (quest.questType == QuestType.Unfreeze)
        {
            SoundManager.Instance.PlayBossMusic();
            // GameUIHandler.Instance.PlayBattleMusic();
            foreach (GameObject enemy in quest.enemiesToUnfreeze)
            {
                if (enemy != null)
                {
                    enemy.GetComponent<EnemyAI>().enabled = true;
                    enemy.GetComponent<Animator>().enabled = true;
                }
            }
        }
        if (quest.disableNpcAfterAcceptedQuest && quest.refToThisNpc != null)
        {
            quest.refToThisNpc.SetActive(false);
        }
    }
    public Quest GetCurrentQuest()
    {
        return currentQuest;
    }
    public void UpdateQuestUI()
    {
        if (currentQuestDescription != null)
        {
            if (currentQuest != null)
            {
                if (currentQuest.isCompleted)
                {
                    currentQuestDescription.text = "Powróć do miejsca zadania";
                }
                else
                {
                    currentQuestDescription.text = currentQuest.questDescription;
                }
            }

        }
    }
    public void CheckQuestProgress(Quest quest)
    {
        if (quest == null || quest.isCompleted)
            return;

        switch (quest.questType)
        {
            case QuestType.KillEnemies:
                if (quest.currentAmount >= quest.targetAmount)
                {
                    quest.CompleteQuest();
                    GameUIHandler.Instance.PlayAmbientMusic();
                }
                break;
            case QuestType.CollectItems:
                if (quest.currentAmount >= quest.targetAmount)
                {
                    quest.CompleteQuest();


                }
                break;
            case QuestType.TalkToNPC:
                if (quest.currentAmount >= 1)
                {
                    quest.CompleteQuest();

                    GameUIHandler.Instance.PlayAmbientMusic();
                }
                break;
            case QuestType.Unfreeze:
                if (PlayerMovement.playerMovementInstance != null && PlayerMovement.playerMovementInstance.health <= 0)
                {
                    StartCoroutine(UnfreezeQuestFinishAfterDelay());
                }
                break;

        }

        UpdateQuestUI();
    }
    private IEnumerator UnfreezeQuestFinishAfterDelay()
    {
        yield return new WaitForSeconds(7f);
        if (PlayerMovement.playerMovementInstance != null && PlayerMovement.playerMovementInstance.health <= 0)
        {
            gameUIHandler.FinishActUI();
            Debug.Log("Act finished, final quest started");
            GameManager.Instance.UpdateQuestFinishedIndex(actIndex);
            // Player is dead, handle quest fail or respawn logic here
        }
    }
    public void SpawnHealsNearNPC(GameObject npc, int amountOfHeals)
    {
        if (healthPrefab == null || npc == null) return;

        for (int i = 0; i < amountOfHeals; i++)
        {
            float radius = 10f; // Increase this value for more distance
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 spawnPos = new Vector3(
                npc.transform.position.x + randomCircle.x,
                npc.transform.position.y + 0f, // 1f above NPC's base position
                npc.transform.position.z + randomCircle.y
            );
            Instantiate(healthPrefab, spawnPos, Quaternion.identity);
        }
    }
    
}
