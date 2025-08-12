using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform parent;
    public Vector3 offset;
     void Start()
    {
        Camera cam = GetComponent<Camera>();
        cam.aspect = 1f; // wymusza kwadrat
    }

    void LateUpdate()
    {
        transform.position = parent.position + offset;
    }
}
