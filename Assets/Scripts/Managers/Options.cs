using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Options : MonoBehaviour
{
 public enum QualityLevel { Low, Medium, High }

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