
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public InventoryItem inventoryItem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PutOnItem()
    {
        InventoryItem item = inventoryItem;

        // Zakładamy, że InventoryItem ma pole equipmentType
        foreach (var equipmentSlot in EquipingSystem.Instance.equipmentSlots)
        {
            if (equipmentSlot.equipmentType == item.equipmentType && equipmentSlot.slotImage != null)
            {
                Debug.Log($"Putting on item: {item.itemName} to slot: {equipmentSlot.equipmentType}");
                equipmentSlot.slotImage.sprite = item.itemIcon;
                equipmentSlot.slotImage.gameObject.SetActive(true);
                RemoveItemFromSlot();
                break;
            }
        }
    }
    public void RemoveItemFromSlot()
    {
        // inventoryItem = null;
        inventoryItem.isEquipped = true;
        this.gameObject.GetComponent<Image>().sprite = null;
        this.gameObject.SetActive(false);
    }
}
