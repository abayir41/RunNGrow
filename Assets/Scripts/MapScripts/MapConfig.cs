using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "MapConfig")]
public class MapConfig : ScriptableObject
{

    [HorizontalGroup("Base")] [BoxGroup("Base/Left")]
    public List<ObstacleStruct> leftSideObstacles;

    
    [BoxGroup("Base/Right")]
    public List<ObstacleStruct> rightSideObstacles;
    
}
