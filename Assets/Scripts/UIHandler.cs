using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> uiBuildingObjects;
    [SerializeField]
    private TextMeshProUGUI coinCounter;
    [SerializeField]
    private GameObject coinBubble;
    void Start()
    {
        SetMoney(GameManager.Instance.playerCoinCount);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TestDebug()
    {
        Debug.Log("Button is worting");
    }
    public void LoadSceneByName(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("Ładowanie sceny: " + sceneName);
        }
        else
        {
            Debug.LogError("Scena " + sceneName + " nie istnieje.");
        }
    }
    public void CloseUIObject(GameObject gameObject)
    {
        // gameObject.SetActive(false);
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("IsOpen", false);
    }
    public void CloseListOfUIObjects()
    {
        foreach (GameObject go in uiBuildingObjects)
        {
            go.SetActive(false);
        }
    }
    public void CloseListOfUIObjectsWithAnimation()
    {
        foreach (GameObject go in uiBuildingObjects)
        {
            CloseUIObject(go);
        }
    }
    public void OpenUIObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
        Animator animator = gameObject.GetComponent<Animator>();
        animator.SetBool("IsOpen", true);
    }
    public void CollectMoney()
    {
        int collectedMoney = GameManager.Instance.temporaryCoinsToCollect;
        GameManager.Instance.increaseCoins(collectedMoney);

        if (coinCounter != null)
            coinCounter.text = GameManager.Instance.playerCoinCount.ToString();
        if (coinBubble != null)
        {
            CloseUIObject(coinBubble);
            StartCoroutine(openAlertAfterTime());
        }
        else Debug.Log("ui not signed");

    }
    private IEnumerator openAlertAfterTime()
    {
        yield return new WaitForSeconds(5f);
        OpenUIObject(coinBubble);
    }
    public void SetMoney(int amount)
    {
        if (coinCounter != null)
            coinCounter.text = amount.ToString();
    }
}
