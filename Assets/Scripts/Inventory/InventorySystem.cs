using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public EquipmentType equipmentType;
    public bool isEquipped = false;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public int health = 0;
    public int attack = 0;
    public int attackSpeed = 0;
    public int movementSpeed = 0;
    public bool isUnlocked = false;
    public int cost = 0;
    public string itemNameToDisable = null;
}
//Hair1
//Legs_Seperate
//Pants
//Torso
//Boots
//Tunic
//Body_02
//shorts
//Gloves
//Helmet
//Head
//Bags_1
//Bags_2
public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryItemDirectory;
    public List<GameObject> inventoryPrefabs;
    [SerializeField]
    public List<InventoryItem> playerInventoryParts;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadItems();
        PutOnItems();
        
    }
     // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.realTimeUpdate)
            {
                    PutOnItems();
            }
    }
    
    public void LoadItems()
    {
        inventoryPrefabs = new List<GameObject>();
        if (inventoryItemDirectory != null)
        {
            foreach (Transform child in inventoryItemDirectory.transform)
            {
                inventoryPrefabs.Add(child.gameObject);
            }
        }
    }
    public void PutOnItems()
    {
        List<InventoryItem> unlockedItems = GameManager.Instance.unlockedItems;
        if (unlockedItems == null) return;
        if (inventoryPrefabs.Count > 0)
        {
            foreach (GameObject item in inventoryPrefabs)
            {
                foreach (InventoryItem itemUnlocked in unlockedItems)
                {
                    if (itemUnlocked.isUnlocked)
                    {
                        if (item.name == itemUnlocked.itemName && unlockedItems.Exists(x => x.itemName == itemUnlocked.itemName && x.isEquipped))
                        {
                            WearObject(item, itemUnlocked.itemName, itemUnlocked.itemNameToDisable);
                        }
                    }
                    else
                    {
                        UnwearObject(item, itemUnlocked.itemName, itemUnlocked.itemNameToDisable);
                    }

                }
            }

        }
    }
    
    public void WearObject(GameObject gameObject, string itemName, string itemNameToDisable)
    {
         if (inventoryPrefabs.Count > 0)
        {
            if (gameObject.name == itemName)
            {
                GameObject findedItem = GetItemByName(itemName);
                findedItem.SetActive(true);


                if (itemNameToDisable != null && itemNameToDisable != "")
                {
                    GameObject findedItem2 = GetItemByName(itemNameToDisable);
                    if (findedItem2 != null)
                        findedItem2.SetActive(false);
                }
            }
        }
    }
     public void UnwearObject(GameObject gameObject, string itemName, string itemNameToDisable)
    {
          if (inventoryPrefabs.Count > 0)
        {
            if (gameObject.name == itemName)
            {
                GameObject findedItem = GetItemByName(itemName);
                findedItem.SetActive(false);


                if (itemNameToDisable != null && itemNameToDisable != "")
                {
                    GameObject findedItem2 = GetItemByName(itemNameToDisable);
                    if (findedItem2 != null)
                        findedItem2.SetActive(true);
                }
            }
        }
    }
    
    
    public GameObject GetItemByName(string itemName)
    {
        foreach (var item in inventoryPrefabs)
        {
            if (item.name == itemName)
            {
                return item;
            }
        }
        return null;
    }
   
}
