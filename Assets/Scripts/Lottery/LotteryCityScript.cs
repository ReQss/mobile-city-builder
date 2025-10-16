using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public enum LotteryType
{
    Bronze,
    Silver,
    Gold
}
public class LotteryCityScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public LotteryType lotteryType;
    public Animator levelAnimator;
    [Header("Item slot ")]
    public Image itemImageSlot;
    public List<TextMeshProUGUI> itemStatsText = new List<TextMeshProUGUI>();
    public TextMeshProUGUI itemText;
    public TextMeshProUGUI keyCountText;
    public Image keyImageSlot;
    [Header("Sprites")]
    public List<Sprite> bronzeSprites = new List<Sprite>();
    public List<Sprite> keySprites = new List<Sprite>();
    private KeyItem bronzeKey;
    private KeyItem silverKey;
    private KeyItem goldKey;
    public EquipingSystem equipingSystem;
    public TextMeshProUGUI bronzeKeyCountText;
    public TextMeshProUGUI silverKeyCountText;
    public TextMeshProUGUI goldKeyCountText;
    void Start()
    {
        InitKeyCount();
    }
    public void InitKeyCount()
    {
        bronzeKey = GameManager.Instance.keys.Find(k => k.keyType == KeyType.Bronze);
        silverKey = GameManager.Instance.keys.Find(k => k.keyType == KeyType.Silver);
        goldKey = GameManager.Instance.keys.Find(k => k.keyType == KeyType.Gold);
        UpdateKeyCount();
        UpdateKeyCountPanel();   
    }
    public void UpdateKeyCountPanel()
    {
        if (bronzeKeyCountText != null)
            bronzeKeyCountText.text = bronzeKey != null ? bronzeKey.quantity.ToString() : "0";
        if (silverKeyCountText != null)
            silverKeyCountText.text = silverKey != null ? silverKey.quantity.ToString() : "0";
        if (goldKeyCountText != null)
            goldKeyCountText.text = goldKey != null ? goldKey.quantity.ToString() : "0";
    }
    public void UseLever()
    {
        switch (lotteryType)
        {
            case LotteryType.Bronze:
                BronzeLottery();
                break;
            case LotteryType.Silver:
                SilverLottery();
                break;
            case LotteryType.Gold:
                GoldLottery();
                break;
        }
    }
   
    public void SetLotteryType(int lotteryType)
    {
        this.lotteryType = (LotteryType)lotteryType;
        UpdateKeyImage();
        UpdateKeyCount();
    }

    public void UpdateKeyImage()
    {
        switch (lotteryType)
        {
            case LotteryType.Bronze:
                keyImageSlot.sprite = keySprites[0];
                break;
            case LotteryType.Silver:
                keyImageSlot.sprite = keySprites[1];
                break;
            case LotteryType.Gold:
                keyImageSlot.sprite = keySprites[2];
                break;
        }
    }
    public void UpdateKeyCount()
    {
        switch (lotteryType)
        {
            case LotteryType.Bronze:
                keyCountText.text = bronzeKey != null ? bronzeKey.quantity.ToString() : "0";
                break;
            case LotteryType.Silver:
                keyCountText.text = silverKey != null ? silverKey.quantity.ToString() : "0";
                break;
            case LotteryType.Gold:
                keyCountText.text = goldKey != null ? goldKey.quantity.ToString() : "0";
                break;
        }
    }
    // exp lub gold
    public void BronzeLottery()
    {
        if (bronzeKey == null || bronzeKey.quantity <= 0)
            return;
        levelAnimator.SetTrigger("UseLever");
        bronzeKey.quantity--;
        UpdateKeyCount();
        int rollExpOrGold = Random.Range(0, 2); // 0 - exp, 1 - gold
        switch (rollExpOrGold)
        {
            case 0:
                ReceiveRandomExp();
                //  dodac wyswietlanie statystyk po zdobyciu poziomu w miescie
                break;
            case 1:
                ReceiveRandomGold();
                break;
        }
        UpdateKeyCountPanel();
    }
    // wiecej expa lub golda mala szansa na item
    public void SilverLottery()
    {
        if (silverKey == null || silverKey.quantity <= 0)
            return;
        levelAnimator.SetTrigger("UseLever");
        silverKey.quantity--;
        UpdateKeyCount();

        // 0-0.4 gold, 0.4-0.8 exp, 0.8-1 item
        float roll = Random.value;
        if (roll < 0.4f)
        {
            ReceiveRandomGold();
        }
        else if (roll < 0.85f)
        {
            ReceiveRandomExp();
            // dodac wyswietlanie statystyk po zdobyciu poziomu w miescie
        }
        else
        {
            ReceiveRandomItem();
        }
        
        UpdateKeyCountPanel();
    }
    public InventoryItem GetRandomItem(EquipmentType randomType)
    {
        // EquipmentType randomType = (EquipmentType)Random.Range(0, System.Enum.GetValues(typeof(EquipmentType)).Length);
        InventoryItem itemOriginal = GameManager.Instance.unlockedItems.Find(item => item.equipmentType == randomType);
        if (itemOriginal == null)
        {
            Debug.LogWarning("Brak odblokowanego przedmiotu typu: " + randomType);
            return null;
        }
        InventoryItem newItem = new InventoryItem(
            randomType,
            false,
            randomType.ToString(),
            "Great item found at treasure chest from dungeon",
            itemOriginal.itemIcon,
            Random.Range(itemOriginal.health, itemOriginal.health + 15),
            Random.Range(itemOriginal.attack, itemOriginal.attack + 4),
            Random.Range(itemOriginal.attackSpeed, itemOriginal.attackSpeed + 1),
            itemOriginal.movementSpeed,
            true
            , itemOriginal.cost,
            itemOriginal.itemNameToDisable
        );
        return newItem;
    }
    // item
    public void GoldLottery()
    {
        if (goldKey == null || goldKey.quantity <= 0)
            return;
        levelAnimator.SetTrigger("UseLever");
        goldKey.quantity--;
        UpdateKeyCount();
        ReceiveRandomItem();

        UpdateKeyCountPanel();

    }
    public void ReceiveRandomExp()
    {
        int expAmount = Random.Range(GameManager.Instance.playerExperienceToGetLevel / 2, GameManager.Instance.playerExperienceToGetLevel);
        GameManager.Instance.AddExp(expAmount);
        itemText.text = expAmount.ToString();
        itemImageSlot.sprite = bronzeSprites[0];
    }
    public void ReceiveRandomGold()
    {
        int goldAmount = Random.Range(100, 301);
        itemText.text = goldAmount.ToString();
        itemImageSlot.sprite = bronzeSprites[1];
        GameManager.Instance.coinsCollected += goldAmount;
    }
    public void ReceiveRandomItem()
    {
        EquipmentType randomType = (EquipmentType)Random.Range(0, (int)EquipmentType.None);
        InventoryItem itemOriginal = GameManager.Instance.unlockedItems.Find(item => item.equipmentType == randomType);
        
        itemImageSlot.sprite = itemOriginal.itemIcon;
        itemStatsText[0].text = itemOriginal.health.ToString();
        itemStatsText[1].text = itemOriginal.attack.ToString();
        itemStatsText[2].text = itemOriginal.attackSpeed.ToString();
        itemText.text = itemOriginal.itemName;
        InventoryItem itemReward = GetRandomItem(randomType);
        GameManager.Instance.CopyNewItemStats(itemReward);
        equipingSystem.LoadObtainedItemsIntoInventory();
        equipingSystem.PutOnAlreadyEquippedItems();
    }
}
