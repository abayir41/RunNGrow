using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTrigger : MonoBehaviour
{
    [SerializeField]
    private NormalObstacleObject obstacleObject;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CharController>() == null) return;
        
        if (other.GetComponent<CharController>().IsCharacterGhostMode) return;

        GameActions.NormalObstacleColl?.Invoke(obstacleObject, other.GetComponent<CharController>());
    }
}
