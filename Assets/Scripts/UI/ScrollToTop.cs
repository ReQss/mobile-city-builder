using UnityEngine;
using UnityEngine.UI;

public class ScrollToTop : MonoBehaviour
{
    public ScrollRect scrollRect;

    void Start()
    {
        // Ustaw scroll na górę
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void OnContentChanged()
    {
        // Jeśli dynamicznie dodajesz elementy
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
