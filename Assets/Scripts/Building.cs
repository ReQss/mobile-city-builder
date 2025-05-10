using System.Collections;
using TMPro;
using UnityEngine;

public class Building : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Building Details")]
    public int level;
    public int cost;
    public float TimeToUpgrade = 60f;
    public string nameOfBuilding;
    public string descriptionOfBuilding;

    [Header("Building Details")]
    [SerializeField]
    private TextMeshProUGUI buildingLevel;
    [SerializeField]
    private TextMeshProUGUI buildingName;
    [SerializeField]
    private TextMeshProUGUI buildingDescription;


    void Start()
    {
        SetBuildingUI();
    }
    private void SetBuildingUI()
    {
        if (buildingLevel != null && buildingName != null && buildingDescription != null)
        {
            buildingLevel.text = level.ToString();
            buildingName.text = nameOfBuilding;
            buildingDescription.text = descriptionOfBuilding;
        }
        else Debug.Log("UI not found");
    }
    private void SetBuildingValues(int level, int cost, string nameOfBuilding, string descriptionOfBuilding)
    {
        this.level = level;
        this.cost = cost;
        this.nameOfBuilding = nameOfBuilding;
        this.descriptionOfBuilding = descriptionOfBuilding;
    }
    // Update is called once per frame
    void Update()
    {
        SetBuildingUI();
    }
    public void UpgradeBuilding()
    {
        if (GameManager.Instance.playerCoinCount >= cost)
        {
            StartCoroutine(UpgradingBuilding());

        }
        else
        {
            Debug.Log("Not enough coins to upgrade the building.");
        }
    }
    IEnumerator UpgradingBuilding()
    {
        GameObject buildingUpgraded = GameManager.Instance.currentPickedBuilding;
        GameManager.Instance.isWorkerUpgrading = true;

        // Find the "Loading" object anywhere under buildingUpgraded and enable it
        Transform loadingTransform = buildingUpgraded.transform.Find("Loading");
        if (loadingTransform == null)
        {
            // If not found directly, search recursively
            loadingTransform = buildingUpgraded.transform.GetComponentInChildren<Transform>(true);
            foreach (Transform t in buildingUpgraded.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "Loading")
                {
                    loadingTransform = t;
                    break;
                }
            }
        }
        TextMeshProUGUI loadingText = null;
        if (loadingTransform != null)
        {
            loadingTransform.gameObject.SetActive(true);

            // Find "RawImage" inside "Loading"
            Transform rawImageTransform = loadingTransform.Find("RawImage");
            if (rawImageTransform != null)
            {
                // Find "LoadingText" inside "RawImage" and set its text to TimeToUpgrade
                Transform loadingTextTransform = rawImageTransform.Find("LoadingText");
                if (loadingTextTransform != null)
                {
                    loadingText = loadingTextTransform.GetComponent<TextMeshProUGUI>();
                    if (loadingText != null)
                    {
                        loadingText.text = TimeToUpgrade.ToString();
                    }
                }
            }
        }

        yield return null;
        GameManager.Instance.playerCoinCount -= cost;


        // Countdown logic: decrease value every 1s
        float timeLeft = TimeToUpgrade;
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
            if (loadingText != null)
            {
                loadingText.text = Mathf.CeilToInt(timeLeft).ToString();
            }
        }

        GameManager.Instance.isWorkerUpgrading = false;

        // Optionally, turn off the Loading object after upgrade
        if (loadingTransform != null)
        {
            loadingTransform.gameObject.SetActive(false);
        }
        level++;
        cost = cost * 2; // Example increment, adjust as needed
        SetBuildingUI();
    }
}
