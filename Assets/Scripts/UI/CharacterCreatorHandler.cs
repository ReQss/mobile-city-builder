using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
[System.Serializable]
public enum PowerType
{
    None,
    Undead,
    Shield
}
[System.Serializable]
public class CharacterPower
{
    public PowerType powerType;
    public Sprite powerSprite;
    public bool isUnlocked;
}
[System.Serializable]
public class CharacterClass
{
    public LocalizedString className;
    public LocalizedString classDescription;
    public Sprite classIcon;
    public GameObject classWeapon;
    public int health;
    public int attack;
    public int speed;
    [SerializeField]
    public InventoryItem bonusItem;
    
    public List<CharacterPower> characterPower = new List<CharacterPower>();
}
public class CharacterCreatorHandler : MonoBehaviour
{
    [SerializeField]
    public List<CharacterClass> characterClasses = new List<CharacterClass>();
    public List <TextMeshProUGUI> statsTextsList = new List<TextMeshProUGUI>();
    public TextMeshProUGUI classNameText;
    public TextMeshProUGUI classDescriptionText;
    public Image bonusItem;
    public Image powerIcon;
    private int currentClassIndex = 0;
    public CharacterClass selectedClass;
    public Sprite spriteNone;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateSelectedClass(characterClasses[0]);
        selectedClass = characterClasses[0];
    }
    public void UpdateSelectedClass(CharacterClass characterClass)
    {
        // classNameText.text = characterClass.className;
        // classDescriptionText.text = characterClass.classDescription;
        characterClass.className.StringChanged += (value) =>
        {
            classNameText.text = value;
        };
        characterClass.classDescription.StringChanged += (value) =>
        {
            classDescriptionText.text = value;
        };
        statsTextsList[0].text = characterClass.health.ToString();
        statsTextsList[1].text = characterClass.attack.ToString();
        statsTextsList[2].text = characterClass.speed.ToString();
        bonusItem.sprite = characterClass.bonusItem.itemIcon;
        if (characterClass.characterPower.Count == 0)
            powerIcon.sprite = spriteNone;
        else
            powerIcon.sprite = characterClass.characterPower[0].powerSprite;

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
