using TMPro;
using UnityEngine;

public class MenuStatisticsUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI levelText;
    [SerializeField]
    TextMeshProUGUI experienceText;
    [Header("Character Stats")]
    [SerializeField]
    TextMeshProUGUI healthText;
    [SerializeField]
    TextMeshProUGUI attackText;
    [SerializeField]
    TextMeshProUGUI speedText;
    [Header("Coin stats")]
    [SerializeField]
    TextMeshProUGUI coinsText;
    [SerializeField]
    TextMeshProUGUI diamondsText;

    void Start()
    {
        levelText.text = GameManager.Instance.playerLevel.ToString();
        experienceText.text = GameManager.Instance.playerCurrentExperience.ToString() + " / " + GameManager.Instance.playerExperienceToGetLevel.ToString();
        healthText.text = GameManager.Instance.playerHealth.ToString();
        attackText.text = GameManager.Instance.playerAttack.ToString();
        speedText.text = GameManager.Instance.playerSpeed.ToString();
        coinsText.text = GameManager.Instance.playerCoinCount.ToString();
        diamondsText.text = "0";

    }
}
