using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum EquipmentType
{


    Boots,
    Tunic,

    Gloves,
    Helmet,

    Pants,
    None
    // Head,
    // Bags1,
    // Bags2,
    // Hair,
    // Legs,

    // Torso,
    // Body,
    // Shorts,
}
[System.Serializable]
public class EquipmentSlot
{
    public EquipmentType equipmentType;
    public Image slotImage;
    
}
public class EquipingSystem : MonoBehaviour
{
    public static EquipingSystem Instance;
    public List<Image> inventorySlots = new List<Image>();
    public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
    
    
    public List<InventoryItem> unlockedItems = new List<InventoryItem>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
      void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        unlockedItems = GameManager.Instance.unlockedItems;
        LoadObtainedItemsIntoInventory();
        PutOnAlreadyEquippedItems();
    }
    public void PutOnAlreadyEquippedItems()
    {
        foreach (var item in unlockedItems)
        {
            if (item.isEquipped)
            {
                foreach (var equipmentSlot in equipmentSlots)
                {
                    if (equipmentSlot.equipmentType == item.equipmentType && equipmentSlot.slotImage != null)
                    {
                        Debug.Log($"Equipping already equipped item: {item.itemName} to slot: {equipmentSlot.equipmentType}");
                        equipmentSlot.slotImage.sprite = item.itemIcon;
                        equipmentSlot.slotImage.gameObject.SetActive(true);
                        break;
                    }
                }
                // Remove the item from the inventory slots if it is equipped
                foreach (var invSlotImage in inventorySlots)
                {
                    var invSlot = invSlotImage.GetComponent<InventorySlot>();
                    if (invSlot != null && invSlot.inventoryItem == item)
                    {
                        invSlot.RemoveItemFromSlot();
                        break;
                    }
                }
            }
        }
    }
    public void LoadObtainedItemsIntoInventory()
    {
        HashSet<InventoryItem> assignedItems = new HashSet<InventoryItem>();

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            bool assigned = false;

            for (int j = 0; j < unlockedItems.Count; j++)
            {
                InventoryItem item = unlockedItems[j];
                if (item.isUnlocked && item.itemIcon != null && !assignedItems.Contains(item))
                {
                    inventorySlots[i].gameObject.GetComponent<InventorySlot>().inventoryItem = item;
                    inventorySlots[i].gameObject.SetActive(true);
                    inventorySlots[i].sprite = item.itemIcon;
                    assignedItems.Add(item);
                    assigned = true;
                    break;
                }
            }

            if (!assigned)
            {
                inventorySlots[i].sprite = null;
                inventorySlots[i].gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // LoadObtainedItemsIntoInventory();
    }
}
