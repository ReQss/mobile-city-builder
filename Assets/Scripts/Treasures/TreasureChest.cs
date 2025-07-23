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
    public GameObject itemReward;
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
                // itemReward = GameManager.Instance.GetRandomItem();
                break;
        }
    }

    public int GetRandomExp()
    {
        int maxExp = GameManager.Instance.playerExperienceToGetLevel;
        int randomExp = Random.Range(maxExp / 2, maxExp * 2);
        return randomExp;
    }
    public int GetRandomGold()
    {
        int gold = Random.Range(500, 2000);
        return gold;
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
                break;
            case RewardType.Exp:
                GameManager.Instance.AddExp(expReward);

                GameUIHandler.Instance.ChangeRewardItemImage(itemSprites[1]);
                break;
            case RewardType.Statistics:
                GameManager.Instance.playerHealth += healthBonus;
                if (healthBonus != 0)
                {
                    GameUIHandler.Instance.ChangeRewardItemImage(itemSprites[2]);
                }
                GameManager.Instance.playerAttack += attackBonus;
                if (attackBonus != 0)
                {
                    GameUIHandler.Instance.ChangeRewardItemImage(itemSprites[3]);
                }
                break;
            case RewardType.Item:
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
