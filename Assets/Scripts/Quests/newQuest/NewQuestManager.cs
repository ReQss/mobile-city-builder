using System.Collections.Generic;
using UnityEngine;

public class NewQuestManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    public List<QuestData> allQuests;
    public int currentQuestIndex = 0;
    public QuestData CurrentQuest => allQuests[currentQuestIndex];
    public static NewQuestManager Instance { get; private set; }
     void Awake()
    {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        
    }
    public void AdvanceToNextQuest()
    {
        if (currentQuestIndex < allQuests.Count - 1)
        {
            currentQuestIndex++;
        }
        else
        {
            Debug.Log("All quests completed!");
        }
    }
    
}
