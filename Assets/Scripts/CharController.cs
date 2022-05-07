using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using UnityEngine;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

public class CharController : MonoBehaviour
{
    
    private IntermediateObjectController IOCInstance => IntermediateObjectController.Instance;
    private GameController ControllerInstance => GameController.Instance;
    private GameConfig Config => GameController.Config;
    
    //Character Fields
    public Side SideOfChar => sideOfChar;
    [SerializeField] private Side sideOfChar;
    public bool CanCharShrinkMore { get; private set; }
    public Vector2 PointOfChar { get; private set; }
    public bool IsCharacterGhostMode { get; private set; }
    
    //Other
    
    
    
    private void Awake()
    {
        CanCharShrinkMore = true;
    }

    private void Start()
    {
        PointOfChar = GameController.Config.StartPoint;
    }

    private void OnEnable()
    {
        IntermediateObjectActions.IntermediateObjectArrivedSuccessfully += IntermediateObjectArrivedSuccessfully;
        
        GameActions.NormalObstacleColl += NormalObstacleColl;
    }
    


    private void OnDisable()
    {
        IntermediateObjectActions.IntermediateObjectArrivedSuccessfully -= IntermediateObjectArrivedSuccessfully;
        
        GameActions.NormalObstacleColl -= NormalObstacleColl;
    }

    #region Intermediate Moves

        private void IntermediateObjectArrivedSuccessfully(Vector2 size, GameObject obj, Side side)
    {
        if(side != SideOfChar) return;

        if (PointOfChar == Vector2.zero)
            SetCharToVisibleMode();
        
        PointOfChar += size;
        
        CanCharShrinkMore = true;
        

        var result =  new Vector3(PointOfChar.x, PointOfChar.y, 1);
        transform.DOScale(result, 0.5f);
    }
    
    private void IntermediateStartedToMove(Vector2 size, Side side)
    {
        if(side != SideOfChar) return;
        
        PointOfChar -= size;

        if (PointOfChar == Vector2.zero)
        {
            CanCharShrinkMore = false;
            SetCharToGhostMode();
            return;
        }


        var result = new Vector3(PointOfChar.x, PointOfChar.y, 1);
        transform.DOScale(result, 0.5f);

    }
    
    
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public IEnumerator StartTransfer(float delay)
    {
        var goingTo = Utilities.GetOtherSide(SideOfChar);
        var leaveSide = SideOfChar;


        while (CanCharShrinkMore)
        {
            var leftSidePos = ControllerInstance.CharacterTransforms[Side.Left].position;
            var rightSidePos = ControllerInstance.CharacterTransforms[Side.Right].position;

            var startPos = goingTo == Side.Right ? leftSidePos : rightSidePos;
            var endPos = goingTo == Side.Right ? rightSidePos : leftSidePos;
            
            
            var sizeX = 0;
            var sizeY = 0;

            if (PointOfChar.x != 1.0f)
                sizeX = 1;
            
            if (PointOfChar.y != 1.0f)
                sizeY = 1;
            
            if (PointOfChar.x == 1 && PointOfChar.y == 1)
            {
                sizeX = 1;
                sizeY = 1;
            }
            
            var resultSize = new Vector2(sizeX, sizeY);
            
            IntermediateStartedToMove(resultSize, leaveSide);

            IOCInstance.MoveIntermediateObject(startPos, endPos, Config.DurationOfAnimation, goingTo, resultSize);
            yield return new WaitForSeconds(delay);

        }
    }

    #endregion

    
    private void NormalObstacleColl(NormalObstacleObject obstacle, CharController @char)
    {
        if (@char != this) return;

        var resultAfterHit = Utilities.NormalObstacleToPoint(obstacle.Type, PointOfChar, obstacle.ObstaclePoint);

        var x = resultAfterHit.x <= 0;
        var y = resultAfterHit.y <= 0;

        PointOfChar = resultAfterHit;
        
        transform.DOScale(new Vector3(x ? 1 : resultAfterHit.x, y ? 1 : resultAfterHit.y, 1), 0.5f).OnComplete(() =>
        {
            if (x || y)
            {
                SetCharToGhostMode();
            }
        });

    }

    private void SetCharToGhostMode()
    {
        transform.localScale = Vector3.one;
        PointOfChar = Vector2.zero;
        GetComponent<MeshRenderer>().enabled = false;
        IsCharacterGhostMode = true;
    }

    private void SetCharToVisibleMode()
    {
        GetComponent<MeshRenderer>().enabled = true;
        IsCharacterGhostMode = false;
    }
}
