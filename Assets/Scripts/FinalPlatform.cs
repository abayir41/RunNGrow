using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPlatform : MonoBehaviour
{
    public float multiplier;
    public bool gotHit;
    public GameObject leftObject;
    public GameObject rightObject;
    public Transform pos;
    public Transform SelfPos => transform;

    

    private void OnEnable()
    {
        IntermediateObjectActions.IntermediateObjectFinalPlatformArrivedSuccessfully+= IntermediateObjectFinalPlatformArrivedSuccessfully;
    }

    private void IntermediateObjectFinalPlatformArrivedSuccessfully(Vector2 arg1, GameObject arg2, FinalPlatform arg3)
    {
        gotHit = true;
        
        if(arg3 != this) return;

        if (leftObject.activeSelf)
        {
            leftObject.transform.parent = null;
            leftObject.GetComponent<BoxCollider>().enabled = true;
            leftObject.GetComponent<Rigidbody>().AddForceAtPosition(leftObject.transform.forward * (-1 * 1000), pos.position);
        }
        else
        {
            rightObject.transform.parent = null;
            leftObject.GetComponent<BoxCollider>().enabled = true;
            rightObject.GetComponent<Rigidbody>().AddForceAtPosition(rightObject.transform.forward * (-1 * 1000), pos.position);
        }
    }

    private void OnDisable()
    {
        IntermediateObjectActions.IntermediateObjectFinalPlatformArrivedSuccessfully -=
            IntermediateObjectFinalPlatformArrivedSuccessfully;
    }
}
