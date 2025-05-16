using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject currentWeaponImage;
    [SerializeField]
    //how can i change it to sprite 2d and ui?
    public List<GameObject> weaponImages;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateWeaponImage(String weaponName)
    {
        foreach (GameObject weaponImage in weaponImages)
        {
            if (weaponImage.name == weaponName)
            {
                weaponImage.SetActive(true);
                break;
            }
            else if (currentWeaponImage != null)
            {
                currentWeaponImage.SetActive(false);
            }
        }

        if (currentWeaponImage != null)
        {
            Image uiImage = GetComponent<Image>();
            if (uiImage != null)
            {
                uiImage.sprite = currentWeaponImage.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
}
