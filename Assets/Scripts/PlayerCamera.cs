using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    private Camera cam;
    private Vector3 initialOffset;
    private Quaternion initialRotation;

    void Awake()
    {
        transform.position += transform.forward * 2f;

        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = true;
        }
        if (target != null)
        {
            initialOffset = transform.position - target.position;
        }
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + initialOffset;
        transform.rotation = initialRotation;
    }
}
