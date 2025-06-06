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
    private TextMeshProUGUI energyCounter;
    [SerializeField]
    private GameObject coinBubble;
    public GameObject rewardsPrefab;
    private int amountToUse = 0;
    public GameObject weaponPrefab1;
    public GameObject weaponPrefab2;
    public GameObject weaponPrefab3;
    public List<GameObject> workerPrefabs;
    public List<GameObject> workerPrefabsEnabled;
    public List<GameObject> npcWorkersPrefabs;
    private Dictionary<int, GameObject> weaponPriceToPrefab;
    private Dictionary<int, GameObject> workerPriceToPrefab;
    private Dictionary<int, GameObject> workerPriceToPrefabEnabled;
    private Dictionary<int, GameObject> npcWorkerPriceToPrefab;
    void Start()
    {
        if (GameManager.Instance != null)
            SetMoney(GameManager.Instance.playerCoinCount);

        WeaponAndNpcHandling();
        DisableWeaponsThatAreUnlocked();
        DisableWorkersThatAreUnlocked();
    }
    public void WeaponAndNpcHandling()
    {
          weaponPriceToPrefab = new Dictionary<int, GameObject>
        {
            { 200, weaponPrefab1 },
            { 800, weaponPrefab2 },
            { 2000, weaponPrefab3 }
        };
        workerPriceToPrefab = new Dictionary<int, GameObject>();
        int price = 1000;
        for (int i = 0; i < workerPrefabs.Count; i++)
        {
            workerPriceToPrefab.Add(price, workerPrefabs[i]);
            price *= 2;
        }
         workerPriceToPrefabEnabled = new Dictionary<int, GameObject>();
        price = 1000;
        for (int i = 0; i < workerPrefabs.Count; i++)
        {
            workerPriceToPrefabEnabled.Add(price, workerPrefabsEnabled[i]);
            price *= 2;
        }

        npcWorkerPriceToPrefab = new Dictionary<int, GameObject>();
         price = 1000;
        for (int i = 0; i < npcWorkersPrefabs.Count; i++)
        {
            npcWorkerPriceToPrefab.Add(price, npcWorkersPrefabs[i]);
            price *= 2;
        }

    }
    public void DisableWeaponsThatAreUnlocked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance is null in DisableWeaponsThatAreUnlocked!");
            return;
        }

        if (GameManager.Instance.weaponLevel == 1)
        {
            if (weaponPrefab1 != null)
                DisableUIElement(weaponPrefab1);
        }
        else if (GameManager.Instance.weaponLevel == 2)
        {
            if (weaponPrefab1 != null)
                DisableUIElement(weaponPrefab1);
            if (weaponPrefab2 != null)
                DisableUIElement(weaponPrefab2);
        }
        else if (GameManager.Instance.weaponLevel == 3)
        {
            if (weaponPrefab1 != null)
                DisableUIElement(weaponPrefab1);
            if (weaponPrefab2 != null)
                DisableUIElement(weaponPrefab2);
            if (weaponPrefab3 != null)
                DisableUIElement(weaponPrefab3);

        }
    }
    public void DisableWorkersThatAreUnlocked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance is null in DisableWorkersThatAreUnlocked!");
            return;
        }

        for (int i = 0; i < GameManager.Instance.workerCount && i < workerPrefabs.Count; i++)
        {
            if (workerPrefabs[i] != null)
                DisableUIElement(workerPrefabs[i]);
        }
        for (int i = 0; i < GameManager.Instance.workerCount && i < workerPrefabsEnabled.Count; i++)
        {
            if (workerPrefabsEnabled[i] != null)
                EnableUIElement(workerPrefabsEnabled[i]);
        }


        for (int i = 0; i < GameManager.Instance.workerCount && i < npcWorkersPrefabs.Count; i++)
        {
            if (npcWorkersPrefabs[i] != null)
                EnableUIElement(npcWorkersPrefabs[i]);
        }
       
    }
    // Update is called once per frame
    void Update()
    {
        if (coinCounter != null && GameManager.Instance != null)
            coinCounter.text = GameManager.Instance.playerCoinCount.ToString();
        if(energyCounter != null && GameManager.Instance != null){
            energyCounter.text = GameManager.Instance.energy.ToString()+"/100";
        }
            
        if (GameManager.Instance != null && GameManager.Instance.coinsCollected > 0)
        {
            if (rewardsPrefab != null)
                rewardsPrefab.SetActive(true);
        }
        else if (GameManager.Instance != null && GameManager.Instance.coinsCollected <= 0 && rewardsPrefab != null)
        {
            if (rewardsPrefab != null)
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
      public void LoadLevelByName(string sceneName)
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.energy >= GameManager.Instance.energyRequiredForQuest)
            {
                GameManager.Instance.energy -= GameManager.Instance.energyRequiredForQuest;
                LoadSceneByName(sceneName);
            }
        }
        
    }
    public void CloseUIObject(GameObject gameObject)
    {
        // gameObject.SetActive(false);
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator != null)
            animator.SetBool("IsOpen", false);
        else Debug.Log("Not found");
        GameManager.Instance.isUIOpen = false;
    }
     public void CloseUIObjectNoAnimation(GameObject gameObject)
    {
         gameObject.SetActive(false);
       
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
        if(animator!=null)
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
    public void BuyWorker(int amount)
    {
        if (amount > GameManager.Instance.playerCoinCount)
            return;

        GameManager.Instance.decreaseCoins(amount);
        IncreaseWorkerCount();

        if (workerPriceToPrefab.TryGetValue(amount, out GameObject prefab) && prefab != null)
        {
            DisableUIElement(prefab);
        }
        if (workerPriceToPrefabEnabled.TryGetValue(amount, out GameObject prefabEnabled) && prefabEnabled != null)
        {
            EnableUIElement(prefabEnabled);
        }
        if (npcWorkerPriceToPrefab.TryGetValue(amount, out GameObject npcPrefab) && npcPrefab != null)
        {
            EnableUIElement(npcPrefab);
        }
    }
    public void IncreaseWeaponLevel()
    {
        GameManager.Instance.weaponLevel++;

    }
    public void IncreaseWorkerCount()
    {
        GameManager.Instance.workerCount++;
        
    }
    public void UpgradePickedBuilding()
    {
        GameManager.Instance.isUIOpen = false;
        // GameManager.Instance.currentPickedBuilding.GetComponent<Building>().UpgradeBuilding();
        GameManager.Instance.currentPickedBuilding.GetComponent<Building>().UpgradeBuilding2();

    }
    public void DisableUIElement(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    public void EnableUIElement(GameObject gameObject)
    {
        gameObject.SetActive(true);
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
