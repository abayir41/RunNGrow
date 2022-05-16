using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SkinSystem : MonoBehaviour
{
    public static SkinSystem Instance { get; private set; }
    
    public List<string> KeysOfSkins => keysOfSkins;
    [HorizontalGroup("Base")] [BoxGroup("Base/Left")]
    [SerializeField] private List<string> keysOfSkins;
    public List<int> PriceOfSkins => priceOfSkins;
    [BoxGroup("Base/Mid")]
    [SerializeField] private List<int> priceOfSkins;

    public List<Sprite> SkinImages => skinImages;
    [BoxGroup("Base/Right")]
    [SerializeField] private List<Sprite> skinImages;
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

    public bool IsSkinEnabled(string skinName)
    {
        return PlayerPrefs.GetInt(skinName) == 1;
    }

    public void EnableSkin(string skinName)
    {
        PlayerPrefs.SetInt(skinName, 1);
    }

    public void DisableSkin(string skinName)
    {
        PlayerPrefs.SetInt(skinName, 0);
    }

    public bool IsThereAnyNewSkin()
    {
        if (ScoreSystem.TotalPoints > priceOfSkins.Last())
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool IsSkinUnlocked(string skinName)
    {
        return ScoreSystem.TotalPoints > priceOfSkins[keysOfSkins.IndexOf(skinName)];
    }
    
    
   

    public Sprite CurrentSkinImage()
    {
        for (int i = 0; i < skinImages.Count; i++)
            if (ScoreSystem.TotalPoints < priceOfSkins[i])
                return skinImages[i];

        return null;
    }

    public int CurrentSkinIndex()
    {
        for (int i = 0; i < priceOfSkins.Count; i++)
        {
            if (ScoreSystem.TotalPoints < priceOfSkins[i]) return i;
        }

        return -1;
    }
}
