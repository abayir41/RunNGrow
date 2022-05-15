using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkinSystem : MonoBehaviour
{
    public static SkinSystem Instance { get; private set; }
    
    public List<string> KeysOfSkins => keysOfSkins;
    [HorizontalGroup("Base")] [BoxGroup("Base/Left")]
    [SerializeField] private List<string> keysOfSkins;
    public List<int> PriceOfSkins => priceOfSkins;
    [BoxGroup("Base/Right")]
    [SerializeField] private List<int> priceOfSkins;
    
    public Dictionary<string, int> Skins { get; private set; }

    private void Awake()
    {
        Instance = this;

        Skins = new Dictionary<string, int>();
        for (var i = 0; i < KeysOfSkins.Count; i++)
        {
            Skins.Add(KeysOfSkins[i], PriceOfSkins[i]);
        }
    }

    public static bool IsSkinEnabled(string skinName)
    {
        return PlayerPrefs.GetInt(skinName) == 1;
    }
}
