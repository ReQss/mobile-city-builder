using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[System.Serializable]
public enum LightLevel
{
    Light,
    Normal,
    Dark
}

[System.Serializable]
public class ColorAdjustmentsPreset
{
    public float postExposure=0.5f;
    public float contrast = 10f;
}

public class Options : MonoBehaviour
{
    public ColorAdjustmentsPreset lightPreset;
    public ColorAdjustmentsPreset normalPreset;
    public ColorAdjustmentsPreset darkPreset;


    public enum QualityLevel { Low, Medium, High }
    public LightLevel lightLevel;

    
    public void SetQuality(string qualityName)
    {
        QualityLevel quality;
        if (System.Enum.TryParse(qualityName, true, out quality))
        {
            ApplyQualitySettings(quality);
        }
        else
        {
            Debug.LogWarning("Nieznany poziom jakości: " + qualityName);
        }
    }

    public void SetLight(int level)
    {
        lightLevel = (LightLevel)level;
        SetLightLevel((LightLevel)level);
    }

    private void ApplyQualitySettings(QualityLevel quality)
    {
        switch (quality)
        {
            case QualityLevel.Low:
                QualitySettings.SetQualityLevel(1); // "Low"
                SetRenderScale(0.5f);
                break;

            case QualityLevel.Medium:
                QualitySettings.SetQualityLevel(2); // "Medium"
                SetRenderScale(0.75f);
                break;

            case QualityLevel.High:
                QualitySettings.SetQualityLevel(3); // "High"
                SetRenderScale(1.0f);
                break;
        }

        Debug.Log("Ustawiono jakość: " + quality.ToString());
    }

    public void SetLightLevel(LightLevel level)
    {
       GameManager.Instance.lightLevel = level;
        switch (level)
        {
            case LightLevel.Light:
                GameManager.Instance.lightPreset = lightPreset;
                break;
            case LightLevel.Normal:
                GameManager.Instance.lightPreset = normalPreset;
                break;
            case LightLevel.Dark:
                GameManager.Instance.lightPreset = darkPreset;
                break;
        }
    }

    private void SetRenderScale(float scale)
    {
#if USING_URP
        var urpAsset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
        if (urpAsset != null)
        {
            urpAsset.renderScale = scale;
        }
        else
        {
            Debug.LogWarning("URP nie jest aktywny – renderScale nie zostanie zmieniony.");
        }
#else
        Debug.Log("RenderScale nieobsługiwany bez URP.");
#endif
    }
}