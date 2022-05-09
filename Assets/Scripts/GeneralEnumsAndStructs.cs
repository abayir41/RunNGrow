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
    PartRemover,
    Nothing
}

[Serializable]
public struct ObstacleStruct
{
    public NormalObstacleType obstacleType;
    public int obstaclePoint;
}

public enum ScaleMode
{
    X,
    Y,
    Z,
    XY,
    XZ,
    YZ,
    XYZ,
    NoScaling
}

public enum Axis
{
    X,
    Y,
    Z
}