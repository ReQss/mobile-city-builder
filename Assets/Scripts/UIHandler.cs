using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    
    public GameObject getRewardsAlert;
    public TextMeshProUGUI getRewardsText;
    public bool loadTutorial = false;
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
    [SerializeField] private GameObject speed1Button;
    [SerializeField] private GameObject speed2Button;
    [SerializeField] private GameObject speed3Button;
    [SerializeField] private GameObject ironConstitution1Button;
    [SerializeField] private GameObject ironConstitution2Button;
    [SerializeField] private GameObject ironConstitution3Button;
    public List<GameObject> uiElements;
    public GameObject darkBackground;
    public GameObject uiTopPanel;
    
    public GameObject alertSuccess;
    public GameObject alertFailure;
    public GameObject continueButton;
    public List<Button> buttonToTriggetOnStart;
    public Vector2Int dungeonSizeSmall = new Vector2Int(3,3);
    public bool IsUIOpen()
    {
        foreach (GameObject go in uiElements)
        {
            if (go.activeSelf)
            {
                Animator animator = go.GetComponent<Animator>();
                if (animator != null && animator.GetBool("IsOpen"))
                {
                    return true;
                }
                else if (animator == null)
                {
                    return true; // If there's no animator, we assume the UI is open if the GameObject is active
                }

            }

        }
        return false;
    }
    public void SetDungeonSizeSmall()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDungeonSize(dungeonSizeSmall,1);
        }
    }
     public void SetDungeonSizeMedium()
    {
        if (GameManager.Instance != null)
        {
        GameManager.Instance.SetDungeonSize(dungeonSizeSmall + new Vector2Int(1,1),2);
        }
    }
        public void SetDungeonSizeLarge()
        {
            if (GameManager.Instance != null)
            {
            GameManager.Instance.SetDungeonSize(dungeonSizeSmall + new Vector2Int(2,2),3);
            }
        }

    void Start()
    {
        if (GameManager.Instance != null)
            SetMoney(GameManager.Instance.playerCoinCount);

        WeaponAndNpcHandling();
        DisableWeaponsThatAreUnlocked();
        DisableWorkersThatAreUnlocked();

        DisablePerkUIElement();
        DisableContinueButton();
        EnableOrDisableInteractionButtons();
        if (uiToDisable.Count > 0)
        {
            DisableUIElements();
        }
    }
    public void EnableOrDisableInteractionButtons()
    {
        if( buttonToTriggetOnStart == null) return;
        if (GameManager.Instance == null)
        {
            foreach (Button button in buttonToTriggetOnStart)
            {
                button.interactable = false;
            }
        }
        else
        {
            foreach (Button button in buttonToTriggetOnStart)
            {
                button.interactable = true;
            }
        }
    }
    public void DisableContinueButton()
    {
        if (continueButton == null) return;
        if (GameManager.Instance == null || GameManager.Instance.selectedClass == null)
        {
            continueButton.SetActive(false);
        }
    }
    private void DisablePerkUIElement()
    {
        if (GameManager.Instance == null || GameManager.Instance.playerPerks == null) return;
        var swiftSteps = GameManager.Instance.playerPerks.Find(p => p.perkName == "Swift Steps");
        if (swiftSteps != null)
        {
            if (swiftSteps.perkLevel >= 1 && speed1Button != null)
                DisableUIElement(speed1Button);
            if (swiftSteps.perkLevel >= 2 && speed2Button != null)
                DisableUIElement(speed2Button);
            if (swiftSteps.perkLevel >= 3 && speed3Button != null)
                DisableUIElement(speed3Button);
        }

        var ironConstitution = GameManager.Instance.playerPerks.Find(p => p.perkName == "Iron Constitution");
        if (ironConstitution != null)
        {
            if (ironConstitution.perkLevel >= 1 && ironConstitution1Button != null)
                DisableUIElement(ironConstitution1Button);
            if (ironConstitution.perkLevel >= 2 && ironConstitution2Button != null)
                DisableUIElement(ironConstitution2Button);
            if (ironConstitution.perkLevel >= 3 && ironConstitution3Button != null)
                DisableUIElement(ironConstitution3Button);
        }
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

        if (GameManager.Instance.weapons.isBowEnabled)
        {
            if (weaponPrefab1 != null)
                DisableUIElement(weaponPrefab1);
        }
        else if (GameManager.Instance.weapons.isCrossbowEnabled)
        {
            // if (weaponPrefab1 != null)
            //     DisableUIElement(weaponPrefab1);
            if (weaponPrefab2 != null)
                DisableUIElement(weaponPrefab2);
        }
        else if (GameManager.Instance.weapons.isRodEnabled)
        {
            // if (weaponPrefab1 != null)
            //     DisableUIElement(weaponPrefab1);
            // if (weaponPrefab2 != null)
            //     DisableUIElement(weaponPrefab2);
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
        if (energyCounter != null && GameManager.Instance != null)
        {
            energyCounter.text = GameManager.Instance.energy.ToString() + "/100";
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
        // if (IsUIOpen())
        // {
        //     GameManager.Instance.isUIOpen = true;
        //     if (darkBackground != null)
        //     {
        //         darkBackground.SetActive(true);
        //     }
        //     if (uiTopPanel != null)
        //     {
        //         uiTopPanel.SetActive(false);
        //     }
        // }
        // else
        // {
        //     if (GameManager.Instance == null)
        //     {
        //         Debug.LogWarning("GameManager.Instance is null in Update!");
        //         return;
        //     }
        //     GameManager.Instance.isUIOpen = false;
        //     if (darkBackground != null)
        //     {
        //         darkBackground.SetActive(false);
        //     }
        //     if (uiTopPanel != null)
        //     {
        //         uiTopPanel.SetActive(true);
        //     }
        // }
     
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
            LevelManager.Instance.LoadScene(sceneName);
            Debug.Log("Ładowanie sceny: " + sceneName);
        }
        else
        {
            Debug.LogError("Scena " + sceneName + " nie istnieje.");
        }
    }
    public void LoadSceneByNameCity(string sceneName)
    {
       
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
             if (loadTutorial  == true)
        {
             LevelManager.Instance.LoadScene("Statues");
        }
        else LevelManager.Instance.LoadScene(sceneName);
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
    public void DisableUIObject(GameObject gameObject)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);

        }
        else
        {
            Debug.LogWarning("GameObject is null in DisableUIObject!");
        }
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
    public void SuccessfulOperation()
    {
        OpenUIObject(alertSuccess);
        StartCoroutine(CloseAlertAfterDelay(alertSuccess));
    }
    public void FailureOperation()
    {
        OpenUIObject(alertFailure);
        StartCoroutine(CloseAlertAfterDelay(alertFailure));
    }
    private IEnumerator CloseAlertAfterDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(1.5f);
        CloseUIObject(gameObject);
    }
    public void OpenUIObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
        Animator animator = gameObject.GetComponent<Animator>() ? gameObject.GetComponent<Animator>() : null;
        if (animator != null)
            animator.SetBool("IsOpen", true);
        GameManager.Instance.isUIOpen = true;
    }
    public void OpenUIInteractiveObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("IsOpen", true);
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
     public void TriggerScaleAnim(GameObject gameobject) 
    {
        if (gameobject != null)
        {
            Animator animator = gameobject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("enable");
            }
        }
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
        {
            FailureOperation();
            return;
        }
        switch (amount)
        {
            case 200:
                GameManager.Instance.weapons.isBowEnabled = true;
                break;
            case 800:
                GameManager.Instance.weapons.isCrossbowEnabled = true;
                break;
            case 2000:
                GameManager.Instance.weapons.isRodEnabled = true;
                break;
        }
       
        GameManager.Instance.decreaseCoins(amount);
        IncreaseWeaponLevel();

        if (weaponPriceToPrefab.TryGetValue(amount, out GameObject prefab) && prefab != null)
        {
            TriggerScaleAnim(prefab);
            StartCoroutine(DisableUIElementAfterDelay(prefab, 0.3f));
        }
        
    }

    private IEnumerator DisableUIElementAfterDelay(GameObject gameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableUIElement(gameObject);
    }
    public void BuySpeed1(int amount)
    {
        BuySpeedPerk(amount, 1);
    }

    public void BuySpeed2(int amount)
    {
        BuySpeedPerk(amount, 2);
    }

    public void BuySpeed3(int amount)
    {
        BuySpeedPerk(amount, 3);
    }

    private void BuySpeedPerk(int amount, int targetLevel)
    {
        if (amount > GameManager.Instance.playerCoinCount)
        {
            FailureOperation();
            return;
        }
        var perk = GameManager.Instance.playerPerks.Find(p => p.perkName == "Swift Steps");
        if (perk != null && perk.perkLevel < targetLevel)
        {
            GameManager.Instance.decreaseCoins(amount);
            perk.perkLevel = targetLevel;
            perk.perkIsActive = true;
            Debug.Log($"Swift Steps upgraded to level {perk.perkLevel}");

            if (targetLevel == 1 && speed1Button != null)
            {
                TriggerScaleAnim(speed1Button);
                StartCoroutine(DisableUIElementAfterDelay(speed1Button, 0.3f));
            }
            else if (targetLevel == 2 && speed2Button != null)
            {
                TriggerScaleAnim(speed2Button);
                StartCoroutine(DisableUIElementAfterDelay(speed2Button, 0.3f));
            }
            else if (targetLevel == 3 && speed3Button != null)
            {
                TriggerScaleAnim(speed3Button);
                StartCoroutine(DisableUIElementAfterDelay(speed3Button, 0.3f));
            }
        }
        else
        {
            Debug.LogWarning("Swift Steps perk not found or already at this level or higher.");
        }
            SuccessfulOperation();
    }
      public void BuyIronConstitution1(int amount)
    {
        BuyIronConstitutionPerk(amount, 1);
    }

    public void BuyIronConstitution2(int amount)
    {
        BuyIronConstitutionPerk(amount, 2);
    }

    public void BuyIronConstitution3(int amount)
    {
        BuyIronConstitutionPerk(amount, 3);
    }
    public List<GameObject> uiToDisable;
     public void DisableUIElements()
    {
        foreach (GameObject gameObject in uiToDisable)
        {
            if (gameObject != null)
                gameObject.SetActive(false);
        }
        Time.timeScale = 0;
    }
    private void BuyIronConstitutionPerk(int amount, int targetLevel)
    {
        if (amount > GameManager.Instance.playerCoinCount)
        {
            FailureOperation();
            return;
        }

        var perk = GameManager.Instance.playerPerks.Find(p => p.perkName == "Iron Constitution");
        if (perk != null && perk.perkLevel < targetLevel)
        {
            GameManager.Instance.decreaseCoins(amount);
            perk.perkLevel = targetLevel;
            perk.perkIsActive = true;
            Debug.Log($"Iron Constitution upgraded to level {perk.perkLevel}");

            if (targetLevel == 1 && ironConstitution1Button != null)
            {
                TriggerScaleAnim(ironConstitution1Button);
                StartCoroutine(DisableUIElementAfterDelay(ironConstitution1Button, 0.3f));
            }
            else if (targetLevel == 2 && ironConstitution2Button != null)
            {
                TriggerScaleAnim(ironConstitution2Button);
                StartCoroutine(DisableUIElementAfterDelay(ironConstitution2Button, 0.3f));
            }
            else if (targetLevel == 3 && ironConstitution3Button != null)
            {
                TriggerScaleAnim(ironConstitution3Button);
                StartCoroutine(DisableUIElementAfterDelay(ironConstitution3Button, 0.3f));
            }
        }
        else
        {
            Debug.LogWarning("Iron Constitution perk not found or already at this level or higher.");
        }
        SuccessfulOperation();
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
   public void GetRewards()
    {

        SetMoneyToCollect();
        OpenUIObject(getRewardsAlert);
        SetRewardsCost(getRewardsText);
    }

}
