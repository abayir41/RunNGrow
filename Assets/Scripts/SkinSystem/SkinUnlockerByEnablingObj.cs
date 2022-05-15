using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinUnlockerByEnablingObj : MonoBehaviour, ISkin
{
    public string SkinName => skinName;
    [SerializeField] private string skinName;

    [SerializeField] private GameObject skin;
    
    private void Awake()
    {
        if (SkinSystem.IsSkinEnabled(skinName))
        {
            EnableSkin();
        }
        else
        {
            DisableSkin();
        }
    }

    public void EnableSkin()
    {
        PlayerPrefs.SetInt(skinName,1);
        skin.SetActive(true);
    }

    public void DisableSkin()
    {
        PlayerPrefs.SetInt(skinName,0);
        skin.SetActive(false);
    }
    
    public bool IsSkinUnlocked()
    {
        return ScoreSystem.TotalPoints > SkinSystem.Instance.Skins[skinName];
    }
}
