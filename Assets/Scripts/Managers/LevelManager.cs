using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField]
    private List<Sprite> canvasBackgrounds;
    [SerializeField]
    private GameObject loaderCanvas;
    [SerializeField]
    private Image progressBar;
    private float target;
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public Sprite RandomSpriteGenerate()
    {
        if (canvasBackgrounds != null && canvasBackgrounds.Count > 0)
        {
            int randomIndex = Random.Range(0, canvasBackgrounds.Count);
            return canvasBackgrounds[randomIndex];
        }
        return null;
    }
    public async void LoadScene(string sceneName)
    {
        if (sceneName == "Maps" || sceneName == "Menu")
        {
            LoadSceneNoLoader(sceneName);
            return;
        }
        target = 0f;
        progressBar.fillAmount = 0f;
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        Sprite randomSprite = RandomSpriteGenerate();
        if(randomSprite!=null)
            loaderCanvas.GetComponent<Image>().sprite = randomSprite;
        loaderCanvas.SetActive(true);
        do
        {
            await Task.Delay(100);
            target = scene.progress;
        } while (scene.progress < 0.9f);
        await Task.Delay(1000);
        scene.allowSceneActivation = true;

        await Task.Yield();
        loaderCanvas.SetActive(false);
    }
    public async void LoadSceneNoLoader(string sceneName)
    { 
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    void Update()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, target, 3 * Time.deltaTime);
    }
}
