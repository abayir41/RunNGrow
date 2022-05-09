using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class NormalObstacleObject : MonoBehaviour
{
    private GameConfig Config => GameController.Config;
    public NormalObstacleType Type => _obstacleProperties.obstacleType;

    public int ObstaclePoint => _obstacleProperties.obstaclePoint;

    [ShowInInspector]
    private ObstacleStruct _obstacleProperties;

    [SerializeField] private MeshRenderer middlePanel;
    

    public void SetProperties(ObstacleStruct prop)
    {
        _obstacleProperties = prop;

        switch (_obstacleProperties.obstacleType)
        {
            case NormalObstacleType.AddHeight:
                middlePanel.material = Config.GreenColor;
                break;
            
            case NormalObstacleType.AddWeight:
                middlePanel.material = Config.GreenColor;
                break;
            
            case NormalObstacleType.DivideHeight:
                middlePanel.material = Config.RedColor;
                break;
            
            case NormalObstacleType.DivideWeight:
                middlePanel.material = Config.RedColor;
                break;
            
            case NormalObstacleType.ExtractHeight:
                middlePanel.material = Config.RedColor;
                break;
            
            case NormalObstacleType.ExtractWeight:
                middlePanel.material = Config.RedColor;
                break;
            
            case NormalObstacleType.MultiplyHeight:
                middlePanel.material = Config.GreenColor;
                break;
            
            case NormalObstacleType.MultiplyWeight:
                middlePanel.material = Config.GreenColor;
                break;
            
            case NormalObstacleType.Destroyer:
                middlePanel.material = Config.RedColor;
                break;
            
        }
    }
}   
