
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
                Image slotImage = equipmentSlot.slotImage.transform.GetChild(0).GetComponent<Image>();
                if (slotImage != null)
                        {
                            switch (item.equipmentQuality)
                            {
                                case EquipmentQuality.Common:
                                    slotImage.color = Color.white;
                                    break;
                                case EquipmentQuality.Uncommon:
                                    slotImage.color = new Color(173f / 255f, 255f / 255f, 47f / 255f);
                                    break;
                                case EquipmentQuality.Rare:
                                    slotImage.color = new Color(255f / 255f, 235f / 255f, 4f / 255f);
                                    break;
                                default:
                                    slotImage.color = Color.white;
                                    break;
                            }
                            ;
                        }
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
        this.gameObject.transform.parent.GetComponent<Image>().color = Color.white;
        this.gameObject.SetActive(false);
    }
}
