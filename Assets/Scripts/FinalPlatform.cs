using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

public class FinalPlatform : MonoBehaviour
{
    public float multiplier;
    public bool gotHit;
    public GameObject racket;
    public Transform pos;
    public Transform SelfPos => transform;
    private Vector3 _cachedRacketDiff;
    public List<Renderer> colorfulItems;

    public Color colorfulItemColor;

   

    private void Awake()
    {
        _cachedRacketDiff = racket.transform.position - transform.position;
    }

    private void OnEnable()
    {
        IntermediateObjectActions.IntermediateObjectFinalPlatformArrivedSuccessfully += IntermediateObjectFinalPlatformArrivedSuccessfully;
    }

    private void IntermediateObjectFinalPlatformArrivedSuccessfully(Vector2 arg1, GameObject arg2, FinalPlatform arg3)
    {
        if(arg3 != this) return;

        gotHit = true;

        colorfulItems.ForEach(renderer1 =>
        {
            var trans = renderer1.gameObject.transform;

            //trans.DOScale(trans.localScale * 2f, 0.2f).SetLoops(2, LoopType.Yoyo);

            var temp = colorfulItemColor;

            DOTween.To(() => temp, value =>
            {
                temp = value;
                var matBlock = new MaterialPropertyBlock();
                matBlock.SetColor("_BaseColor", value);
                renderer1.SetPropertyBlock(matBlock);
            }, Color.white, GameController.Config.GoingWhiteAnimDuration).OnComplete(() =>
            {
                DOTween.To(() => temp, value =>
                {
                    temp = value;
                    var matBlock = new MaterialPropertyBlock();
                    matBlock.SetColor("_BaseColor", value);
                    renderer1.SetPropertyBlock(matBlock);
                }, Color.green, GameController.Config.GoingGreenAnimDuration);
            });
        });
      
        racket.transform.parent = null;
        racket.transform.DORotate(new Vector3(-90, -90, 0), GameController.Config.RacketAnimation).SetEase(Ease.OutBounce);

    }

    private void Update()
    {
        if (gotHit)
        {
            racket.transform.position = transform.position + _cachedRacketDiff;
        }
    }

    private void OnDisable()
    {
        IntermediateObjectActions.IntermediateObjectFinalPlatformArrivedSuccessfully -=
            IntermediateObjectFinalPlatformArrivedSuccessfully;
    }
}
