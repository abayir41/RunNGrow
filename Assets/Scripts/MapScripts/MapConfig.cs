using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "MapConfig")]
public class MapConfig : ScriptableObject
{
    
    [HorizontalGroup("Base")] 
    
    [BoxGroup("Base/Left1")]
    public List<NormalObstacleType> leftObstacleTypes;
    [BoxGroup("Base/Left2")]
    public List<int> leftObstaclePoint;
    
    [BoxGroup("Base/Right1")]
    public List<NormalObstacleType> rightObstacleTypes;
    [BoxGroup("Base/Right2")]
    public List<int> rightObstaclePoint;

    public List<ObstacleStruct> leftSideObstacles
    {
        get
        {
            var list = new List<ObstacleStruct>();
            for (var i = 0; i < leftObstacleTypes.Count; i++)
            {
                var obsStruct = new ObstacleStruct
                {
                    obstaclePoint = leftObstaclePoint[i], obstacleType = leftObstacleTypes[i]
                };

                list.Add(obsStruct);
            }

            return list;
        }
    }

    public List<ObstacleStruct> rightSideObstacles
    {
        get
        {
            var list = new List<ObstacleStruct>();
            for (var i = 0; i < rightObstacleTypes.Count; i++)
            {
                var obsStruct = new ObstacleStruct
                {
                    obstaclePoint = rightObstaclePoint[i], obstacleType = rightObstacleTypes[i]
                };

                list.Add(obsStruct);
            }

            return list;
        }
    }
    
}
