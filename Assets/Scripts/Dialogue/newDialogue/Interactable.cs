using System.Threading.Tasks;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public DialogueData dialogueData;
    void Start()
    {
        
          NewDialogueManager.Instance.StartDialogue(dialogueData);
          _= startdialog();
    }
    public async Task startdialog()
    {
        await Task.Delay(10000);
        NewDialogueManager.Instance.StartDialogue(dialogueData);
    }
    // Update is called once per frame
    void Update()
    {
        
            // Start a dialogue when E is pressed
        
    }
}
