using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinUnlockByPainting : MonoBehaviour, ISkin
{
    private static readonly Color DefaultColor = Color.white;
    
    [SerializeField] private List<GameObject> parentGameObjectOfMeshRenderers;
    [SerializeField] private Color skinColor;
    private List<MeshRenderer> _meshRenderers;
    
    private void Awake()
    {
        _meshRenderers = new List<MeshRenderer>();
        
        parentGameObjectOfMeshRenderers.ForEach(obj =>
        {
            _meshRenderers.AddRange(obj.GetComponentsInChildren<MeshRenderer>().ToList());
        });
    }

    public void EnableSkin()
    {
        var matBlock = new MaterialPropertyBlock();
        matBlock.SetColor("_Color", skinColor);
        _meshRenderers.ForEach(meshRenderer => meshRenderer.SetPropertyBlock(matBlock));
    }

    public void DisableSkin()
    {
        var matBlock = new MaterialPropertyBlock();
        matBlock.SetColor("_Color", DefaultColor);
        _meshRenderers.ForEach(meshRenderer => meshRenderer.SetPropertyBlock(matBlock));
    }
    
}
