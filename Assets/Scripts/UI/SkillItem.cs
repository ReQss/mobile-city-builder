using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    public Image fillAmountImageVertical;
    public float startTime;
    public float duration = 2f;
    public bool isActive = true;

    void Start()
    {
        fillAmountImageVertical.fillAmount = 1f;
    }

    void Update()
    {
        if (!isActive)
        {
            float timePassed = Time.time - startTime;
            float progress = timePassed / duration;

            fillAmountImageVertical.fillAmount = progress;

            if (progress >= 1f)
            {
                fillAmountImageVertical.fillAmount = 1f;
                isActive = true;
            }
        }
    }

    public void UseSkill()
    {
        if (!isActive) return; // opcjonalnie blokada spamu

        isActive = false;
        startTime = Time.time;
        fillAmountImageVertical.fillAmount = 0f;
    }
}
