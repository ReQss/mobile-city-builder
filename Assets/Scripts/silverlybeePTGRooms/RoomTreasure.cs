using System.Collections.Generic;
using UnityEngine;

public class RoomTreasure : RoomBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> treasureChests;
    void Start()
    {
        for(int i=DungeonGenerator.Instance.numberOfTreasureChests;i<treasureChests.Count;++i)
        {
            treasureChests[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
