using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameConfig Config { get; private set; }
    
    private IntermediateObjectController IOCInstance => IntermediateObjectController.Instance;

    [SerializeField] private GameConfig config;
    [SerializeField]private Transform leftSide;
    [SerializeField]private Transform rightSide;
    private Dictionary<Side, int> _pointsOfSide;

    private void Awake()
    {
        _pointsOfSide = new Dictionary<Side, int> {{Side.Left, 0}, {Side.Right, 0}};
        Config = config;
    }

    private void Start()
    {
        _pointsOfSide[Side.Left] = 10;
        _pointsOfSide[Side.Right] = 10;
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
    
    private void IntermediateObjectArrivedSuccessfully(GameObject obj, Side goingTo)
    {
        _pointsOfSide[goingTo]++;
    }
    
    private void IntermediateStartToMove(GameObject obj, Side leaveFrom)
    {
        _pointsOfSide[leaveFrom]--;
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
    }
    
    
    IEnumerator StartMovement(Side goingTo, float delay)
    {
        var leaveSide = Utilities.GetOtherSide(goingTo);
        var canAnotherObjectGo = true;
        
        var leftSidePos = leftSide.position;
        var rightSidePos = rightSide.position;

        var startPos = goingTo == Side.Right ? leftSidePos : rightSidePos;
        var endPos = goingTo == Side.Right ? rightSidePos : leftSidePos;
        
        while (canAnotherObjectGo)
        {
            IOCInstance.MoveIntermediateObject(startPos, endPos, Config.DurationOfAnimation, goingTo);
            yield return new WaitForSeconds(delay);

            if (_pointsOfSide[leaveSide] == 0)
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
        if(_pointsOfSide[leaveSide] == 0) return;

        StartCoroutine(StartMovement(goingTo, Config.DelayBetweenTwoIntermediate));
    }
}
