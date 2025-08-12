using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[System.Serializable]
public class DoorAndWall
{
    public GameObject door;
    public GameObject wall;

    public DoorAndWall(GameObject door, GameObject wall)
    {
        this.door = door;
        this.wall = wall;
    }
}
public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls; // 0 - Up 1 -Down 2 - Right 3- Left
    public GameObject[] doors;
    public List<DoorAndWall> openDoors = new List<DoorAndWall>();
    private bool isUnlockingDoors = false;

    public void UpdateRoom(bool[] status)
    {
        openDoors.Clear();
        for (int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
            if (status[i])
            {
                DoorAndWall doorAndWall = new DoorAndWall(doors[i], walls[i]);
                openDoors.Add(doorAndWall);
            }
        }
    }
    public async Task UnlockOrLockDoors(bool isUnlocking)
    {
        if (isUnlockingDoors)
            return;
        isUnlockingDoors = true;
        foreach (DoorAndWall doorAndWall in openDoors)
        {
            doorAndWall.door.SetActive(isUnlocking);
            doorAndWall.wall.SetActive(!isUnlocking);
        }
        await Task.Delay(100);
        isUnlockingDoors = false;
    }
    
}
