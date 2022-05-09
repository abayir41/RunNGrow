using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public static ObstacleController Instance { get; private set; }
    
    private GameConfig Config => GameController.Config;
    private GameController Controller => GameController.Instance;

    
    [SerializeField] private GameObject obstacle;
    [SerializeField] private Transform rightFirstObstacle;
    [SerializeField] private Transform leftFirstObstacle;
    [SerializeField] private Transform obstacleParent;

    private List<GameObject> Obstacles => _leftObstacles.Concat(_rightObstacles).ToList();
    private List<NormalObstacleObject> ObstacleObjects => _leftObstacleObjects.Concat(_rightObstacleObjects).ToList();
    
    private List<GameObject> _leftObstacles;
    private List<NormalObstacleObject> _leftObstacleObjects;
    
    private List<GameObject> _rightObstacles;
    private List<NormalObstacleObject> _rightObstacleObjects;
    
    
    [ShowInInspector]
    public Vector3 LastObstaclePos { get; private set; }


    private void Awake()
    {
        Instance = this;

        _leftObstacles = new List<GameObject>();
        _leftObstacleObjects = new List<NormalObstacleObject>();

        _rightObstacles = new List<GameObject>();
        _rightObstacleObjects = new List<NormalObstacleObject>();
    }


    public void SpawnMapObstacles(MapConfig config)
    {
        var left = leftFirstObstacle.position;
        var right = rightFirstObstacle.position;

        var along = Controller.MapAlongAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };

        var lastLeftObstaclePos = left;
        
        foreach (var leftSideObstacle in config.leftSideObstacles)
        {
            lastLeftObstaclePos += along * Config.DistanceBetweenObstacle;
            
            if(leftSideObstacle.obstacleType == NormalObstacleType.Nothing) continue;
            
            var obs = Instantiate(obstacle, obstacleParent);
            obs.transform.position = lastLeftObstaclePos;
            
            _leftObstacles.Add(obs);

            var obsScript = obs.GetComponent<NormalObstacleObject>();
            obsScript.SetProperties(leftSideObstacle);
            
            _leftObstacleObjects.Add(obsScript);
        }

        var lastRightObstaclePos = right;
        
        foreach (var rightSideObstacle in config.rightSideObstacles)
        {
            lastRightObstaclePos += along * Config.DistanceBetweenObstacle;
            
            if(rightSideObstacle.obstacleType == NormalObstacleType.Nothing) continue;
            
            var obs = Instantiate(obstacle, obstacleParent);
            obs.transform.position = lastRightObstaclePos;
            
            _rightObstacles.Add(obs);

            var obsScript = obs.GetComponent<NormalObstacleObject>();
            obsScript.SetProperties(rightSideObstacle);
            
            _rightObstacleObjects.Add(obsScript);
            
        }

        LastObstaclePos = Vector3.Max(lastLeftObstaclePos, lastRightObstaclePos);
    }
}
