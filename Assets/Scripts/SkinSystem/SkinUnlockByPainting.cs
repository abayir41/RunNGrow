using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinUnlockByPainting : MonoBehaviour, ISkin
{
    private static readonly Color DefaultColor = Color.white;
    
    public string SkinName => skinName;
    [SerializeField] private string skinName;
    [SerializeField] private GameObject parentGameObjectOfMeshRenderers;
    [SerializeField] private Color skinColor;
    private List<MeshRenderer> _meshRenderers;
    
    private void Awake()
    {
        _meshRenderers = parentGameObjectOfMeshRenderers.GetComponentsInChildren<MeshRenderer>().ToList();
        
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
        var matBlock = new MaterialPropertyBlock();
        matBlock.SetColor("_Color", skinColor);
        _meshRenderers.ForEach(meshRenderer => meshRenderer.SetPropertyBlock(matBlock));
    }

    public void DisableSkin()
    {
        PlayerPrefs.SetInt(skinName,0);
        var matBlock = new MaterialPropertyBlock();
        matBlock.SetColor("_Color", DefaultColor);
        _meshRenderers.ForEach(meshRenderer => meshRenderer.SetPropertyBlock(matBlock));
    }
    
    public bool IsSkinUnlocked()
    {
        return ScoreSystem.TotalPoints > SkinSystem.Instance.Skins[skinName];
    }
}
