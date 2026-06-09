using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    DialogueTrigger dialogueTrigger;
    void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private bool hasInteracted = false;
    async void OnTriggerEnter(Collider other)
    {
        if(hasInteracted) return;
        if (other.CompareTag("Player"))
        {
            // Trigger interaction
            
            hasInteracted = true;
            dialogueTrigger.TriggerDialogue();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && hasInteracted)
        {
            // Reset interaction state when player exits the trigger
            hasInteracted = false;
        }
    }

}
