using UnityEngine;
[System.Serializable]
public enum ItemType
{
    Equipment,
    Money,
    Exp,
    Heal,
    TemporarySpeed,

}
public class LootItem : MonoBehaviour
{
    public ItemType itemType;
    public int amount;
    public GameObject prefabSource; // Set this when spawning from pool
    public float movementSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetRandomAmount();
    }
    public void GetRandomAmount()
    {
        switch (itemType)
        {
            case ItemType.Money:
                amount = Random.Range(100, 300); // 5 to 15
                break;
            case ItemType.Exp:
                amount = Random.Range(100, GameManager.Instance.playerExperienceToGetLevel/10); // 10 to 25
                break;
            case ItemType.Heal:
                amount = Random.Range(20, 30); // 15 to 30
                break;
            case ItemType.TemporarySpeed:
                amount = 2;
                break;
            default:
                amount = 0;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")&& GetComponent<BoxCollider>().bounds.Contains(other.transform.position))
        {
            Debug.Log("Player is in the loot item trigger area.");
            CollectItem();


           
        }
    }
    public void CollectItem()
    {
         switch (itemType)
            {
                case ItemType.Money:
                    GameManager.Instance.coinsCollected += amount;
                    break;
                case ItemType.Exp:
                    GameManager.Instance.AddExp(amount);
                    break;
                case ItemType.Heal:
                    PlayerMovement.playerMovementInstance.HealPlayer(amount);
                    break;
                default:
                    break;
            }
        if (prefabSource != null)
        {
            GameItemsPool.Instance.ReturnItem(prefabSource, gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player")&&  GetComponent<SphereCollider>().bounds.Contains(other.transform.position))
        {
           
              transform.position = Vector3.MoveTowards(
            transform.position,
            other.transform.position,
            movementSpeed * Time.deltaTime
        );
        if (Vector3.Distance(transform.position, other.transform.position) < 0.7f)
        {
            CollectItem();
        }
        }
    }
}
