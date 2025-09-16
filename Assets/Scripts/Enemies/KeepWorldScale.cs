using UnityEngine;

[ExecuteAlways] // działa też w edytorze
public class KeepWorldScale : MonoBehaviour
{
    public Vector3 targetWorldScale = Vector3.one;

    void LateUpdate()
    {
        if (transform.parent == null)
            return;

        Vector3 parentScale = transform.parent.lossyScale;

        // unikamy dzielenia przez zero
        if (Mathf.Approximately(parentScale.x, 0f)) parentScale.x = 1e-6f;
        if (Mathf.Approximately(parentScale.y, 0f)) parentScale.y = 1e-6f;
        if (Mathf.Approximately(parentScale.z, 0f)) parentScale.z = 1e-6f;

        transform.localScale = new Vector3(
            targetWorldScale.x / parentScale.x,
            targetWorldScale.y / parentScale.y,
            targetWorldScale.z / parentScale.z
        );
    }
}
