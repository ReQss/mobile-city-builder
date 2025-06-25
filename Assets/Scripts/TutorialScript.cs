using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class TutorialObject
{
    public string name;
    public bool isCompleted;
    public string description;
    
}
public class TutorialScript : MonoBehaviour
{
    public static TutorialScript Instance { get; private set; } 

    public bool wasWellClicked = false;
    public bool wasFactoryClicked = false;
    public bool wasRewardsClicked = false;
    public bool wasWeaponBought = false;
    public bool wasWeaponTaken = false;
    public bool wasNavigationClicked;
    public TextMeshProUGUI objectiveText;
    public GameObject objectiveDescriptionPanel;
    public List<string> objectivesDescription;
    public List<TutorialObject> tutorialObjects;
    public int currentObjectiveIndex = 0;
    public DialogueTrigger dialogueTrigger;
    public GameObject player;
    
    public Button buttonToUnlock;
    // public GameObject currentWeapon;

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
    
    public void SetNavigationClicked(int index)
    {
        if (index != currentObjectiveIndex)
        {
            return;
        }
        wasNavigationClicked = true;
        objectiveDescriptionPanel.SetActive(false);
    }
    public void GrabObjectQuest()
    {
        if (PlayerMovement.playerMovementInstance != null && PlayerMovement.playerMovementInstance.currentWeapon != null)
        {
            Debug.Log("Weapon taken");
            wasWeaponTaken = true;
            NextObjective(1);
            TriggerDialogue(1);
            currentObjectiveIndex++; // Assuming the first objective is to grab the weapon
        }
    }
    public void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player != null)
        {
            Animator animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("getup");
            }
            else
            {
                Debug.LogError("Animator component not found on player GameObject.");
            }
        }
        GameManager.Instance.coinsCollected = 0;
        // SetDescription();
    }
    public void SetCoinsCollected(int index)
    {
        if (index != currentObjectiveIndex) return;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.coinsCollected = 100;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(wasWeaponTaken== false )
            GrabObjectQuest();
    }
    public void IncreaseObjectiveIndex(int index)
    {
        if(currentObjectiveIndex== index)
        currentObjectiveIndex++;
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
    public void SetDescription2(String desc)
    {
           if (objectiveText == null)
        {
            Debug.LogError("Objective Text is not assigned in the TutorialScript.");
            return;
        }

        objectiveText.text = desc;
        
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
            // currentObjectiveIndex++;
            SetDescription();
        }
        else
        {
            Debug.Log("All objectives completed.");
        }
    }
    public void TriggerDialogue(int index)
    {
        if (index != currentObjectiveIndex)
        {
            return;
        }
        Debug.Log("xd");
        this.GetComponent<DialogueTrigger>().TriggerDialogueNoQuests();
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
