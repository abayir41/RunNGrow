using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    // Start is called before the first frame update
    public static Side GetOtherSide(Side side)
    {
        return side == Side.Right ? Side.Left : Side.Right;
    }

    public static Vector2 NormalObstacleToPoint(NormalObstacleType obstacleType, Vector2 input, int amount)
    {
        switch (obstacleType)
        {
            case NormalObstacleType.AddHeight:
                return input + Vector2.up * amount;
            
            case NormalObstacleType.AddWeight:
                return input + Vector2.right * amount;
            
            case NormalObstacleType.DivideHeight:
                var resultDy = (int) (input.y / amount);
                return new Vector2(input.x, resultDy);
            
            case NormalObstacleType.DivideWeight:
                var resultDx = (int) (input.x / amount);
                return new Vector2(resultDx, input.y);
            
            case NormalObstacleType.ExtractHeight:
                return input - Vector2.up * amount;
            
            case NormalObstacleType.ExtractWeight:
                return input - Vector2.right * amount;
            
            case NormalObstacleType.MultiplyHeight:
                var resultMy = (int) (input.y * amount);
                return new Vector2(input.x, resultMy);
            
            case NormalObstacleType.MultiplyWeight:
                var resultMx = (int) (input.x * amount);
                return new Vector2(resultMx, input.y);
            
            case NormalObstacleType.Destroyer:
                return new Vector2(-1,-1);
            
            default:
                return new Vector2(0, 0);
        }
    }
}
