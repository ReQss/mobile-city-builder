using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIHandler : MonoBehaviour
{
    public enum NotificationType { None, NPC, Weapon }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject currentWeaponImage;
    [SerializeField]
    public List<GameObject> weaponImages;
    public TextMeshProUGUI currentWeaponUses;
    public TextMeshProUGUI currentCoinsCollected;
    public GameObject notificationPrefab;
    void Start()
    {

    }

    void Update()
    {
        if (currentCoinsCollected != null)
        {
            currentCoinsCollected.text = GameManager.Instance.coinsCollected.ToString();
        }
        
        EnableNotification("Press E to interact with NPCs", NotificationType.NPC);

    }
    public void UpdateWeaponImage(String weaponName)
    {
        foreach (GameObject weaponImage in weaponImages)
        {
            if (weaponImage.name == weaponName)
            {
                weaponImage.SetActive(true);
                break;
            }
            else if (currentWeaponImage != null)
            {
                currentWeaponImage.SetActive(false);
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
        else if ( notificationType == NotificationType.Weapon)
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
  

}