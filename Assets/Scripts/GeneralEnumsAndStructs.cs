using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public enum Side
{
    Left,
    Middle,
    Right,
    Boss,
    Null
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

public readonly struct TweenObjectAnims
{
    public TweenObjectAnims(TweenerCore<Vector3, Vector3, VectorOptions> animX,
        TweenerCore<Vector3, Vector3, VectorOptions> animYp1,
        TweenerCore<Vector3, Vector3, VectorOptions> animYp2,
        TweenerCore<Vector3, Vector3, VectorOptions> animZ)
    {
        AnimX = animX;
        AnimYp1 = animYp1;
        AnimYp2 = animYp2;
        AnimZ = animZ;
    }

    private TweenerCore<Vector3, Vector3, VectorOptions> AnimX { get; }
    private TweenerCore<Vector3, Vector3, VectorOptions> AnimYp1 { get; }
    private TweenerCore<Vector3, Vector3, VectorOptions> AnimYp2 { get; }
    private TweenerCore<Vector3, Vector3, VectorOptions> AnimZ { get; }

    public void SetOnComplete(Action action)
    {
        AnimX.OnComplete(() => action?.Invoke());
    }

    public void KillAllAnims()
    {
        AnimX?.Kill();
        AnimYp1?.Kill();
        AnimYp2?.Kill();
        AnimZ?.Kill();
    }
}

public interface ISkin
{
    void EnableSkin();
    void DisableSkin();
}

[Flags]
public enum UIAnimationTypes 
{
    MoveLeft = 1,
    MoveRight = 2,
    MoveUp = 4,
    MoveDown = 8,
    FadeOut = 16,
    FadeLoop = 32
}




