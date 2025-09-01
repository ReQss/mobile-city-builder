using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public enum KeyType
{
    Bronze,
    Silver,
    Gold
}
public class DungeonRewardsInfo : MonoBehaviour
{
    public int goldCollected;
    public int experienceCollected;
    public int levelCollected;
    public KeyType keyCollected;
    public List<Sprite> itemsCollected = new List<Sprite>();
    public List<TextMeshProUGUI> rewardsStatsText = new List<TextMeshProUGUI>();
    public List<Image> rewardsItemsImagesSlots = new List<Image>();
    [SerializeField]
    private Image keyImageSlot;
    [SerializeField]
    private List <Sprite> keyImageSprites = new List<Sprite>();
    public static DungeonRewardsInfo Instance { get; private set; }
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        goldCollected = 0;
        experienceCollected = 0;
        levelCollected = 0;
        itemsCollected.Clear();
    }
    public void UpdateRewardsStatsText()
    {
        rewardsStatsText[0].text = goldCollected.ToString();
        rewardsStatsText[1].text = experienceCollected.ToString();
        rewardsStatsText[2].text = levelCollected.ToString();
    }

    public void AddItemImage(Sprite image)
    {
        itemsCollected.Add(image);
    }
    public void UpdateRewardsItemsImages()
    {
        for (int i = 0; i < itemsCollected.Count; i++)
        {
            rewardsItemsImagesSlots[i].sprite = itemsCollected[i];
        }
    }
    public void GetRandomRewardKey()
    {
        float rand = Random.value;
        if (rand < 0.5f)
            keyCollected = KeyType.Bronze;
        else if (rand < 0.8f)
            keyCollected = KeyType.Silver;
        else
            keyCollected = KeyType.Gold;
        SetKeyImage();
        GameManager.Instance.AddKey(keyCollected, 1);
    }
    public void SetKeyImage()
    {
        switch (keyCollected)
        {
            case KeyType.Bronze:
                keyImageSlot.sprite = keyImageSprites[0];
                break;
            case KeyType.Silver:
                keyImageSlot.sprite = keyImageSprites[1];
                break;
            case KeyType.Gold:
                keyImageSlot.sprite = keyImageSprites[2];
                break;
        }
    }
}
