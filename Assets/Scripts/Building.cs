using TMPro;
using UnityEngine;

public class Building : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Building Details")]
    public int level;
    public int cost;
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
}
