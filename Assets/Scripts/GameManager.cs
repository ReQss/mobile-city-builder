using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    private IEnumerator IncreaseCoinsOverTime(int amount, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            temporaryCoinsToCollect += amount;
            Debug.Log("Coins to collect: " + temporaryCoinsToCollect);
        }
    }
    public void increaseCoins(int amount)
    {
        playerCoinCount += amount;
    }


    private void Start()
    {
        StartCoroutine(IncreaseCoinsOverTime(10, coinsTimeInterval));

    }
}
