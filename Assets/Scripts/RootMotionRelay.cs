using UnityEngine;

public class RootMotionRelay : MonoBehaviour
{
    public Transform playerRoot; // Przeciągnij tu obiekt Player

    void OnAnimatorMove()
    {
        if (playerRoot != null)
        {
            playerRoot.position += GetComponent<Animator>().deltaPosition;
            playerRoot.rotation = GetComponent<Animator>().rootRotation;
        }
    }
}