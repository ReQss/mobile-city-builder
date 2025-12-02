
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [System.Serializable]
    public class Ability
    {
        public Image abilityIcon;
        public string abilityName;
        public string abilityDescription;
        public Sprite abilitySprite;
        public Image abilityBackground;
        public int abilityCost;
        public bool isUnlocked=false;
        public bool isAvailable=false;
    }
    [System.Serializable]
    public class AbilitiesSection
    {
        public Ability[] abilities;
    }
    [SerializeField]
    public AbilitiesSection[] sections;

  
    public int currentPoints;
    public TextMeshProUGUI currentPointsText;
    int findedSectionIndex = 0;
    public Animator alertFailAnimator;
    public string selectedAbilityName = "";
    public Ability selectedAbility = null;
    public TextMeshProUGUI selectedAbilityNameText;
    public TextMeshProUGUI selectedAbilityDescriptionText;
    public TextMeshProUGUI selectedAbilityCostText;
    public Image selectedAbilitySplashart;
    public void ShowFailAlert()
    {
        if (alertFailAnimator == null) return;
        alertFailAnimator.SetTrigger("triggerAnimator");
    }
    public Ability FindAbilityByName(string name)
    {
        int i = 0;
        foreach (AbilitiesSection section in sections)
        {
            findedSectionIndex = i;
            foreach (Ability ability in section.abilities)
            {
                if (ability.abilityName == name)
                {

                    return ability;
                }
            }
            i++;
        }
        findedSectionIndex = -1;
        return null;
    }
    public void DeselectAbility()
    {
        selectedAbilityName = "";
    }
    public void SelectAbility(string abilityName)
    {
        selectedAbilityName = abilityName;
        selectedAbility = FindAbilityByName(abilityName);
        SetSelectedAbilityUI();
    }
    public void SetSelectedAbilityUI()
    {
        if (selectedAbility == null) return;

        selectedAbilityNameText.text = selectedAbility.abilityName;
        selectedAbilityDescriptionText.text = selectedAbility.abilityDescription;
        selectedAbilityCostText.text = selectedAbility.abilityCost.ToString();
        selectedAbilitySplashart.sprite = selectedAbility.abilitySprite == null ? null : selectedAbility.abilitySprite;

    }
    public void UnlockAbility()
    {
        string abilityName = selectedAbilityName;
        Ability ability = FindAbilityByName(abilityName);
        if (ability == null) return;
        if (currentPoints < ability.abilityCost)
        {
            ShowFailAlert();
            return;
        }
        if (ability.isAvailable == false) return;
        if(ability.isUnlocked) return;
        currentPoints -= ability.abilityCost;
        GameManager.Instance.playerLevelPoints = currentPoints;
        ability.isUnlocked = true;
        ability.abilityBackground.color = Color.red;

        currentPointsText.text = currentPoints.ToString();
        if(GameManager.Instance != null){
            GameManager.Instance.UnlockContent(abilityName);
        }
        UpdateNextSection();
        DeselectAbility();
    }
    public void UpdateNextSection(){
        if(findedSectionIndex + 1 >= sections.Length) return;
            foreach (Ability ab in sections[findedSectionIndex + 1].abilities)
            {
                    if (ab.isUnlocked) continue;
                    ab.isAvailable = true;
                    ab.abilityBackground.color = Color.green;
            }
    }
    public void UpdateUnlockedAbilities()
    {
        if (GameManager.Instance == null) return;
        UnlockedContent unlocked = GameManager.Instance.unlockedContent;

        bool isNextSectionAvailable = false;
        foreach (AbilitiesSection section in sections)
        {
            bool wasSectionUnlocked = isNextSectionAvailable;
            isNextSectionAvailable = false;
            foreach (Ability ability in section.abilities)
            {
                // Tworzymy nazwę pola, np. "magicShopUnlocked"
                string unlockedFieldName = ability.abilityName + "Unlocked";
                var field = typeof(UnlockedContent).GetField(unlockedFieldName);
                if (field != null && field.FieldType == typeof(bool))
                {
                    bool isUnlocked = (bool)field.GetValue(unlocked);
                    ability.isUnlocked = isUnlocked;
                    if (ability.isUnlocked)
                    {
                        ability.abilityBackground.color = Color.red;
                        isNextSectionAvailable = true;
                    }
                    else if (wasSectionUnlocked)
                    {
                        ability.abilityBackground.color = Color.green;
                        ability.isAvailable = true;
                    }
                    else
                    {
                        ability.abilityBackground.color = Color.white;
                    }
                }
            }
        }
    }
    void Start()
    {
        currentPoints = GameManager.Instance != null ? GameManager.Instance.playerLevelPoints : 0;
        currentPointsText.text = currentPoints.ToString();
        UpdateUnlockedAbilities();
        if (sections[0].abilities[0].isUnlocked == false)
        {
            sections[0].abilities[0].abilityBackground.color = Color.green;
        }
            
    }

   
}
