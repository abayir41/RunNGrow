using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActions : MonoBehaviour
{
    public static Action<NormalObstacleObject, CharController> NormalObstacleColl;

    public static Action GameFinishAnimationStarted;

    public static Action BossRunningStarted;

    public static Action GameEndedWithWinning;
    
    public static Action GameFailed;

    
}
