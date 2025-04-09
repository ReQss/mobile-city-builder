using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

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
        // Sprawdza, czy scena jest wbudowana w projekt
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
        gameObject.SetActive(false);
    }
    public void OpenUIObject(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

}
