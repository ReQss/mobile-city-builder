using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BuildingToUnlock
{
    public GameObject unlockedBuilding;
    public GameObject disabledBuilding;
    public GameObject activationPanel;
    public bool isUnlocked = false;
    public void UnlockBuilding()
    {
        if (!isUnlocked)
        {
            if (unlockedBuilding)
                unlockedBuilding.SetActive(true);
            if(disabledBuilding)
            disabledBuilding.SetActive(false);
            isUnlocked = true;
        }
    }
    public void ActivatePanel()
    {
        if(activationPanel)
            activationPanel.SetActive(true);
        
    }
}
public class BuildingUnlocker : MonoBehaviour
{
    public static BuildingUnlocker Instance { get; private set; }
    public AnimationFramesBuildingUnlocker animationFramesBuildingUnlocker;

    public List<BuildingToUnlock> buildings;
    public int index = 0;
    
    public void SetIndex(int idx)
    {
        if (idx == index) return;
        Debug.Log("index set");
        animationFramesBuildingUnlocker.InitFrames(null);
        index = idx;
        if (index == 0)
            GameManager.Instance.unlockedContent.magicShopActivated = true;
        else if (index == 1)
            GameManager.Instance.unlockedContent.armorShopActivated = true;
        else if (index == 2)
            GameManager.Instance.unlockedContent.weaponShopActivated = true;
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void ActivateUnlockedBuildings()
    {
        UnlockedContent unlocked = GameManager.Instance.unlockedContent;
        if (unlocked.magicShopUnlocked)
        {
            buildings[0].ActivatePanel();
        }
        if (unlocked.armorShopUnlocked)
        {
            buildings[1].ActivatePanel();
        }
        if (unlocked.weaponShopUnlocked)
        {
            buildings[2].ActivatePanel();
        }
    }
    public void EnableAlreadyUnlockedBuildings()
    {
        if (GameManager.Instance.unlockedContent.magicShopActivated)
        {
            buildings[0].UnlockBuilding();
        }
        if (GameManager.Instance.unlockedContent.armorShopActivated)
        {
            buildings[1].UnlockBuilding();
        }
        if (GameManager.Instance.unlockedContent.weaponShopActivated)
        {
            buildings[2].UnlockBuilding();
        }
        if (GameManager.Instance.unlockedContent.lotteryActivated)
        {
            buildings[3].UnlockBuilding();
        }
        if(GameManager.Instance.unlockedContent.moneyFactoryActivated)
        {
            buildings[4].UnlockBuilding();
        }
    }
    void Start()
    {
        ActivateUnlockedBuildings();
        EnableAlreadyUnlockedBuildings();
    }

    void Update()
    {

    }
}
