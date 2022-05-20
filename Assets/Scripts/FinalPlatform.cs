using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
