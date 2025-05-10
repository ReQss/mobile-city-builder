using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target; // Assign your player transform in the inspector

    private Camera cam;
    private Vector3 initialOffset;
    private Quaternion initialRotation;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = true; // Ensure camera is orthographic
        }
        // Store the initial offset and rotation from the inspector
        if (target != null)
        {
            initialOffset = transform.position - target.position;
        }
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Keep the camera at the same offset and rotation as set in the inspector
        transform.position = target.position + initialOffset;
        transform.rotation = initialRotation;
    }
}
