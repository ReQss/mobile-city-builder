using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable]
public class TutorialObject
{
    public string name;
    public bool isCompleted;
    public string description;
    
}
public class TutorialScript : MonoBehaviour
{
    public static TutorialScript Instance { get; private set; } // Singleton instance

    public bool wasWellClicked = false;
    public bool wasFactoryClicked = false;
    public bool wasRewardsClicked = false;
    public bool wasWeaponBought = false;
    public TextMeshProUGUI objectiveText;
    public List<string> objectivesDescription;
    public List<TutorialObject> tutorialObjects;
    public int currentObjectiveIndex = 0;
    public DialogueTrigger dialogueTrigger;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        // Optional: Uncomment if you want this to persist between scenes
        // DontDestroyOnLoad(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetDescription();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetDescription()
    {
        if (objectiveText == null)
        {
            Debug.LogError("Objective Text is not assigned in the TutorialScript.");
            return;
        }
        if (currentObjectiveIndex < objectivesDescription.Count)
        {
            objectiveText.text = objectivesDescription[currentObjectiveIndex];
            // TriggerNextDialogue();
        }
        else
        {
            objectiveText.text = "Tutorial Completed!";
        }
    }
    public void TriggerNextDialogue()
    {
        dialogueTrigger.TriggerDialogueCity();   
    }
    public void NextObjective(int index)
    {
        if (index != currentObjectiveIndex)
        {
            return;
        }
        if (currentObjectiveIndex < objectivesDescription.Count - 1)
        {
            currentObjectiveIndex++;
            SetDescription();
        }
        else
        {
            Debug.Log("All objectives completed.");
        }
    }
    public void SetWellClicked(bool value)
    {
        wasWellClicked = value;
    }

    public void SetFactoryClicked(bool value)
    {
        wasFactoryClicked = value;
    }

    public void SetRewardsClicked(bool value)
    {
        wasRewardsClicked = value;
    }

    public void SetWeaponBought(bool value)
    {
        wasWeaponBought = value;
    }
}
