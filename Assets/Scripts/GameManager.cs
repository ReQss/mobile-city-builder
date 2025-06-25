using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
public class Weapon
{
    public int id;
    public string name;
    public string description;
    public int attackBonus;
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
public class GameManager : MonoBehaviour
{
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
    public int playerExperienceToGetLevel = 1000;
    public int playerCurrentExperience = 0;
    public int pointsToSpend = 0;
    
    public int priceForStatistics = 1000;


    
   
    public static GameManager Instance { get; private set; }
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
        if (pointsToSpend-1 > 0)
        {
            playerSpeed += 1;
            pointsToSpend -= 2;
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

        // if (isPlayerInteracting && Time.timeScale != 0)
        // {
        //     Time.timeScale = 0;
        // }
        // else if (!isPlayerInteracting && Time.timeScale != 1)
        // {
        //     Time.timeScale = 1;
        // }
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


    private void Start()
    {
        StartCoroutine(IncreaseCoinsToCollectOverTime(10, coinsTimeInterval));
           StartCoroutine(RestoreEnergyOverTime(1, 30f)); 
        StartCoroutine(IncreaseCoinsOverTime(1, 0.5f));

        // AddPerks();
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
