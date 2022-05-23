using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapKeeper : MonoBehaviour
{
    public static MapKeeper Instance { get; private set; }

    [SerializeField] private MapConfig map;
    [SerializeField] private int obstacleAmount;

    [HorizontalGroup("%60")] 
    
    [BoxGroup("%60/%60Left1")]
    public List<NormalObstacleType> leftObstacleTypes;
    [BoxGroup("%60/%60Left2")]
    public List<int> leftObstaclePoints;
    
    [BoxGroup("%60/%60Right1")]
    public List<NormalObstacleType> rightObstacleTypes;
    [BoxGroup("%60/%60Right2")]
    public List<int> rightObstaclePoints;
    
    
    [HorizontalGroup("%25")] 
    
    [BoxGroup("%25/%25Left1")]
    public List<NormalObstacleType> leftObstacleTypes2;
    [BoxGroup("%25/%25Left2")]
    public List<int> leftObstaclePoints2;
    
    [BoxGroup("%25/%25Right1")]
    public List<NormalObstacleType> rightObstacleTypes2;
    [BoxGroup("%25/%25Right2")]
    public List<int> rightObstaclePoints2;
    
    
    [HorizontalGroup("%15")] 
    
    [BoxGroup("%15/%15Left1")]
    public List<NormalObstacleType> leftObstacleTypes3;
    [BoxGroup("%15/%15Left2")]
    public List<int> leftObstaclePoints3;
    
    [BoxGroup("%15/%15Right1")]
    public List<NormalObstacleType> rightObstacleTypes3;
    [BoxGroup("%15/%15Right2")]
    public List<int> rightObstaclePoints3;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public MapConfig GetMap()
    {
        var mapConfig = map;

        mapConfig.leftObstacleTypes = new List<NormalObstacleType>();
        mapConfig.leftObstaclePoint = new List<int>();
        mapConfig.rightObstacleTypes = new List<NormalObstacleType>();
        mapConfig.rightObstaclePoint = new List<int>();
        
        for (var i = 0; i < obstacleAmount; i++)
        {
            var randFloat = Random.Range(0, 100);

            NormalObstacleType leftObstacle;
            int leftObstaclePoint;
            
            NormalObstacleType rightObstacle;
            int rightObstaclePoint;
            
            if (randFloat >= 0 && randFloat <= 60)
            {
                var randInt = Random.Range(0, leftObstacleTypes.Count);
                
                leftObstacle = leftObstacleTypes[randInt];
                leftObstaclePoint = leftObstaclePoints[randInt];

                rightObstacle = rightObstacleTypes[randInt];
                rightObstaclePoint = rightObstaclePoints[randInt];
                
            }
            else if (randFloat > 60 && randFloat <= 80)
            {
                var randInt = Random.Range(0, leftObstacleTypes2.Count);
                
                leftObstacle = leftObstacleTypes2[randInt];
                leftObstaclePoint = leftObstaclePoints2[randInt];

                rightObstacle = rightObstacleTypes2[randInt];
                rightObstaclePoint = rightObstaclePoints2[randInt];
            }
            else
            {
                var randInt = Random.Range(0, leftObstacleTypes3.Count);
                
                leftObstacle = leftObstacleTypes3[randInt];
                leftObstaclePoint = leftObstaclePoints3[randInt];

                rightObstacle = rightObstacleTypes3[randInt];
                rightObstaclePoint = rightObstaclePoints3[randInt];
            }
            
            mapConfig.leftObstacleTypes.Add(leftObstacle);
            mapConfig.leftObstaclePoint.Add(leftObstaclePoint);
            mapConfig.rightObstacleTypes.Add(rightObstacle);
            mapConfig.rightObstaclePoint.Add(rightObstaclePoint);
        }

        return mapConfig;
    }
}
