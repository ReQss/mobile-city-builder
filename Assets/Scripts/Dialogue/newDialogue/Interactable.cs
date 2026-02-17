using System.Threading.Tasks;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public DialogueData dialogueData;
    public Statistics reward;
    private bool statisticsReceived = false;
  
    public void StartDialog()
    {
        if(dialogueData == null) return;
        NewDialogueManager.Instance.StartDialogue(dialogueData);
    }
    // Update is called once per frame
    public void ReceiveStatistics()
    {
        if (statisticsReceived) return;
        statisticsReceived = true;
        NewQuestManager.Instance.playerStatistics.Add(reward);
    }
    void Update()
    {
        
            // Start a dialogue when E is pressed
        
    }
}
