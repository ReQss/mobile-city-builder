using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIHandler : MonoBehaviour
{
    public static GameUIHandler Instance;
    public enum NotificationType { None, NPC, Weapon, QuestItem }
    GameObject currentWeaponImage;
    [SerializeField]
    public List<GameObject> weaponImages;
    public TextMeshProUGUI currentWeaponUses;
    public TextMeshProUGUI currentCoinsCollected;
    public GameObject notificationPrefab;
    public GameObject questAcceptUI;
    public GameObject finishActUI;
    public GameObject gameOverUI;
    public GameObject darkBackground;
    public GameObject MapUI;
    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference interactionAction;
    public InputActionReference playerAction;
    public InputActionReference mapAction;
    public InputActionReference specialAction;
    [Header("Input Actions City")]
    public InputActionReference cityMoveAction;
    public InputActionReference cityInteractionAction;

    public bool cityMoveClicked = false;

    [SerializeField]
    private List<GameObject> perksUISlots;
    [SerializeField]
    private List<TextMeshProUGUI> statistics;
    [Header("Notifcations elements")]
    string notificationNPC = "Press 'Interaction Button' to interact with NPCs";
    public GameObject interactionButton;
    public bool isInteractingWithNpc= false;
    public bool isInteractingWithWeapon = false;
    public AudioClip ambientMusicClip;
    public AudioClip battleMusicClip;
    public AudioClip bossMusicClip;
    public AudioSource musicSource;
    public bool musicChanging = false;
    public GameObject autonavigationUI;
    public GameObject autoAttackUI;
     public GameObject MoneyFactoryUpgradingPanel;
     public TextMeshProUGUI MoneyFactoryUpgradingPanelTimeLeft;
    public GameObject WellUpgradingPanel;
     public TextMeshProUGUI WellUpgradingPanelTimeLeft;
    public GameObject autoNavigationNofication;
    public GameObject attackNotification;
    
    [Header("Player level")]
    public TextMeshProUGUI playerLevel;
    public TextMeshProUGUI playerExp;
    public TextMeshProUGUI playerPointsToSpend;
    public GameObject levelUpVFX;
    public Image dashButtonImage;
    public List<GameObject> weaponChoosePanels;
    public GameObject obtainRewardPanel;
    public Image obtainRewardItemImage;
    public void EnableWeaponToChoose()
    {
        if (weaponChoosePanels == null || weaponChoosePanels.Count == 0)
            return;

        if (GameManager.Instance.weapons.isSwordEnabled && weaponChoosePanels.Count > 0)
            weaponChoosePanels[0].SetActive(true);

        if (GameManager.Instance.weapons.isBowEnabled && weaponChoosePanels.Count > 1)
            weaponChoosePanels[1].SetActive(true);

        if (GameManager.Instance.weapons.isCrossbowEnabled && weaponChoosePanels.Count > 2)
            weaponChoosePanels[2].SetActive(true);

        if (GameManager.Instance.weapons.isRodEnabled && weaponChoosePanels.Count > 3)
            weaponChoosePanels[3].SetActive(true);
    }
    
    public void PlayBattleMusic()
    {
        if (musicChanging == false) return;
        if (musicSource != null && battleMusicClip != null)
        {
            musicSource.clip = battleMusicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    public void ChangeButtonColorBasedOnTime(float time, float duration)
    {
        if (dashButtonImage == null) return;

        float alpha = Mathf.Clamp01(1f-(time / duration)); 
            Color color = dashButtonImage.color;
            color.a = Mathf.Lerp(0.2f, 1f, alpha);
            dashButtonImage.color = color;
    }
    public void PlayBossMusic()
    {
        if (musicChanging == false) return;
        if (musicSource != null && bossMusicClip != null)
        {
            musicSource.clip = bossMusicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    public void LevelUp()
    {
        if (levelUpVFX == null) return;
        levelUpVFX.SetActive(true);
        Animator animator = levelUpVFX.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("levelupanim", -1, 0f); // "levelupanim" to nazwa stanu w Animatorze
            if(SoundManager.Instance != null)
                SoundManager.Instance.PlayLevelUp();
        }
    }
    public void SetLevelAndExp()
    {
        if (playerLevel == null || playerExp == null || playerPointsToSpend == null) return;
        playerLevel.text = GameManager.Instance.playerLevel.ToString();
        playerExp.text = (GameManager.Instance.playerExperienceToGetLevel - GameManager.Instance.playerCurrentExperience).ToString();
        playerPointsToSpend.text = GameManager.Instance.pointsToSpend.ToString();
    }
    public void SetTimeLeftForUpgrading()
    {
        bool foundWell = false;
        bool foundFactory = false;

        if (GameManager.Instance.currentUpgradedBuildings.Count > 0)
        {
            foreach (CurrentUpgradedBuilding temp in GameManager.Instance.currentUpgradedBuildings)
            {
                if (temp.buildingName == "Mityczna studnia")
                {
                    foundWell = true;
                    WellUpgradingPanel.SetActive(true);
                    if (WellUpgradingPanelTimeLeft != null)
                    {
                        WellUpgradingPanelTimeLeft.text = temp.timeLeft.ToString();
                    }
                }
                if (temp.buildingName == "Fabryka monet")
                {
                    foundFactory = true;
                    MoneyFactoryUpgradingPanel.SetActive(true);
                    if (MoneyFactoryUpgradingPanelTimeLeft != null)
                    {
                        MoneyFactoryUpgradingPanelTimeLeft.text = temp.timeLeft.ToString();
                    }
                }
            }
        }

        if (!foundWell && WellUpgradingPanel != null)
            WellUpgradingPanel.SetActive(false);

        if (!foundFactory && MoneyFactoryUpgradingPanel != null)
        {
            MoneyFactoryUpgradingPanel.SetActive(false);
        }
    }
   
   
    public void ActiveBlackSmithNotification()
    {
        if (MoneyFactoryUpgradingPanel != null)
        {
            MoneyFactoryUpgradingPanel.SetActive(true);
        }
    }
    public void ActiveWellNotification()
    {
        if (WellUpgradingPanel != null)
        {
            WellUpgradingPanel.SetActive(true);
        }
    }
    public void PlayAmbientMusic()
    {
        if (musicChanging == false) return;
        if (musicSource != null && ambientMusicClip != null)
        {
            musicSource.clip = ambientMusicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    public void ChangeRewardItemImage(Sprite newSprite)
    {
        if (obtainRewardItemImage != null)
        {
            obtainRewardItemImage.sprite = newSprite;
        }
    }
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(obtainRewardPanel == null)
        obtainRewardPanel = GameObject.Find("ObtainReward");
        if (obtainRewardPanel != null)
        {
            Transform rewardItemImageTransform = obtainRewardPanel.transform.Find("RewardItemImage");
            if (rewardItemImageTransform != null)
            {
                obtainRewardItemImage = rewardItemImageTransform != null ? rewardItemImageTransform.GetComponent<Image>() : null;
            }
            
        }

        if (TutorialScript.Instance == null)
        {
            if (GameManager.Instance.playerHealth > 600)
            {
                GameManager.Instance.playerHealth = 100;
            }
        }
        // Enable the input action if not already enabled
        if (GameUIHandler.Instance.moveAction != null && !GameUIHandler.Instance.moveAction.action.enabled)
        {
            GameUIHandler.Instance.moveAction.action.Enable();
        }
        if (GameUIHandler.Instance.interactionAction != null && !GameUIHandler.Instance.interactionAction.action.enabled)
        {
            GameUIHandler.Instance.interactionAction.action.Enable();
        }
        if (GameUIHandler.Instance.playerAction != null && !GameUIHandler.Instance.playerAction.action.enabled)
        {
            GameUIHandler.Instance.playerAction.action.Enable();
        }
        if (mapAction != null && !mapAction.action.enabled)
        {
            mapAction.action.Enable();
        }
        if (GameUIHandler.Instance.cityMoveAction != null && !GameUIHandler.Instance.cityMoveAction.action.enabled)
        {
            GameUIHandler.Instance.cityMoveAction.action.Enable();
        }
        if (GameUIHandler.Instance.specialAction != null && !GameUIHandler.Instance.specialAction.action.enabled)
        {
            GameUIHandler.Instance.specialAction.action.Enable();
        }
        if (GameUIHandler.Instance.cityInteractionAction != null && !GameUIHandler.Instance.cityInteractionAction.action.enabled)
        {
            GameUIHandler.Instance.cityInteractionAction.action.Enable();
        }
        LoadPerksToUI();
        EnableWeaponToChoose();
    }
    

    void Update()
    {   
        SetLevelAndExp();
        SetTimeLeftForUpgrading();
         
        if (currentCoinsCollected != null)
        {
            currentCoinsCollected.text = GameManager.Instance.coinsCollected.ToString();
        }
        if (mapAction != null && mapAction.action.triggered)
        {
            if (MapUI != null)
            {
                MapUI.SetActive(!MapUI.activeSelf);
            }
        }
        // EnableNotification(notificationNPC, NotificationType.NPC);
        if (isInteractingWithNpc || isInteractingWithWeapon)
        {
            TriggerButtonAnimation(interactionButton,true);
        }
        else
        {
            TriggerButtonAnimation(interactionButton, false);
        }
       
        // Check every frame if cityMoveAction is pressed (mouse/touch click)
        cityMoveClicked = cityMoveAction != null && cityMoveAction.action.WasPressedThisFrame();
    }

    public void SwitchQuestAcceptUI()
    {
        if (questAcceptUI != null)
        {
            questAcceptUI.SetActive(!questAcceptUI.activeSelf);
        }
    }
    public void UpdatePerksUI()
    {

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.playerPerks != null)
            {
                foreach (PlayerPerks perk in GameManager.Instance.playerPerks)
                {
                    if (perk.perkIsActive)
                    {
                        foreach (GameObject perkUISlot in perksUISlots)
                        {
                            GameObject elements = perkUISlot.transform.Find("Elements").gameObject;
                            if (!elements.activeSelf)
                            {
                                elements.SetActive(true);
                            }
                        }

                        Debug.Log($"Active Perk: {perk.perkName} - Level: {perk.perkLevel}");
                    }
                }
            }
        }
    }
    public void DisableOrEnableElement(GameObject gameObject)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }   
    }
    public void UpdateWeaponImage(String weaponName)
    {
        foreach (GameObject weaponImage in weaponImages)
        {
            if (weaponImage.name.Contains(weaponName))
            {
                Debug.Log("Updating weapon image: " + weaponName);
                weaponImage.SetActive(true);

            }
            // else if (currentWeaponImage != null)
            // {
            //     currentWeaponImage.SetActive(false);
            // }
            else
            {
                weaponImage.SetActive(false);
            }
        }

        if (currentWeaponImage != null)
        {
            Image uiImage = GetComponent<Image>();
            if (uiImage != null)
            {
                uiImage.sprite = currentWeaponImage.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    public void UpdateUsesCount(int usesCount)
    {
        if (currentWeaponUses != null)
        {
            currentWeaponUses.text = usesCount.ToString();
        }
    }
    public void EnableNotification(String notificationText, NotificationType notificationType)
    {
        if (notificationType == NotificationType.None)
        {
            if (notificationPrefab != null)
            {
                notificationPrefab.SetActive(false);
            }
            isInteractingWithNpc = false;
            isInteractingWithWeapon = false;
            return;
        }

        if (NPC.anyNPCDetectsPlayer && notificationType == NotificationType.NPC)
        {
            EnableNotificationPrefab(notificationText);
            isInteractingWithNpc = true;
        }
        else if (notificationType == NotificationType.Weapon)
        {
            EnableNotificationPrefab(notificationText);
            isInteractingWithWeapon = true;
        }
        else
        {
            if (notificationPrefab != null)
            {
                notificationPrefab.SetActive(false);
            }
            isInteractingWithNpc = false;
            isInteractingWithWeapon = false;
        }

        // Ensure only one interaction type is true at a time
        if (notificationType != NotificationType.NPC)
            isInteractingWithNpc = false;
        if (notificationType != NotificationType.Weapon)
            isInteractingWithWeapon = false;
    }
    public void TriggerButtonAnimation(GameObject button, bool turnOn)
    {
        if (button != null)
        {
            Animator animator = button.GetComponent<Animator>();
            if (animator != null)
            {
                if (turnOn == true)
                    animator.SetBool("isEnabled", true);
                else
                    animator.SetBool("isEnabled", false);
            }

        }
    }
    public void EnableNotificationPrefab(String notificationText)
    {
        if (notificationPrefab != null)
        {
            notificationPrefab.SetActive(true);
            TextMeshProUGUI notificationTextComponent = notificationPrefab.GetComponentInChildren<TextMeshProUGUI>();
            if (notificationTextComponent != null)
            {
                notificationTextComponent.text = notificationText;
            }
        }
    }
    public void EnableOrDisableUI(GameObject gameObject)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
    public void FinishActUI()
    {
        if (finishActUI != null)
        {

            GameManager.Instance.isPlayerInteracting = true;
            finishActUI.SetActive(true);
            darkBackground.SetActive(true);
        }
    }
    public void LoadSceneByName(string sceneName)
    {

        GameManager.Instance.isPlayerInteracting = false;
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("Ładowanie sceny: " + sceneName);
        }
        else
        {
            Debug.LogError("Scena " + sceneName + " nie istnieje.");
        }
    }
    private void LoadPerksToUI()
    {
        // First, disable all perk slots
        foreach (var slot in perksUISlots)
        {
            var elements = slot.transform.Find("Elements");
            if (elements != null)
                elements.gameObject.SetActive(false);
        }

        List<PlayerPerks> playerPerks = GameManager.Instance.playerPerks;
        int i = 0;
        foreach (PlayerPerks perk in playerPerks)
        {
            if (perk.perkIsActive && i < perksUISlots.Count)
            {
                EnablePerk(perksUISlots[i], perk);
                i++;
                Debug.Log($"Actived Perk: {perk.perkName} ");
            }
        }
    }
    public IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(6f);
        if (gameOverUI != null)
        {
            Debug.Log("Game Over UI is enabled");
            GameManager.Instance.isPlayerInteracting = true;
            gameOverUI.SetActive(true);
            darkBackground.SetActive(true);
        }
        yield return null;

    }
    private void EnablePerk(GameObject perkUISlot, PlayerPerks perk)
    {
        Transform elementsTransform = perkUISlot.transform.Find("Elements");
        if (elementsTransform == null)
        {
            Debug.LogError("Elements not found in " + perkUISlot.name);
            return;
        }
        GameObject element = elementsTransform.gameObject;
        if (!element.activeSelf)
        {
            element.SetActive(true);
        }

        // Name
        Transform perkNameTextTransform = elementsTransform.Find("Name/Text (TMP)");
        if (perkNameTextTransform == null)
        {
            Debug.LogError("Text (TMP) not found in " + perkUISlot.name);
            return;
        }
        TextMeshProUGUI perkName = perkNameTextTransform.GetComponent<TextMeshProUGUI>();
        if (perkName == null)
        {
            Debug.LogError("TextMeshProUGUI missing on Text (TMP) in " + perkUISlot.name);
            return;
        }
        perkName.text = perk.perkName;

        // Description
        Transform perkDescTextTransform = elementsTransform.Find("Description/Text (TMP)");
        if (perkDescTextTransform != null)
        {
            TextMeshProUGUI perkDesc = perkDescTextTransform.GetComponent<TextMeshProUGUI>();
            if (perkDesc != null)
            {
                perkDesc.text = perk.perkDescription;
            }
        }

        // Icon
        Transform perkIconTransform = elementsTransform.Find("PerkIcon/RawImage");
        if (perkIconTransform != null)
        {
            UnityEngine.UI.RawImage iconImage = perkIconTransform.GetComponent<UnityEngine.UI.RawImage>();
            if (iconImage != null && perk.perkIcon != null)
            {
                iconImage.texture = perk.perkIcon.texture;
            }
        }

        // isActiveColor
        Transform isActiveColorTransform = elementsTransform.Find("isActiveColor");
        if (isActiveColorTransform != null)
        {
            UnityEngine.UI.Image colorImage = isActiveColorTransform.GetComponent<UnityEngine.UI.Image>();
            if (colorImage != null)
            {
                colorImage.color = perk.perkIsActive ? Color.green : Color.red;
            }
        }
    }
    public void HandleStatistics()
    {
        if (PlayerMovement.playerMovementInstance != null)
        {
            statistics[0].text = PlayerMovement.playerMovementInstance.health.ToString();
            statistics[1].text = PlayerMovement.playerMovementInstance.playerAttack.ToString();
            statistics[2].text = PlayerMovement.playerMovementInstance.speed.ToString();
        }
    }

    public void IncreasePlayerHealthUI(int amount)
    {
        if(GameManager.Instance.coinsCollected >= GameManager.Instance.priceForStatistics){
            GameManager.Instance.coinsCollected -= GameManager.Instance.priceForStatistics;
            GameManager.Instance.IncreasePlayerHealth(amount);
            PlayerMovement.playerMovementInstance.health += amount;
            HandleStatistics();
        }
       
    }
    public void IncreasePlayerHealthByPoints()
    {
        bool result = GameManager.Instance.UsePointForHealth();
        if (result)
        {
            PlayerMovement.playerMovementInstance.health += 1;
            GameManager.Instance.playerHealth += 1;
            HandleStatistics();
        }

    }
    public void IncreasePlayerAttackByPoints()
    {
        bool result = GameManager.Instance.UsePointForAttack();
        if (result)
        {
            PlayerMovement.playerMovementInstance.playerAttack += 1;
            GameManager.Instance.playerAttack += 1;
            HandleStatistics();
        }
    }
    public void IncreasePlayerSpeedByPoints()
    {
        bool result = GameManager.Instance.UsePointForSpeed();
        if (result)
        {
            PlayerMovement.playerMovementInstance.speed += 1;
            GameManager.Instance.playerSpeed += 1;
            HandleStatistics();
        }
    }

    public void IncreasePlayerAttackUI(int amount)
    {
        if (GameManager.Instance.coinsCollected >= GameManager.Instance.priceForStatistics)
        {
            GameManager.Instance.coinsCollected -= GameManager.Instance.priceForStatistics;
            GameManager.Instance.IncreasePlayerAttack(amount);
            PlayerMovement.playerMovementInstance.playerAttack += amount;
            HandleStatistics();
        }
    }

    public void IncreasePlayerSpeedUI(int amount)
    {
        if(GameManager.Instance.coinsCollected >= GameManager.Instance.priceForStatistics){
            GameManager.Instance.coinsCollected -= GameManager.Instance.priceForStatistics;
            GameManager.Instance.IncreasePlayerSpeed(amount);
            PlayerMovement.playerMovementInstance.speed += amount;
            HandleStatistics();
        }
    }
}