using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
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
            
            case NormalObstacleType.PartRemover:
                return input - Vector2.up * amount;
            
            default:
                return new Vector2(0, 0);
        }
    }

    public static Vector3 ScaleModeToVector(ScaleMode mode, float value)
    {
        switch (mode)
        {
            case ScaleMode.XYZ:
                return Vector3.one * value;
            
            case ScaleMode.XY:
                return new Vector3(value, value, 1);
            
            case ScaleMode.XZ:
                return new Vector3(value, 1, value);
            
            case ScaleMode.YZ:
                return new Vector3(1, value, value);
            
            case ScaleMode.X:
                return new Vector3(value, 1, 1);
            
            case ScaleMode.Y:
                return new Vector3(1, value, 1);
            
            case ScaleMode.Z:
                return new Vector3(1, 1, value);
            
            case ScaleMode.NoScaling:
                return Vector3.one;
            
            default:
                return Vector3.zero;
            
        }
    }

    public static Vector3 AxisConverter(Axis axis)
    {
        return axis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };
    }

    public static float ConvertFloatToInterval(float value, float normalMin, float normalMax, float intervalMin,
        float intervalMax)
    {
        var scaleRate =  (intervalMax - intervalMin) / (normalMax - normalMin);
        var diffNormal = value - normalMin;
        var valueFromInterval = intervalMin + (diffNormal * scaleRate);
        return valueFromInterval;
    }
    
    public static float ConvertFloatToIntervalReversed(float value, float normalMin, float normalMax, float intervalMin,
        float intervalMax)
    {
        var scaleRate =  (intervalMax - intervalMin) / (normalMax - normalMin);
        var diffNormal = value - normalMin;
        var valueFromInterval = intervalMax - (diffNormal * scaleRate);
        return valueFromInterval;
    }
}
