using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public EquipmentType equipmentType;
    public EquipmentQuality equipmentQuality;
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
    
    public InventoryItem() { }
    public InventoryItem(
       EquipmentType equipmentType,
       bool isEquipped,
       string itemName,
       string itemDescription,
       Sprite itemIcon,
       int health,
       int attack,
       int attackSpeed,
       int movementSpeed,
       bool isUnlocked,
       int cost,
       string itemNameToDisable
   )
    {
        this.equipmentType = equipmentType;
        this.isEquipped = isEquipped;
        this.itemName = itemName;
        this.itemDescription = itemDescription;
        this.itemIcon = itemIcon;
        this.health = health;
        this.attack = attack;
        this.attackSpeed = attackSpeed;
        this.movementSpeed = movementSpeed;
        this.isUnlocked = isUnlocked;
        this.cost = cost;
        this.itemNameToDisable = itemNameToDisable;
    }


    public InventoryItem(InventoryItem other)
    {
        itemName = other.itemName;
        equipmentType = other.equipmentType;
        isEquipped = other.isEquipped;
        itemDescription = other.itemDescription;
        itemIcon = other.itemIcon;
        health = other.health;
        attack = other.attack;
        attackSpeed = other.attackSpeed;
        movementSpeed = other.movementSpeed;
        isUnlocked = other.isUnlocked;
        cost = other.cost;
        itemNameToDisable = other.itemNameToDisable;
    }
        public void CopyFrom(InventoryItem other)
    {
        this.equipmentType = other.equipmentType;
        this.isEquipped = other.isEquipped;
        this.itemName = other.itemName;
        this.itemDescription = other.itemDescription;
        if(other.itemIcon)
        this.itemIcon = other.itemIcon;
        this.health = other.health;
        this.attack = other.attack;
        this.attackSpeed = other.attackSpeed;
        this.movementSpeed = other.movementSpeed;
        this.isUnlocked = other.isUnlocked;
        this.cost = other.cost;
        this.itemNameToDisable = other.itemNameToDisable;
        this.equipmentQuality = other.equipmentQuality;
    }
}
public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryItemDirectory;
    public List<GameObject> inventoryPrefabs;
    [SerializeField]
    public List<InventoryItem> playerInventoryParts;


    void Start()
    {
        LoadItems();
        PutOnItems();

    }
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
