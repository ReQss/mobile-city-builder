using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> uiBuildingObjects;
    [SerializeField]
    private TextMeshProUGUI coinCounter;
    [SerializeField]
    private GameObject coinBubble;
    public GameObject rewardsPrefab;
    private int amountToUse = 0;
    public GameObject weaponPrefab1;
    public GameObject weaponPrefab2;
    public GameObject weaponPrefab3;

    // Add this dictionary to map weapon prices to prefabs
    private Dictionary<int, GameObject> weaponPriceToPrefab;

    void Start()
    {
        if (GameManager.Instance != null)
            SetMoney(GameManager.Instance.playerCoinCount);

        // Initialize the dictionary
        weaponPriceToPrefab = new Dictionary<int, GameObject>
        {
            { 200, weaponPrefab1 },
            { 800, weaponPrefab2 },
            { 2000, weaponPrefab3 }
        };
        DisableWeaponsThatAreUnlocked();
    }
    public void DisableWeaponsThatAreUnlocked()
    {
        if (GameManager.Instance.weaponLevel == 1)
        {
            if(weaponPrefab1 != null)
                DisableUIElement(weaponPrefab1);
            DisableUIElement(weaponPrefab1);
        }
        else if (GameManager.Instance.weaponLevel == 2)
        {
            if(weaponPrefab1 != null)
                DisableUIElement(weaponPrefab1);
            if (weaponPrefab2 != null)
                DisableUIElement(weaponPrefab2);
        }
        else if (GameManager.Instance.weaponLevel == 3)
        {
            if(weaponPrefab1 != null)
                DisableUIElement(weaponPrefab1);
            if(weaponPrefab2 != null)
                DisableUIElement(weaponPrefab2);
            if(weaponPrefab3 != null)
                DisableUIElement(weaponPrefab3);
          
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (coinCounter != null)
            coinCounter.text = GameManager.Instance.playerCoinCount.ToString();
        if (GameManager.Instance.coinsCollected > 0 && rewardsPrefab != null)
        {
            rewardsPrefab.SetActive(true);
        }
        else if (GameManager.Instance.coinsCollected <= 0 && rewardsPrefab != null)
        {
            rewardsPrefab.SetActive(false);
        }

    }
    private IEnumerator UpdateCoinCount(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            if (coinCounter != null)
                coinCounter.text = GameManager.Instance.playerCoinCount.ToString();
        }
    }
    public void TestDebug()
    {
        Debug.Log("Button is worting");
    }
    public void LoadSceneByName(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("Ładowanie sceny: " + sceneName);
        }
        else
        {
            Debug.LogError("Scena " + sceneName + " nie istnieje.");
        }
    }
    public void CloseUIObject(GameObject gameObject)
    {
        // gameObject.SetActive(false);
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("IsOpen", false);
        GameManager.Instance.isUIOpen = false;
    }
    public void CloseUIInteractiveObject(GameObject gameObject)
    {
        // gameObject.SetActive(false);
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("IsOpen", false);
        // GameManager.Instance.isUIOpen = false;
    }
    public void CloseListOfUIObjects()
    {
        foreach (GameObject go in uiBuildingObjects)
        {
            go.SetActive(false);
        }
        GameManager.Instance.isUIOpen = false;

    }
    public void CloseListOfUIObjectsWithAnimation()
    {
        foreach (GameObject go in uiBuildingObjects)
        {
            CloseUIObject(go);
        }
        GameManager.Instance.isUIOpen = false;
    }
    public void OpenUIObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("IsOpen", true);
        GameManager.Instance.isUIOpen = true;
    }
    public void OpenUIInteractiveObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("IsOpen", true);
        // GameManager.Instance.isUIOpen = true;
    }


    public void CollectMoney()
    {
        int collectedMoney = GameManager.Instance.temporaryCoinsToCollect;
        GameManager.Instance.increaseCoins(collectedMoney);
        GameManager.Instance.temporaryCoinsToCollect = 0;
        if (coinCounter != null)
            coinCounter.text = GameManager.Instance.playerCoinCount.ToString();
        if (coinBubble != null)
        {
            CloseUIInteractiveObject(coinBubble);
            StartCoroutine(openAlertAfterTime());
        }
        else Debug.Log("ui not signed");

    }
    private IEnumerator openAlertAfterTime()
    {
        yield return new WaitForSeconds(5f);
        OpenUIInteractiveObject(coinBubble);
    }

    public void SetMoney(int amount)
    {
        amountToUse = amount;
        if (coinCounter != null)
            coinCounter.text = amount.ToString();
    }
    public void SetMoneyToCollect()
    {
         amountToUse = GameManager.Instance.coinsCollected;
    }
    public void SetBuildingToUse(GameObject pickedBuidling)
    {
        GameManager.Instance.currentPickedBuilding = pickedBuidling;
    }
    public void ResetMoneysToCollect()
    {
        GameManager.Instance.coinsCollected = 0;
      
    }
    public void BuyWeapon(int amount)
    {
        if (amount > GameManager.Instance.playerCoinCount)
            return;

        GameManager.Instance.decreaseCoins(amount);
        IncreaseWeaponLevel();

        if (weaponPriceToPrefab.TryGetValue(amount, out GameObject prefab) && prefab != null)
        {
            DisableUIElement(prefab);
        }
    }
    public void IncreaseWeaponLevel()
    {
        GameManager.Instance.weaponLevel++;

    }
    public void UpgradePickedBuilding()
    {
        GameManager.Instance.isUIOpen = false;
        GameManager.Instance.currentPickedBuilding.GetComponent<Building>().UpgradeBuilding();
    }
    public void DisableUIElement(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    public void GetMoney()
    {
        GameManager.Instance.increaseCoins(amountToUse);
        if (coinCounter != null)
            coinCounter.text = GameManager.Instance.playerCoinCount.ToString();

    }
    public void SetAlertCost(TextMeshProUGUI alertCostText)
    {
        if (alertCostText != null && GameManager.Instance.currentPickedBuilding != null)
        {
            Building building = GameManager.Instance.currentPickedBuilding.GetComponent<Building>();
            if (building != null)
            {
                alertCostText.text = building.cost.ToString();
            }
        }
    }
    public void SetRewardsCost(TextMeshProUGUI alertCostText)
     {
        if (alertCostText != null )
        {
          
                alertCostText.text = amountToUse.ToString();
            
        }
    }

}
