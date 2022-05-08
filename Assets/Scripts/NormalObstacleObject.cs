using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalObstacleObject : MonoBehaviour
{
    public NormalObstacleType Type => type;
    [SerializeField] private NormalObstacleType type;

    public int ObstaclePoint => obstaclePoint;
    [SerializeField] private int obstaclePoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharController>().IsCharacterGhostMode) return;

        GameActions.NormalObstacleColl?.Invoke(this, other.GetComponent<CharController>());
    }
}   
