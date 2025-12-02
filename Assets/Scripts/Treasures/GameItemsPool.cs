using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemsPool : MonoBehaviour
{
    public static GameItemsPool Instance { get; private set; }
    public GameObject healItemPrefab;
    public GameObject moneyItemPrefab;
    public GameObject expItemPrefab;
    public GameObject speedItemPrefab;
    public GameObject equipmentItemPrefab;

    public int poolSize = 10;
    private Dictionary<GameObject, Queue<GameObject>> itemPools = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // Add all prefabs you want to pool here
        InitializePool(healItemPrefab);
        InitializePool(moneyItemPrefab);
        InitializePool(expItemPrefab);
        InitializePool(speedItemPrefab);
        InitializePool(equipmentItemPrefab);
        // Add more prefabs as needed
    }

    private void InitializePool(GameObject prefab)
    {
        if (prefab == null) return;
        if (!itemPools.ContainsKey(prefab))
            itemPools[prefab] = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject item = Instantiate(prefab, this.transform);
            item.SetActive(false);
            itemPools[prefab].Enqueue(item);
        }
    }

    public GameObject GetItem(GameObject prefab)
    {
        if (prefab == null) return null;
        if (!itemPools.ContainsKey(prefab))
            itemPools[prefab] = new Queue<GameObject>();
        GameObject item;
        if (itemPools[prefab].Count > 0)
        {
            item = itemPools[prefab].Dequeue();
            item.SetActive(true);
        }
        else
        {
            item = Instantiate(prefab, this.transform);
        }
        // Set prefabSource for LootItem component if present
        var lootItem = item.GetComponent<LootItem>();
        if (lootItem != null)
        {
            lootItem.prefabSource = prefab;
        }
        return item;
    }

    public void ReturnItem(GameObject prefab, GameObject item)
    {
        if (prefab == null || item == null) return;
        item.SetActive(false);
        if (!itemPools.ContainsKey(prefab))
            itemPools[prefab] = new Queue<GameObject>();
        itemPools[prefab].Enqueue(item);
    }

    public GameObject GetRandomItem()
    {
        if (itemPools.Count == 0) return null;
        var keys = new List<GameObject>(itemPools.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            int idx = Random.Range(0, keys.Count);
            var prefab = keys[idx];
            if (itemPools[prefab].Count > 0)
                return GetItem(prefab);
        }
        return null;
    }
}
