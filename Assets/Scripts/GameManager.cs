using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
public class GameManager : MonoBehaviour
{
    public Building moneyFactory;
    public Player playerStats;
    [SerializeField]
    public List<Armor> armors;
    public int energy = 100;
    public int playerCoinCount = 0;
    public int temporaryCoinsToCollect = 0;
    public float coinsTimeInterval = 3f;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Duplicate GameManager destroyed: " + gameObject.name);
            Destroy(gameObject);
        }
    }
    private IEnumerator IncreaseCoinsToCollectOverTime(int amount, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            switch (moneyFactory.level)
            {
                case 1:
                    temporaryCoinsToCollect += amount;
                    break;
                case 2:
                    temporaryCoinsToCollect += amount * 5;
                    break;
                case 3:
                    temporaryCoinsToCollect += amount * 20;
                    break;
                default:
                    break;

            }
        }
    }
    private IEnumerator IncreaseCoinsOverTime(int amount, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            // Debug.Log("Coin updae" + playerCoinCount);
            switch (moneyFactory.level)
            {
                case 1:
                    playerCoinCount += amount;
                    break;
                case 2:
                    playerCoinCount += amount * 5;
                    break;
                case 3:
                    playerCoinCount += amount * 20;
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


    private void Start()
    {
        StartCoroutine(IncreaseCoinsToCollectOverTime(10, coinsTimeInterval));
        StartCoroutine(IncreaseCoinsOverTime(1, 0.5f));

    }
}
