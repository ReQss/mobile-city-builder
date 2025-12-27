using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public enum EffectType
{
    None,
    Slow,
    Poison,
    Burn,
    Healing
}
[System.Serializable]
public class EffectTypeAndIcon
{
    public EffectType effectType;
    public Sprite effectIcon;
}
[System.Serializable]
public class EffectSlot
{
    public GameObject slotGameObject;
    public EffectType effectType;
}
public class PlayerEffectsUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static PlayerEffectsUI Instance;
    PlayerMovement playerInstance;
    [SerializeField]
    private List<EffectSlot> effectSlots = new List<EffectSlot>();


    
    [SerializeField]
    public List <EffectTypeAndIcon> effectTypeAndIcons = new List<EffectTypeAndIcon>();
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        playerInstance = PlayerMovement.playerMovementInstance;
    }

    // Update is called once per frame
    void Update()
    {   
    }
    public void ActiveEffect(EffectType effectType)
    {
            EffectSlot slot = returnFirstFreeSlot();
            if (slot != null)
            {
                slot.slotGameObject.SetActive(true);
                
                slot.slotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = returnEffectIcon(effectType);
                slot.effectType = effectType;
            }
    }
    public void DeactiveEffect(EffectType effectType)
    {
        foreach (var slot in effectSlots)
        {
            if (slot.effectType == effectType && slot.slotGameObject.activeSelf)
            {
                slot.slotGameObject.SetActive(false);
                slot.slotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
                slot.effectType = EffectType.None;
                break;
            }
        }
    }
    public Sprite returnEffectIcon(EffectType effectType)
    {
        foreach (var effectTypeAndIcon in effectTypeAndIcons)
        {
            if (effectTypeAndIcon.effectType == effectType)
            {
                return effectTypeAndIcon.effectIcon;
            }
        }
        return null;
    }
    public EffectSlot returnFirstFreeSlot()
        {
            foreach (var slot in effectSlots)
            {
                if (!slot.slotGameObject.activeSelf)
                {
                    return slot;
                }
            }
            return null;
        }
    }
