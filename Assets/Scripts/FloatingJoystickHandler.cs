using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform handle; // Przypisz drążek joysticka w Inspectorze
    public float handleRange = 50f; // Maksymalny zasięg drążka

    public void OnPointerDown(PointerEventData eventData)
    {
        // Po kliknięciu drążek wraca na środek
        handle.anchoredPosition = Vector2.zero;
        OnDrag(eventData); // Od razu obsłuż drag, by joystick zareagował natychmiast
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );
        Vector2 offset = Vector2.ClampMagnitude(localPoint, handleRange);
        handle.anchoredPosition = offset;
        // Tutaj możesz dodać logikę sterowania postacią na podstawie offset.normalized
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
    }
}
