using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinOnMenu : MonoBehaviour
{

    [SerializeField] private string nameOfSkin;
    private ISkin _skinEnabler;
    [SerializeField]private GameObject skinSelectedTick;
    [SerializeField] private GameObject parentObj;
    
    

    private void Start()
    {
        var skinsys = SkinSystem.Instance;
        _skinEnabler = GetComponent<ISkin>();

        if (!skinsys.IsSkinUnlocked(nameOfSkin))
        {
            parentObj.SetActive(false);
        };

        if (skinsys.IsSkinEnabled(nameOfSkin))
        {
            skinSelectedTick.SetActive(true);
            _skinEnabler.EnableSkin();
        }
    }

    public void Clicked()
    {
        if (SkinSystem.Instance.IsSkinEnabled(nameOfSkin))
        {
            SkinSystem.Instance.DisableSkin(nameOfSkin);
            skinSelectedTick.SetActive(false);
            _skinEnabler.DisableSkin();
        }
        else
        {
            SkinSystem.Instance.EnableSkin(nameOfSkin);
            skinSelectedTick.SetActive(true);
            _skinEnabler.EnableSkin();
        }
    }
}
