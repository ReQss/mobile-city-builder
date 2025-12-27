using UnityEngine;

public class FireCollisionDetector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isCollidingWithPlayer = false;
    public bool isBurning = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         isCollidingWithPlayer = true;
    //     }
    // }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
            if(isBurning == false){
                isBurning = true;
                PlayerEffectsUI.Instance.ActiveEffect(EffectType.Burn);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
            if(isBurning == true){
                isBurning = false;
                PlayerEffectsUI.Instance.DeactiveEffect(EffectType.Burn);
            }
        }
    }
}
