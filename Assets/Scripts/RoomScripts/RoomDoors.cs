using UnityEngine;

public class RoomDoors : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private bool isEnabled;
    [SerializeField]
    private RoomBehaviour roomBehaviour;
    [SerializeField]
    private GameObject enemiesFolder;
    private bool isRoomCleared = false;
    private bool isPlayerInRoom = false;
    [SerializeField]
    private RoomEnemiesGenerator roomEnemiesGenerator;
    private bool wereEnemiesGenerated = false;

    // Update is called once per frame
    void Update()
    {
        if(isRoomCleared== false && isPlayerInRoom)
            CheckAliveEnemies();
    }
    public void CheckAliveEnemies()
    {
        if (enemiesFolder != null)
        {
            if (enemiesFolder.transform.childCount == 0)
            {
                // true otwiera drzwi 
                // false zamyka drzwi
                _ = roomBehaviour.UnlockOrLockDoors(true);
                isRoomCleared = true;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRoom = true;
            if (isEnabled && isRoomCleared == false && wereEnemiesGenerated == false)
            {
                if (roomEnemiesGenerator == null) return;
                roomEnemiesGenerator.SpawnObjectsNumber();
                wereEnemiesGenerated = true;
                // _ = roomBehaviour.UnlockOrLockDoors(false);
            }
        }
    }
}
