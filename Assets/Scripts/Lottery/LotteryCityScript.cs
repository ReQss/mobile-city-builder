using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public enum LotteryType
{
    Bronze,
    Silver,
    Gold
}
public class LotteryCityScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public LotteryType lotteryType;
    public Animator levelAnimator;
    [Header("Item slot ")]
    public Image itemImageSlot;
    public TextMeshProUGUI itemText;
    public TextMeshProUGUI keyCountText;
    public Image keyImageSlot;
    [Header("Sprites")]
    public List<Sprite> bronzeSprites = new List<Sprite>();
    public List<Sprite> keySprites = new List<Sprite>();
    private KeyItem bronzeKey;
    private KeyItem silverKey;
    private KeyItem goldKey;
    void Start()
    {
        InitKeyCount();
    }
    public void InitKeyCount()
    {
        bronzeKey = GameManager.Instance.keys.Find(k => k.keyType == KeyType.Bronze);
        silverKey = GameManager.Instance.keys.Find(k => k.keyType == KeyType.Silver);
        goldKey = GameManager.Instance.keys.Find(k => k.keyType == KeyType.Gold);
        UpdateKeyCount();
    }
    public void UseLever()
    {
        switch (lotteryType)
        {
            case LotteryType.Bronze:
                BronzeLottery();
                break;
            case LotteryType.Silver:
                SilverLottery();
                break;
            case LotteryType.Gold:
                GoldLottery();
                break;
        }
    }
    public void SetLotteryType(int lotteryType)
    {
        this.lotteryType = (LotteryType)lotteryType;
        UpdateKeyImage();
        UpdateKeyCount();
    }
    
    public void UpdateKeyImage()
    {
        switch (lotteryType)
        {
            case LotteryType.Bronze:
                keyImageSlot.sprite = keySprites[0];
                break;
            case LotteryType.Silver:
                keyImageSlot.sprite = keySprites[1];
                break;
            case LotteryType.Gold:
                keyImageSlot.sprite = keySprites[2];
                break;
        }
    }
    public void UpdateKeyCount()
    {
        switch (lotteryType)
        {
            case LotteryType.Bronze:
                keyCountText.text = bronzeKey != null ? bronzeKey.quantity.ToString() : "0";
                break;
            case LotteryType.Silver:
                keyCountText.text = silverKey != null ? silverKey.quantity.ToString() : "0";
                break;
            case LotteryType.Gold:
                keyCountText.text = goldKey != null ? goldKey.quantity.ToString() : "0";
                break;
        }
    }
    // exp lub gold
    public void BronzeLottery()
    {
        if (bronzeKey == null || bronzeKey.quantity <= 0)
            return;
        levelAnimator.SetTrigger("UseLever");
        bronzeKey.quantity--; 
        UpdateKeyCount();
        int rollExpOrGold = Random.Range(0, 2); // 0 - exp, 1 - gold
        switch (rollExpOrGold)
        {
            case 0:
                int expAmount = Random.Range(0, GameManager.Instance.playerExperienceToGetLevel); // losowa ilosc expa
                GameManager.Instance.playerCurrentExperience += expAmount;
                itemText.text = expAmount.ToString();
                itemImageSlot.sprite = bronzeSprites[0];
                //  dodac wyswietlanie statystyk po zdobyciu poziomu w miescie
                break;
            case 1:
                int goldAmount = Random.Range(20, 101); // losowa ilosc golda
                itemText.text = goldAmount.ToString();
                itemImageSlot.sprite = bronzeSprites[1];
                GameManager.Instance.coinsCollected += goldAmount;
                break;
        }
    }
    // wiecej expa lub golda mala szansa na item
    public void SilverLottery()
    {
        if (silverKey == null || silverKey.quantity <= 0)
            return;
        levelAnimator.SetTrigger("UseLever");
        silverKey.quantity--; // Zmiana w GameManager, bo to referencja
        UpdateKeyCount();
        int rollExpOrGold = Random.Range(0, 2); // 0 - exp, 1 - gold
        switch (rollExpOrGold)
        {
            case 0:
                int expAmount = Random.Range(GameManager.Instance.playerExperienceToGetLevel / 2, GameManager.Instance.playerExperienceToGetLevel); // losowa ilosc expa
                GameManager.Instance.playerCurrentExperience += expAmount;
                itemText.text = expAmount.ToString();
                itemImageSlot.sprite = bronzeSprites[0];
                //  dodac wyswietlanie statystyk po zdobyciu poziomu w miescie
                break;
            case 1:
                int goldAmount = Random.Range(100, 301); // losowa ilosc golda
                itemText.text = goldAmount.ToString();
                itemImageSlot.sprite = bronzeSprites[1];
                GameManager.Instance.coinsCollected += goldAmount;
                break;
        }
    }
    // item
    public void GoldLottery()
    {

    }
}
