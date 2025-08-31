using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
[System.Serializable]
public class CharacterClass
{
    public string className;
    public string classDescription;
    public Sprite classIcon;
    public int health;
    public int attack;
    public int speed;
    public InventoryItem bonusItem;
}
public class CharacterCreatorHandler : MonoBehaviour
{
    [SerializeField]
    public List<CharacterClass> characterClasses = new List<CharacterClass>();
    public List <TextMeshProUGUI> statsTextsList = new List<TextMeshProUGUI>();
    public TextMeshProUGUI classNameText;
    public TextMeshProUGUI classDescriptionText;
    public Image bonusItem;
    private int currentClassIndex = 0;
    public CharacterClass selectedClass;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateSelectedClass(characterClasses[0]);
        selectedClass = characterClasses[0];
    }
    public void UpdateSelectedClass(CharacterClass characterClass)
    {
        classNameText.text = characterClass.className;
        classDescriptionText.text = characterClass.classDescription;
        statsTextsList[0].text = characterClass.health.ToString();
        statsTextsList[1].text = characterClass.attack.ToString();
        statsTextsList[2].text = characterClass.speed.ToString();
        bonusItem.sprite = characterClass.bonusItem.itemIcon;
    }
    public void SelectNextClass()
    {
        if (currentClassIndex >= characterClasses.Count - 1)
        {
            currentClassIndex = 0;
        }
        else
        {
            currentClassIndex++;
        }
        selectedClass = characterClasses[currentClassIndex];
        UpdateSelectedClass(selectedClass);
    }
    // Update is called once per frame
    public void StartNewGame()
    {
        GameManager.Instance.StartNewGame(selectedClass);
    }   
}
