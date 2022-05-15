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

    [Header("Game Dynamic Configuration")] 
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private float speedOfObstacles;

    //------------------------
    //Obstacles Configurations
    //------------------------
    public float DistanceBetweenObstacle => distanceBetweenObstacle;
    public Material RedColor => redColor;
    public Material GreenColor => greenColor;
    
    [Header("Obstacles Configurations")] 
    [SerializeField] private float distanceBetweenObstacle;
    [SerializeField] private Material redColor;
    [SerializeField] private Material greenColor;


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
     
    [Header("Character Configurations")]
    [SerializeField] private float widthScalingCoefficient;
    [SerializeField] private float heightScalingCoefficient;
    [SerializeField] private float minWidth;
    [SerializeField] private float minHeight;
    [SerializeField] private float minScaleForDelay;
    [SerializeField] private float maxScaleForDelay;
    [SerializeField] private float scalingAnimationDuration;
    
    
    //-------------------------------
    //Middle Character Configurations
    //-------------------------------
    public float MinThrowSpeed => minThrowSpeed;
    public float MaxThrowSpeed => maxThrowSpeed;
    public float MinScaleForThrow => minScaleForThrow;
    public float MaxScaleForThrow => maxScaleForThrow;
    
    [Header("MiddleChar Configurations")] 
    [SerializeField] private float minThrowSpeed;
    [SerializeField] private float maxThrowSpeed;
    [SerializeField] private float minScaleForThrow;
    [SerializeField] private float maxScaleForThrow;
    
    //-------------------------------
    //Final Part Configurations
    //-------------------------------
    public float DistanceBetweenFinalAndLastObstacle => distanceBetweenFinalAndLastObstacle;
    public float SpeedOfFinalPart => speedOfFinalPart;
    
    [Header("Final Configurations")]
    [SerializeField] private float distanceBetweenFinalAndLastObstacle;
    [SerializeField] private float speedOfFinalPart;

}