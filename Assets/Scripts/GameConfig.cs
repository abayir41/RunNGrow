using UnityEngine;


[CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    public float Amplitude => amplitude;
    public float DurationOfAnimation => durationOfAnimation;
    
    [Header("Intermediate Object Configuration")] 
    [SerializeField] private float amplitude;
    [SerializeField] private float durationOfAnimation;


    public Vector2 StartPoint => startPoint;
    public float DelayBetweenTwoIntermediate => delayBetweenTwoIntermediate;

    [Header("Game Dynamic Configuration")] 
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private float delayBetweenTwoIntermediate;

}