using System;

public enum Side
{
    Left,
    Right
}

public enum NormalObstacleType
{
    AddWeight,
    ExtractWeight,
    MultiplyWeight,
    DivideWeight,
    AddHeight,
    ExtractHeight,
    MultiplyHeight,
    DivideHeight,
    Destroyer,
    Nothing
}

[Serializable]
public struct ObstacleStruct
{
    public NormalObstacleType obstacleType;
    public int obstaclePoint;
}
