using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
    Gold,
    Item,
    Exp,
    Statistics
}
public class TreasureChest : MonoBehaviour
{
    public RewardType rewardType;
    [Header("Exp and gold bonus")]
    public int coinReward = 0;
    public int expReward = 0;
    [Header("Stats bonus")]
    public int healthBonus = 0;
    public int attackBonus = 0;
    [Header("Item rewards")]
    public InventoryItem itemReward = null;
    public bool isRewardCollected = false;
    [SerializeField]
    public List<Sprite> itemSprites;

    private bool playerInRange = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rewardType = Random.Range(0, 4) switch
        {
            0 => RewardType.Gold,
            1 => RewardType.Exp,
            2 => RewardType.Statistics,
            _ => RewardType.Item
        };
        switch (rewardType)
        {
            case RewardType.Gold:
                coinReward = GetRandomGold();
                break;
            case RewardType.Exp:
                expReward = GetRandomExp();
                break;
            case RewardType.Statistics:
                RandomStatistics();
                break;
            case RewardType.Item:
                itemReward = GetRandomItem();
                break;
        }
    }

    public int GetRandomExp()
    {
        int maxExp = GameManager.Instance.playerExperienceToGetLevel;
        if (maxExp <= 100)
        {
            maxExp = 2000;
        }
        int randomExp = Random.Range(maxExp / 2, maxExp * 2);
        return randomExp;
    }
    public int GetRandomGold()
    {
        int gold = Random.Range(500, 2000);
        return gold;
    }
    public InventoryItem GetRandomItem()
    {
        EquipmentType randomType = (EquipmentType)Random.Range(0, System.Enum.GetValues(typeof(EquipmentType)).Length);
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
            itemOriginal.itemIcon,//itemicon
            Random.Range(itemOriginal.health, itemOriginal.health + 30),
            Random.Range(itemOriginal.attack, itemOriginal.attack + 10),
            Random.Range(itemOriginal.attackSpeed, itemOriginal.attackSpeed + 1),
            Random.Range(itemOriginal.movementSpeed, itemOriginal.movementSpeed + 2),
            true
            ,itemOriginal.cost,
            itemOriginal.itemNameToDisable
        );
        return newItem;
    }
    public void RandomStatistics()
    {
        int randomStat = Random.Range(1, 3);
        if (randomStat == 1)
        {
            int currentPlayerHealth = GameManager.Instance.playerHealth;

            int healthBonus = Random.Range(currentPlayerHealth / 10, currentPlayerHealth / 4);
            this.healthBonus = healthBonus;
        }
        else if (randomStat == 2)
        {
            int currentPlayerAttack = GameManager.Instance.playerAttack;

            int attackBonus = Random.Range(currentPlayerAttack / 5, currentPlayerAttack / 3);
            this.attackBonus = attackBonus;
        }
    }
    public void CollectReward()
    {
        switch (rewardType)
        {
            case RewardType.Gold:
                GameManager.Instance.coinsCollected += coinReward;
                GameUIHandler.Instance.ChangeRewardItemImage(itemSprites[0]);
                GameUIHandler.Instance.ChangeRewardItemText(coinReward.ToString() + " monet");
                break;
            case RewardType.Exp:
                GameManager.Instance.AddExp(expReward);
                GameUIHandler.Instance.ChangeRewardItemImage(itemSprites[1]);
                GameUIHandler.Instance.ChangeRewardItemText(expReward.ToString() + " exp");
                break;
            case RewardType.Statistics:
                GameManager.Instance.playerHealth += healthBonus;
                if (healthBonus != 0)
                {
                    GameUIHandler.Instance.ChangeRewardItemImage(itemSprites[2]);
                    GameUIHandler.Instance.ChangeRewardItemText(healthBonus.ToString() + " health bonus");
                }
                GameManager.Instance.playerAttack += attackBonus;
                if (attackBonus != 0)
                {
                    GameUIHandler.Instance.ChangeRewardItemImage(itemSprites[3]);
                    GameUIHandler.Instance.ChangeRewardItemText(attackBonus.ToString() + " attack bonus");
                }
                break;
            case RewardType.Item:
            
                GameUIHandler.Instance.ChangeRewardItemText("You got an item!");
                GameUIHandler.Instance.ChangeRewardItemImage(itemReward.itemIcon);
                GameUIHandler.Instance.ChangeRewardItemText(itemReward.itemName);
                GameManager.Instance.CopyNewItemStats(itemReward);
                // GameManager.Instance.AddItem(itemReward);
                break;
        }
        isRewardCollected = true;
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void Update()
    {
        if (!isRewardCollected && playerInRange)
        {
            if (GameUIHandler.Instance.interactionAction != null && GameUIHandler.Instance.interactionAction.action.WasPressedThisFrame())
            {
                CollectReward();
                GameUIHandler.Instance.EnableOrDisableUI(GameUIHandler.Instance.obtainRewardPanel);
                Debug.Log("Reward collected");
            }
        }
    }
}
