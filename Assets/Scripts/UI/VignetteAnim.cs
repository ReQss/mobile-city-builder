using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteAnim : MonoBehaviour
{
    [SerializeField] private Volume volume;       // Referencja do Volume z efektem Vignette
    [SerializeField] private float minIntensity = 0.2f; // Minimalna intensywność
    [SerializeField] private float maxIntensity = 0.5f; // Maksymalna intensywność
    [SerializeField] private float speed = 1.0f;        // Szybkość pulsowania

    private Vignette vignette;
    private float time;
    public bool isEnabled = false;

    void Start()
    {
        if (volume == null)
        {
            volume = GetComponent<Volume>();
        }

        if (volume.profile.TryGet(out vignette) == false)
        {
            Debug.LogError("Brak efektu Vignette w Volume!");
        }
    }
    public void HandleState()
    {
        if(PlayerMovement.playerMovementInstance == null) return;
        if (PlayerMovement.playerMovementInstance.health < 30)
        {
            minIntensity = 0.2f;
            maxIntensity = 0.5f;
            speed = 7f;
        }
        else if(PlayerMovement.playerMovementInstance.health < 50)
        {
            minIntensity = 0.0f;
            maxIntensity = 0.3f;
            speed = 5f;
        }
        else
        {
            minIntensity = 0;
            maxIntensity = 0;
            speed = 0f;
        }
    }

    void Update()
    {
        HandleState();
        if (isEnabled == false) { 
            vignette.intensity.value = 0f;
            return; }
        if (vignette == null)
            return;

        time += Time.deltaTime * speed;

        // Używamy sinusoidy do płynnego przejścia między min a max
        float t = (Mathf.Sin(time) + 1f) / 2f; 
        vignette.intensity.value = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}
