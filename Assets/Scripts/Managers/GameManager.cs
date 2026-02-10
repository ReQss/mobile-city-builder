
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerPerks
{
    public string perkName;
    public string perkDescription;
    public Sprite perkIcon;
    public int perkLevel;
    public int perkMaxLevel;
    public bool perkIsActive;
}

[System.Serializable]
public class Player
{
    public int health;
    public int attack;
}

[System.Serializable]
public class Weapons
{
    public bool isSwordEnabled;
    public bool isBowEnabled;
    public bool isCrossbowEnabled;
    public bool isRodEnabled;
}
[System.Serializable]
public class Armor
{
    public int id;
    public string name;
    public string description;
    public int defenseBonus;
    public int healthBonus;
    public bool isActive;
}
[System.Serializable]
public class CurrentUpgradedBuilding
{
    public string buildingName;
    public int level;
    public int timeToUpgrade;
    public int cost;
    public int timeLeft;
    public long upgradeEndTimestamp;
}
[System.Serializable]
public class KeyItem
{
    public KeyType keyType;
    public int quantity;
}
[System.Serializable]
public class PlayerPowers
{

    [Header("Powers")]
    public bool undead; // gives 2 respawns in dung
    public bool shield; // shield can be activated every 5 s to neglected damage
    public bool magicHeal; // heal 20 hp every 30 s
}
[System.Serializable]
public class UnlockedContent
{
    public bool magicShopUnlocked = false;
    public bool magicShopActivated = false;

    public bool armorShopUnlocked = false;
    public bool armorShopActivated = false;

    public bool weaponShopUnlocked = false;
    public bool weaponShopActivated = false;

    public bool lotteryActivated = false;
    public bool moneyFactoryActivated = false;


    public bool mapUnlocked = false;
    public bool healthUnlocked = false;
    public bool attackUnlocked = false;
    public bool speedUnlocked = false;
    public bool shieldUnlocked = false;
    public bool resurrectionUnlocked = false;
    public bool counterUnlocked = false;
    public bool dashUnlocked = false;
}


public class GameManager : MonoBehaviour
{
    public void UnlockContent(string name)
    {
        var unlockActions = new Dictionary<string, Action>
        {
            { "magicShop", () => unlockedContent.magicShopUnlocked = true },
            { "armorShop", () => unlockedContent.armorShopUnlocked = true },
            { "weaponShop", () => unlockedContent.weaponShopUnlocked = true },
            { "map", () => unlockedContent.mapUnlocked = true },
            { "health", () =>
            {
                unlockedContent.healthUnlocked = true;
                playerHealth += 100;
            } },
            { "attack", () =>
            {
                unlockedContent.attackUnlocked = true;
                playerAttack += 10;
            } },
            { "speed", () =>
            {
                unlockedContent.speedUnlocked = true; 
                playerSpeed += 1;
            } },
            { "shield", () => unlockedContent.shieldUnlocked = true },
            { "resurrection", () => unlockedContent.resurrectionUnlocked = true },
            { "counter", () => unlockedContent.counterUnlocked = true },
            { "dash", () => unlockedContent.dashUnlocked = true }
        };

        if (unlockActions.TryGetValue(name, out var action))
            action();
    }
    [Header("Settings")]
    public LightLevel lightLevel;
    public ColorAdjustmentsPreset lightPreset;
    [Header("Key inventory")]
    public List<KeyItem> keys = new List<KeyItem>();
    public Weapons weapons;
    public UnlockedContent unlockedContent = new UnlockedContent();
    public bool realTimeUpdate = false;
    public List<InventoryItem> unlockedItems = new List<InventoryItem>();
    [SerializeField]
    public List<CurrentUpgradedBuilding> currentUpgradedBuildings = new List<CurrentUpgradedBuilding>();
    public Building moneyFactory;
    [Header("Buildings levels")]
    public int moneyFactoryLevel = 1;
    public int wellLevel = 1;
    public Player playerStats;
    [SerializeField]
    public List<Armor> armors;
    public int temporaryCoinsToCollect = 0;
    public float coinsTimeInterval = 3f;
    public GameObject currentPickedBuilding;
    public bool isWorkerUpgrading = false;
    public bool isUIOpen = false;
    public bool isPlayerInteracting = false;
    public List<PlayerPerks> playerPerks = new List<PlayerPerks>();
    public int questActFinishedIndex = 0;
    public int energyRequiredForQuest = 10;
    [Header("Game attributes")]
    
