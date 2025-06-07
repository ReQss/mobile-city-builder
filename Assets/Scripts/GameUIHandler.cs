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
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {


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
    }

    void Update()
    {
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
            if (Time.timeScale != 0)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        EnableNotification("Press E to interact with NPCs", NotificationType.NPC);

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

        if (NPC.anyNPCDetectsPlayer && notificationType == NotificationType.NPC)
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
        else if (notificationType == NotificationType.Weapon)
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

        else
        {
            if (notificationPrefab != null)
            {
                notificationPrefab.SetActive(false);
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
}