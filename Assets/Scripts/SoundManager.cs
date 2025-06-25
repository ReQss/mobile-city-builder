using UnityEngine;

public class SoundManager : MonoBehaviour
{
      public static SoundManager Instance { get; private set; }

      public AudioClip screamSound;
    public AudioClip ambientSound;
    public AudioClip battleSound;
    public AudioClip bossMusic;
    public AudioClip levelUpSound;
    private AudioSource audioSource;
      void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
   
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
      public void PlayLevelUp()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (levelUpSound != null && audioSource != null)
        {
            audioSource.Stop(); 
            audioSource.clip = levelUpSound;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
}
