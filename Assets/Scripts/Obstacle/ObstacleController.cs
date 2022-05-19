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

    public bool _gameStarted;
    
    [SerializeField] private GameObject obstacle;
    [SerializeField] private Transform rightFirstObstacle;
    [SerializeField] private Transform leftFirstObstacle;
    [SerializeField] private Transform obstacleParent;

    [HideInInspector]
    public List<GameObject> obstacles;
    public List<Transform> obstaclesTransforms;
    private List<NormalObstacleObject> _obstacleObjects;
    
    private List<GameObject> _leftObstacles;
    private List<NormalObstacleObject> _leftObstacleObjects;
    
    private List<GameObject> _rightObstacles;
    private List<NormalObstacleObject> _rightObstacleObjects;
    
    
    public Transform LastObstacleTransform { get; private set; }
    public Vector3 LastObstaclePos { get; private set; }


    private void Awake()
    {
        Instance = this;

        _leftObstacles = new List<GameObject>();
        _leftObstacleObjects = new List<NormalObstacleObject>();

        _rightObstacles = new List<GameObject>();
        _rightObstacleObjects = new List<NormalObstacleObject>();

        var obsToMoveL = GameObject.FindGameObjectsWithTag("LeftObs");
        var obsToMoveR = GameObject.FindGameObjectsWithTag("RightObs");
        for (int i = 0; i < obsToMoveL.Length; i++)
        {
            _leftObstacles.Add(obsToMoveL[i]);
        }
        for (int i = 0; i < obsToMoveR.Length; i++)
        {
            _rightObstacles.Add(obsToMoveR[i]);
        }
    }

    private void OnEnable()
    {
        GameActions.GameStarted += GameStarted;
    }

    

    private void OnDisable()
    {
        GameActions.GameStarted -= GameStarted;
    }

    private void GameStarted()
    {
        _gameStarted = true;
    }
    
    private void Update()
    {
        if(!_gameStarted) return;

        var along = Controller.MapAlongAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };
        
        obstaclesTransforms.ForEach(trans => trans.position -= along * (Config.SpeedOfObstacles * Time.deltaTime));
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

            LastObstacleTransform = obs.transform;

        }

        LastObstaclePos = Vector3.Max(lastLeftObstaclePos, lastRightObstaclePos);
        
        obstacles = _leftObstacles.Concat(_rightObstacles).ToList();
        obstaclesTransforms = obstacles.Select(o => o.GetComponent<Transform>()).ToList();
        _obstacleObjects = _leftObstacleObjects.Concat(_rightObstacleObjects).ToList();
    }
}
