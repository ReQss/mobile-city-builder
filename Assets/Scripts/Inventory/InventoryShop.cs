using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryShop : MonoBehaviour
{
    public List<InventoryItem> unlockedItems;
    public List<GameObject> unlockedItemPanels;
    public Dictionary<string, GameObject> itemPanelsDictionary = new Dictionary<string, GameObject>();
    public UIHandler uiHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unlockedItems = GameManager.Instance.unlockedItems;
        for (int i = 0; i < unlockedItems.Count; i++)
        {
            itemPanelsDictionary[unlockedItems[i].itemName] = unlockedItemPanels[i];
            Debug.Log($"Przypisano panel '{unlockedItemPanels[i].name}' do itemu '{unlockedItems[i].itemName}'");
        }
        DisableBoughtItemsOnLoad();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void DisableBoughtItemsOnLoad()
    {
        foreach (var item in unlockedItems)
        {
            if (item.isUnlocked && itemPanelsDictionary.TryGetValue(item.itemName, out GameObject panel))
            {
                panel.SetActive(false);
                Debug.Log($"Panel '{panel.name}' został wyłączony dla przedmiotu '{item.itemName}'.");
            }
            else
            {
                Debug.LogWarning($"Nie znaleziono panelu dla przedmiotu '{item.itemName}'.");
            }
        }
    }
    public void BuyItem(string itemName)
    {
        InventoryItem foundItem = unlockedItems.Find(item => item.itemName == itemName);
        if (foundItem != null && foundItem.cost < GameManager.Instance.playerCoinCount)
        {
            GameManager.Instance.playerCoinCount -= foundItem.cost;
            foundItem.isUnlocked = true;

            // Wyłącz panel powiązany z kupionym przedmiotem
            if (itemPanelsDictionary.TryGetValue(itemName, out GameObject panel))
            {
                panel.SetActive(false);
                Debug.Log($"Panel '{panel.name}' został wyłączony po zakupie '{itemName}'.");
            }
            else
            {
                Debug.LogWarning($"Nie znaleziono panelu dla przedmiotu '{itemName}'.");
            }
            EquipingSystem.Instance.LoadObtainedItemsIntoInventory();
            uiHandler.SuccessfulOperation();
        }
        else
        {
            uiHandler.FailureOperation();
        }
    }
   
}
