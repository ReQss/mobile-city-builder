using UnityEngine;
enum WeaponType
{
    Sword,
    Bow,
    Crossbow,
    Rod
}
[System.Serializable]
public class PlayerWeapon : MonoBehaviour
{
    public GameObject currentWeapon;
    WeaponType weaponType;
    public bool knockbackEffect = true;
    public bool igniteEffect = false;
    public string currentWeaponName;
    string description;
    int damage;
    float attackSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
