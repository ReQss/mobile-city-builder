using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BuildingToUnlock
{
    public GameObject unlockedBuilding;
    public GameObject disabledBuilding;
    public bool isUnlocked = false;
    public void UnlockBuilding()
    {
        if (!isUnlocked)
        {
            unlockedBuilding.SetActive(true);
            disabledBuilding.SetActive(false);
            isUnlocked = true;
        }
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
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