    public int coinsCollected = 0;
    public int weaponLevel = 0;
    public int workerCount = 0;
    
    public int energy = 20;
    public int playerCoinCount = 0;
    [Header("Player stats")]
    public int playerHealth = 100;
    public int playerAttack = 10;
    public int playerSpeed = 8;
    [Header("Player experience")]
    public int playerLevel = 1;
    public int playerLevelPoints = 0;
    public int playerExperienceToGetLevel = 1000;
    public int playerCurrentExperience = 0;
    public int pointsToSpend = 0;
    
    public int priceForStatistics = 1000;

    public GameObject startWeapon;
    public CharacterClass selectedClass;
    private Vector2Int selectedDungeonSize;
    public int selectedNumberOfTreasureChests = 1;

    public PlayerPowers playerPowers = new PlayerPowers();
    [Header("Starting Weapons")]
    public GameObject introductionWeapon;
    public bool isIntroductionWeaponAchieved;
   
    public static GameManager Instance { get; private set; }
    public void SetDungeonSize(Vector2Int size, int numberOfTreasureChests)
    {
        selectedDungeonSize = size;
        selectedNumberOfTreasureChests = numberOfTreasureChests;
    }
    public Vector2Int GetDungeonSize()
    {
        return selectedDungeonSize;
    }
// copy new item stats to old item stats
    public void CopyNewItemStats(InventoryItem newItem)
    {
        if (newItem == null) return;
        InventoryItem item = unlockedItems.Find(item => item.equipmentType == newItem.equipmentType);
        if (item == null) return;
        Debug.Log("Copying stats from new item to existing item: " + newItem.itemName + " to " + item.itemName);
        item.CopyFrom(newItem);
    }
    public void InitLightSettigns()
{
    GameObject globalVolumeObj = GameObject.Find("Global Volume");
    if (globalVolumeObj == null)
    {
        Debug.LogWarning("Global Volume object not found!");
        return;
    }

    Volume globalVolume = globalVolumeObj.GetComponent<Volume>();
    if (globalVolume == null)
    {
        Debug.LogWarning("Volume component not found on Global Volume!");
        return;
    }

    ColorAdjustments colorAdjustments;
    if (globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
    {
        colorAdjustments.postExposure.value = lightPreset.postExposure;
        colorAdjustments.contrast.value = lightPreset.contrast;
    }
    else
    {
        Debug.LogWarning("ColorAdjustments not found in Global Volume profile!");
    }
}
    public void StartNewGame(CharacterClass selectedClass)
    {
        playerPowers.undead = false;
        playerPowers.shield = false;
        unlockedContent.armorShopActivated = false;
        unlockedContent.magicShopActivated = false;
        unlockedContent.weaponShopActivated = false;
        unlockedContent.mapUnlocked = false;
        unlockedContent.healthUnlocked = false;
        unlockedContent.attackUnlocked = false;
        unlockedContent.speedUnlocked = false;
        unlockedContent.shieldUnlocked = false;
        unlockedContent.resurrectionUnlocked = false;
        unlockedContent.counterUnlocked = false;
        unlockedContent.dashUnlocked = false;
        foreach (CharacterPower cp in selectedClass.characterPower)
        {
            switch (cp.powerType)
            {
                case PowerType.Undead:
                    playerPowers.undead = true;
                    break;
                case PowerType.Shield:
                    playerPowers.shield = true;
                    break;
                case PowerType.MagicHeal:
                    playerPowers.magicHeal = true;
                    break;
            }
        }

        RemoveAllItems();
        playerHealth = selectedClass.health;
        playerAttack = selectedClass.attack;
        playerSpeed = selectedClass.speed;
        coinsCollected = 0;
        playerLevel = 1;
        playerLevelPoints = 0;
        playerCurrentExperience = 0;
        pointsToSpend = 0;
        playerExperienceToGetLevel = 1000;
        this.selectedClass = selectedClass;
        if (selectedClass.bonusItem.equipmentType != EquipmentType.None)
            CopyNewItemStats(selectedClass.bonusItem);
    }
    public void RemoveAllItems()
    {
        foreach (InventoryItem item in unlockedItems)
        {
            item.isUnlocked = false;
            item.isEquipped = false;
        }
    }
    public void AddExp(int exp)
    {
        playerCurrentExperience += exp;
        if (playerCurrentExperience >= playerExperienceToGetLevel)
        {
            playerLevel++;
            pointsToSpend += 1;
            playerCurrentExperience = 0;
            playerExperienceToGetLevel += 500;
            LevelUp();
        }

    }
    public void LevelUp()
    {
        GameUIHandler.Instance.LevelUp();
        playerLevelPoints += 1;
        if(DungeonRewardsInfo.Instance != null)
            DungeonRewardsInfo.Instance.levelCollected += 1;

        GameUIHandler.Instance.ShowLevelUpChoosePanel();
        
    }
    public bool UsePointForAttack()
    {
        if (pointsToSpend > 0)
        {
            playerAttack += 2;
            pointsToSpend--;
            return true;
        }
        return false;
    }
    public bool UsePointForHealth()
    {
        if (pointsToSpend > 0)
        {
            playerHealth += 10;
            pointsToSpend--;
            return true;

        }
        return false;
    }
    public bool UsePointForSpeed()
    {
        if (pointsToSpend > 0)
        {
            playerSpeed += 1;
            pointsToSpend -= 1;
            return true;
        }
        return false;
    }
    public IEnumerator UpgradingBuilding(Building building)
    {
        if (currentUpgradedBuildings.Find(x => x.buildingName == building.nameOfBuilding) != null)
        {
            Debug.Log("Already upgrading this building");
            yield break;
        }
        // switch (building.nameOfBuilding)
        // {
        //     case "Mystical Well":
        //         GameUIHandler.Instance.ActiveWellNotification();
        //         break;
        //     case "Blacksmith Forge":
        //         GameUIHandler.Instance.ActiveBlackSmithNotification();
        //         break;

        // }
        long endTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (int)building.TimeToUpgrade;
        currentUpgradedBuildings.Add(new CurrentUpgradedBuilding
        {
            buildingName = building.nameOfBuilding,
            level = building.level,
            timeToUpgrade = (int)building.TimeToUpgrade,
            cost = building.cost,
            timeLeft = (int)building.TimeToUpgrade,
            upgradeEndTimestamp = endTimestamp
        });

        isWorkerUpgrading = true;
        playerCoinCount -= building.cost;
        yield break;
    }
    public void UpdateUpgradeTimers()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        foreach (var currentBuilding in currentUpgradedBuildings)
        {
            if(currentBuilding.timeLeft>0)
            currentBuilding.timeLeft = (int)(currentBuilding.upgradeEndTimestamp - now);
        }
    }
    public void FindMoneyFactory()
    {
        Building[] buildings = FindObjectsOfType<Building>();
        moneyFactory = buildings.FirstOrDefault(b => b.nameOfBuilding == "MoneyFactory");
        if (moneyFactory != null)
        {
            moneyFactoryLevel = moneyFactory.level;
            Debug.Log("Money Factory found: " + moneyFactory.nameOfBuilding);
        }
    }
    public void FindUpgradedBuildingsAndUpdate()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName != "City")
            return;

        for (int i = 0; i < currentUpgradedBuildings.Count; i++)
        {
            var currentBuilding = currentUpgradedBuildings[i];
            if (currentBuilding.timeLeft <= 0)
            {
                Building[] buildings = FindObjectsOfType<Building>();
                Building buildingToUpgrade = buildings.FirstOrDefault(b => b.nameOfBuilding == currentBuilding.buildingName);

                if (buildingToUpgrade != null)
                {
                    buildingToUpgrade.level = currentBuilding.level + 1;
                    buildingToUpgrade.cost = currentBuilding.cost * 2;
                    Debug.Log($"Building {currentBuilding.buildingName} found  {currentBuilding.level + 1}");
                    currentUpgradedBuildings.RemoveAt(i);
                    i--;
                    if (buildingToUpgrade.nameOfBuilding == "Money Factory")
                    {
                        moneyFactoryLevel = buildingToUpgrade.level;
                        Debug.Log("Money Factory upgraded to level: " + moneyFactoryLevel);
                    }
                    else if (buildingToUpgrade.nameOfBuilding == "Mystical Well")
                    {
                        this.wellLevel = buildingToUpgrade.level;
                        Debug.Log("Mysterious Well upgraded to level: " + wellLevel);
                        // Debug.Log("poziom " + wellLevel);
                    
                    }
                    else
                    {
                        Debug.Log("Building upgraded: " + buildingToUpgrade.nameOfBuilding);
                    }
                }
               
            }
        }
    }
    public void UpdateQuestFinishedIndex(int val)
    {
        if (val > questActFinishedIndex)
        {
            questActFinishedIndex = val;
        }
        else Debug.Log("Quest was already finished");
    }
    void Update()
    {
        UpdateUpgradeTimers();
        FindUpgradedBuildingsAndUpdate();
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentUpgradedBuildings = new List<CurrentUpgradedBuilding>();
        }
        else
        {
            Debug.Log("Duplicate GameManager destroyed: " + gameObject.name);
            Destroy(gameObject);
        }
    }
    
    private IEnumerator IncreaseCoinsToCollectOverTime(int amount, float time)
    {
        FindMoneyFactory();
        if (moneyFactory != null)
            moneyFactoryLevel = moneyFactory.level;
        while (true)
        {
            yield return new WaitForSeconds(time);
            switch (moneyFactoryLevel)
            {
                case 1:
                    temporaryCoinsToCollect += amount * (workerCount + 1);
                    break;
                case 2:
                    temporaryCoinsToCollect += amount * 2 *  (workerCount + 1);
                    break;
                case 3:
                    temporaryCoinsToCollect += amount * 3 * (workerCount + 1);;
                    break;
                default:
                    break;

            }
        }
    }
     
    private IEnumerator IncreaseCoinsOverTime(int amount, float time)
    {
        if (moneyFactory != null)
            moneyFactoryLevel = moneyFactory.level;

        while (true)
        {
            yield return new WaitForSeconds(time);
            switch (moneyFactoryLevel)
            {
                case 1:
                    playerCoinCount += amount *  (workerCount + 1);
                    break;
                case 2:
                    playerCoinCount += amount * 2 *  (workerCount + 1);;
                    break;
                case 3:
                    playerCoinCount += amount * 3 *  (workerCount + 1);;
                    break;
                default:
                    break;

            }
        }
    }
    public void increaseCoins(int amount)
    {
        playerCoinCount += amount;
    }
    public void decreaseCoins(int amount)
    {
        playerCoinCount -= amount;
    }
    public bool isFirstPlaythrough = false;

    private void Start()
    {
        StartCoroutine(IncreaseCoinsToCollectOverTime(10, coinsTimeInterval));
           StartCoroutine(RestoreEnergyOverTime(1, 30f)); 
        StartCoroutine(IncreaseCoinsOverTime(1, 0.5f));

        // AddPerks();
    }
    public void AddKey(KeyType keyType, int quantity)
    {
        KeyItem keyItem = keys.Find(k => k.keyType == keyType);
        if (keyItem != null)
        {
            keyItem.quantity += quantity;
        }
        else 
        {
            keys.Add(new KeyItem { keyType = keyType, quantity = quantity });
        }
    }
    public void AddPerks()
    {
        playerPerks.Add(new PlayerPerks
        {
            perkName = "Windwalker's Step",
            perkDescription = "Harness the ancient winds to dash swiftly across the land, leaving only a whisper in your wake.",
            perkLevel = 1,
            perkMaxLevel = 3,
            perkIsActive = true
        });


        playerPerks.Add(new PlayerPerks
        {
            perkName = "Swift Steps",
            perkDescription = "Increases your movement speed, letting you zip around the city faster.",
            perkLevel = 1,
            perkMaxLevel = 3,
            perkIsActive = true
        });

        playerPerks.Add(new PlayerPerks
        {
            perkName = "Iron Constitution",
            perkDescription = "Boosts your maximum health, making you tougher against all odds.",
            perkLevel = 1,
            perkMaxLevel = 3,
            perkIsActive = true
        });

        playerPerks.Add(new PlayerPerks
        {
            perkName = "Mighty Strikes",
            perkDescription = "Enhances your attack power, allowing you to deal more damage.",
            perkLevel = 1,
            perkMaxLevel = 3,
            perkIsActive = false
        });
    }
    private IEnumerator RestoreEnergyOverTime(int amount, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            switch (wellLevel)
            {
                case 1:
                    energy += amount * 1;
                    break;
                case 2:
                    energy += amount * 2;
                    break;
                case 3:
                    energy += amount * 3;
                    break;
                default:
                    break;

            }
            // energy += amount;

            energy = Mathf.Min(energy, 100);
        }
    }

    public void IncreasePlayerHealth(int amount)
    {
        playerHealth += amount;
    }

    public void IncreasePlayerAttack(int amount)
    {
        playerAttack += amount;
    }

    public void IncreasePlayerSpeed(int amount)
    {
        playerSpeed += amount;
    }
}
