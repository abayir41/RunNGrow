using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameConfig Config { get; private set; }
    
    private IntermediateObjectController IOCInstance => IntermediateObjectController.Instance;

    [SerializeField] private GameConfig config;
    [SerializeField]private Transform leftSide;
    [SerializeField]private Transform rightSide;
    private Dictionary<Side, Vector2> _pointsOfSide;
    private Coroutine _currentIntermediateObjectSpawner;

    private void Awake()
    {
        _pointsOfSide = new Dictionary<Side, Vector2> {{Side.Left, Vector2.zero}, {Side.Right, Vector2.zero}};
        Config = config;
    }

    private void Start()
    {
        _pointsOfSide[Side.Left] = new Vector2(10,5);
        _pointsOfSide[Side.Right] = new Vector2(10,5);
    }

    #region Subscriptions
    
    private void OnEnable()
    {
        IntermediateObjectActions.IntermediateObjectArrivedSuccessfully += IntermediateObjectArrivedSuccessfully;
        IntermediateObjectActions.IntermediateStartToMove += IntermediateStartToMove;
    }
    
    private void OnDisable()
    {
        IntermediateObjectActions.IntermediateObjectArrivedSuccessfully -= IntermediateObjectArrivedSuccessfully;
        IntermediateObjectActions.IntermediateStartToMove -= IntermediateStartToMove;
    }
    
    private void IntermediateObjectArrivedSuccessfully(Vector2 size ,GameObject obj, Side goingTo)
    {
        _pointsOfSide[goingTo] += size;
    }
    
    private void IntermediateStartToMove(Vector2 sizeOfLeavePart ,Side leaveFrom)
    {
        _pointsOfSide[leaveFrom] -= sizeOfLeavePart;
    }
    
    #endregion

    
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

        if (Input.GetKeyDown(KeyCode.A))
        {
            ScreenTouched(Side.Left);
        }
        
        if(Input.GetKeyDown(KeyCode.D))
            IOCInstance.KillTheAnimations();
    }
    
    
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    private IEnumerator StartMovement(Side goingTo, float delay)
    {
        var leaveSide = Utilities.GetOtherSide(goingTo);
        var canAnotherObjectGo = true;
        
        var leftSidePos = leftSide.position;
        var rightSidePos = rightSide.position;

        var startPos = goingTo == Side.Right ? leftSidePos : rightSidePos;
        var endPos = goingTo == Side.Right ? rightSidePos : leftSidePos;
        
        while (canAnotherObjectGo)
        {
            var sizeX = 0;
            var sizeY = 0;

            var sizeOfObject = _pointsOfSide[leaveSide];
            
            if (sizeOfObject.x != 1.0f)
                sizeX = 1;
            
            if (sizeOfObject.y != 1.0f)
                sizeY = 1;
            
            if (sizeOfObject.x == 1 && sizeOfObject.y == 1)
            {
                sizeX = 1;
                sizeY = 1;
            }
            
            var resultSize = new Vector2(sizeX, sizeY);
            
            IntermediateObjectActions.IntermediateStartToMove?.Invoke(resultSize, leaveSide);
            
            IOCInstance.MoveIntermediateObject(startPos, endPos, Config.DurationOfAnimation, goingTo, resultSize);
            yield return new WaitForSeconds(delay);

            if (_pointsOfSide[leaveSide] == Vector2.zero)
            {
                canAnotherObjectGo = false;
            }
        }
    }


    private void ScreenTouched(Side side)
    {
        
        var leaveSide = side;
        var goingTo = Utilities.GetOtherSide(side);

        if (IOCInstance.IsThereAnyIntermediateObjectMoving()) return;
        if(_pointsOfSide[leaveSide] == Vector2.zero) return;

        _currentIntermediateObjectSpawner = StartCoroutine(StartMovement(goingTo, Config.DelayBetweenTwoIntermediate));
    }
    
}
