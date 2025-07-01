using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystickHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform handle; // Rączka joysticka
    public float handleRange = 50f; // Maksymalny zasięg rączki
    public RectTransform joystickPanel; // Cały joystick (ControlsMovement)
    public RectTransform backgroundPanel; // MovementRangeBackground

    public void OnPointerDown(PointerEventData eventData)
    {
        // Ustaw joystick w miejscu kliknięcia na tle
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            backgroundPanel,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );
        joystickPanel.anchoredPosition = localPoint;
        handle.anchoredPosition = Vector2.zero;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickPanel,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );
        Vector2 offset = Vector2.ClampMagnitude(localPoint, handleRange);
        handle.anchoredPosition = offset;
        // offset.normalized - kierunek ruchu
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
    }
}
