using UnityEngine;


[CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    //------------------------
    //Intermediate Object Configuration
    //------------------------
    public float Amplitude => amplitude;
    public float DurationOfAnimation => durationOfAnimation;
    public float DelayMin => delayMin;
    public float DelayMax => delayMax;
    
    [Header("Intermediate Object Configuration")] 
    [SerializeField] private float amplitude;
    [SerializeField] private float durationOfAnimation;
    [SerializeField] private float delayMin;
    [SerializeField] private float delayMax;

    //------------------------
    //Game Dynamic Configurations
    //------------------------
    public Vector2 StartPoint => startPoint;
    public float SpeedOfObstacles => speedOfObstacles;
    public float CameraAnimDuration => cameraAnimDuration;

    [Header("Game Dynamic Configuration")] 
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private float speedOfObstacles;
    [SerializeField] private float cameraAnimDuration;

    //------------------------
    //Obstacles Configurations
    //------------------------
    public float DistanceBetweenObstacle => distanceBetweenObstacle;
    public Material RedColor => redColor;
    public Material BlueColor => blueColor;
    public float DistanceLastObstacleFinishPoint => distanceLastObstacleFinishPoint;
    public float Obs0Rot => obs0Rot;
    public float Obs1Rot => obs1Rot;
    public float Obs3Rot => obs3Rot;
    
    [Header("Obstacles Configurations")] 
    [SerializeField] private float distanceBetweenObstacle;
    [SerializeField] private float distanceLastObstacleFinishPoint;
    [SerializeField] private Material redColor;
    [SerializeField] private Material blueColor;
    [SerializeField] private float obs0Rot;
    [SerializeField] private float obs1Rot;
    [SerializeField] private float obs3Rot;


    //------------------------
    //Character Configurations
    //------------------------
    public float WidthScalingCoefficient => widthScalingCoefficient;
    public float HeightScalingCoefficient => heightScalingCoefficient;
    public float MinWidth => minWidth;
    public float MinHeight => minHeight;
    public float MinScaleForDelay => minScaleForDelay;
    public float MaxScaleForDelay => maxScaleForDelay;
    public float ScalingAnimationDuration => scalingAnimationDuration;
    public float ColorChangeAnimDuration => colorChangeAnimDuration;
    public float MaxWidth => maxWidth;
    public float MaxHeight => maxHeight;
     
    [Header("Character Configurations")]
    [SerializeField] private float widthScalingCoefficient;
    [SerializeField] private float heightScalingCoefficient;
    [SerializeField] private float minWidth;
    [SerializeField] private float minHeight;
    [SerializeField] private float minScaleForDelay;
    [SerializeField] private float maxScaleForDelay;
    [SerializeField] private float scalingAnimationDuration;
    [SerializeField] private float colorChangeAnimDuration;
    [SerializeField] private float maxWidth;
    [SerializeField] private float maxHeight;
    
    
    //-------------------------------
    //Middle Character Configurations
    //-------------------------------
    public float ThrowAnimationSpeed => throwAnimationSpeed;
    public float BallDuration => ballDuration;
    public int BallPower => ballPower;
    public float RacketAnimation => racketAnimation;
    
    [Header("MiddleChar Configurations")] 
    [SerializeField] private float throwAnimationSpeed;
    [SerializeField] private int ballPower;
    [SerializeField] private float ballDuration;
    [SerializeField] private float racketAnimation;
 
    
    //-------------------------------
    //Final Part Configurations
    //-------------------------------
    public float DistanceBetweenFinalAndLastObstacle => distanceBetweenFinalAndLastObstacle;
    public float SpeedOfFinalPart => speedOfFinalPart;
    public float GoingWhiteAnimDuration => goingWhiteAnimDuration;
    public float GoingGreenAnimDuration => goingGreenAnimDuration;
    
    [Header("Final Configurations")]
    [SerializeField] private float distanceBetweenFinalAndLastObstacle;
    [SerializeField] private float speedOfFinalPart;
    [SerializeField] private float goingWhiteAnimDuration;
    [SerializeField] private float goingGreenAnimDuration;
    
    //-------------------------------
    //UI Configurations
    //-------------------------------

    public float UIMoveAnimationDuration => uiMoveAnimationDuration;
    public float UIFadeLoopAnimationDuration => uiFadeLoopAnimationDuration;
    public float UIFadeOutAnimationDuration => uiFadeOutAnimationDuration;
    public float UITimeBetweenPerScoreAdding => uiTimeBetweenPerScoreAdding;
    public float UIMinimizeTime => uiMinimizeTime;
    public float DurationOfSkinUnlockAnimation => durationOfSkinUnlockAnimation;
    
    [Header("UI Configurations")] 
    [SerializeField] private float uiMoveAnimationDuration;
    [SerializeField] private float uiFadeOutAnimationDuration;
    [SerializeField] private float uiFadeLoopAnimationDuration;
    [SerializeField] private float uiTimeBetweenPerScoreAdding;
    [SerializeField] private float uiMinimizeTime;
    [SerializeField] private float durationOfSkinUnlockAnimation;
}