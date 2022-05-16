using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinUnlockerByEnablingObj : MonoBehaviour, ISkin
{
    
    [SerializeField] private List<GameObject> skin;

    public void EnableSkin()
    {
        skin.ForEach(obj => obj.SetActive(true));
    }

    public void DisableSkin()
    {
        skin.ForEach(obj => obj.SetActive(false));
    }
    
}
