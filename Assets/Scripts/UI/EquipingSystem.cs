using System.Collections.Generic;
using UnityEngine;

public class EquipingSystem : MonoBehaviour
{
    public static EquipingSystem Instance;
    public List<Sprite> equipmentSlots = new List<Sprite>();
    
    public List<InventoryItem> unlockedItems = new List<InventoryItem>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
      void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }
    public void LoadObtainedItemsIntoInventory()
    {
        List<InventoryItem> inventoryItems = unlockedItems;
        if (inventoryItems.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < equipmentSlots.Count && i < inventoryItems.Count; i++)
        {
            InventoryItem item = inventoryItems[i];
            equipmentSlots[i] = item.itemIcon;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
