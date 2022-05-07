using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    
    public static GameConfig Config { get; private set; }
    [SerializeField] private GameConfig config;
    private IntermediateObjectController IOCInstance => IntermediateObjectController.Instance;

    #region Character
    
    [SerializeField] private GameObject leftCharacter;
    [SerializeField] private GameObject rightCharacter;

    public Dictionary<Side, GameObject> Characters { get; private set; }
    public Dictionary<Side, CharController> CharacterControllers { get; private set; }
    public Dictionary<Side, Transform> CharacterTransforms { get; private set; }
    public Dictionary<Side, Vector2> PointOfSide => new Dictionary<Side, Vector2>
    {
        {Side.Left, CharacterControllers[Side.Left].PointOfChar}, 
        {Side.Right, CharacterControllers[Side.Right].PointOfChar}
    };
    
    #endregion

    private Coroutine _currentIntermediateObjectSpawner;

    private void Awake()
    {
        Instance = this;
        Config = config;
        Characters = new Dictionary<Side, GameObject>{{Side.Left, leftCharacter}, {Side.Right, rightCharacter}};

        CharacterTransforms = new Dictionary<Side, Transform>
        {
            {Side.Left, Characters[Side.Left].transform},
            {Side.Right, Characters[Side.Right].transform}
        };
        
        CharacterControllers = new Dictionary<Side, CharController>
        {
            {Side.Left, Characters[Side.Left].GetComponent<CharController>()},
            {Side.Right, Characters[Side.Right].GetComponent<CharController>()}
        };
        
    }
    
    
    void Update()
    {

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                var touchPos = touch.position;
                ScreenTouched(touchPos.x > (float) Screen.width / 2 ? Side.Right : Side.Left);
            }
        }
    }
    
    


    private void ScreenTouched(Side side)
    {
        if (IOCInstance.IsThereAnyIntermediateObjectMoving()) return;
        if(CharacterControllers[side].IsCharacterGhostMode) return;

        _currentIntermediateObjectSpawner = StartCoroutine(CharacterControllers[side].StartTransfer(Config.DelayBetweenTwoIntermediate));
    }
    
}
