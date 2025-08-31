using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonRewardsInfo : MonoBehaviour
{
    public int goldCollected;
    public int experienceCollected;
    public int levelCollected;
    public List<Sprite> itemsCollected = new List<Sprite>();
    public List<TextMeshProUGUI> rewardsStatsText = new List<TextMeshProUGUI>();
    public List<Image> rewardsItemsImagesSlots = new List<Image>();
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
    
}
