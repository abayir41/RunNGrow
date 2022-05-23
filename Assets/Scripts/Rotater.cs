using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Rotater : MonoBehaviour
{
    public enum AnimObstacle 
    {
        type0,
        type1,
        type3
    }

    public AnimObstacle type;
    
    public Transform baseRotation;
    
    private void Update()
    {
        switch (type)
        {
            case AnimObstacle.type0:
                baseRotation.Rotate(new Vector3(0,1,0) * (GameController.Config.Obs0Rot * Time.deltaTime));
                break;
            case AnimObstacle.type1:
                baseRotation.Rotate(new Vector3(0,0,1) * (GameController.Config.Obs1Rot * Time.deltaTime));
                break;
            case AnimObstacle.type3:
                baseRotation.Rotate(new Vector3(0,1,0) * (GameController.Config.Obs3Rot * Time.deltaTime));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

