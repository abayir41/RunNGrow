using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

    //Visuals
    [SerializeField] private List<GameObject> partOfChars;
    private List<MeshRenderer> _meshRenderersOfParts;
    private Dictionary<Transform, ScaleMode> _transformsOfParts;
    
    //cofiguration
    [SerializeField] private float widthScalingCoef;
    [SerializeField] private float heightScalingCoef;
    [SerializeField] private float minWidth;
    [SerializeField] private float minHeight;
    
    //Fixing Body
    [Header("BodyFixing")]
    [SerializeField] private Transform lowerBodyPoint;
    [SerializeField] private Transform upperBodyPoint;
    [SerializeField] private Transform cylinderLowerPoint;
    [SerializeField] private Transform cylinderUpPoint;
    [SerializeField] private Transform upperBodyMovePoint;
    [SerializeField] private Vector3 upperBodyMoveVector;
    private const float ThresholdOfBodyFixing = 0.01f;

    [Button]
    private void CalculateUpperBodyMoveVector()
    {
        upperBodyMoveVector = upperBodyPoint.position - lowerBodyPoint.position;
        upperBodyMoveVector = upperBodyMoveVector.normalized;
    }


    private void Awake()
    {
        CanCharShrinkMore = true;
        _meshRenderersOfParts = new List<MeshRenderer>();
        foreach (var partOfChar in partOfChars)
        {
            _meshRenderersOfParts.AddRange(partOfChar.GetComponentsInChildren<MeshRenderer>());
        }
        _transformsOfParts = partOfChars.Select(o => new KeyValuePair<Transform, ScaleMode>(o.GetComponent<Transform>(), o.GetComponent<CharPart>().Mode)).ToDictionary(pair => pair.Key,pair => pair.Value);

    }

    private void Start()
    {
        PointOfChar = GameController.Config.StartPoint;

        var anims = GetAnimCharToAPoint(PointOfChar, 0.5f);
        anims.ForEach(core => core.Complete());
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
        
        
        GetAnimCharToAPoint(PointOfChar,0.5f);

    }
    
    private void IntermediateStartedToMove(Vector2 size, Side side)
    {
        if(side != SideOfChar) return;
        
        PointOfChar -= size;

        
        GetAnimCharToAPoint(PointOfChar,0.5f);

        
        if (PointOfChar == Vector2.zero)
        {
            CanCharShrinkMore = false;
            SetCharToGhostMode();
        }
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
    
    private void Update()
    {
        
        //Fixing Body
        var lowerUpperBodyDistance = Vector3.Distance(lowerBodyPoint.position, upperBodyPoint.position);
        var cylinderLenght = Vector3.Distance(cylinderLowerPoint.position, cylinderUpPoint.position);
        
        while (Math.Abs(cylinderLenght - lowerUpperBodyDistance) > ThresholdOfBodyFixing)
        {
            if (cylinderLenght < lowerUpperBodyDistance)
                cylinderLowerPoint.localScale += Vector3.forward * 0.00001f;
            else
                cylinderLowerPoint.localScale -= Vector3.forward * 0.00001f;
            
            lowerUpperBodyDistance = Vector3.Distance(lowerBodyPoint.position, upperBodyPoint.position);
            cylinderLenght = Vector3.Distance(cylinderLowerPoint.position, cylinderUpPoint.position);
        }

    }

    private void LateUpdate()
    {
        //Fixing Body
        lowerBodyPoint.LookAt(upperBodyPoint.position);
    }

    private void NormalObstacleColl(NormalObstacleObject obstacle, CharController @char)
    {
        if (@char != this) return;

        var resultAfterHit = Utilities.NormalObstacleToPoint(obstacle.Type, PointOfChar, obstacle.ObstaclePoint);

        var x = resultAfterHit.x < 0;
        var y = resultAfterHit.y < 0;

        PointOfChar = resultAfterHit;

        if (x || y)
        {
            GetAnimCharToAPoint(0,0,0.5f);

            PointOfChar = Vector2.zero;
            
            SetCharToGhostMode();
            
        }else if (obstacle.Type == NormalObstacleType.PartRemover)
        {
            GetAnimCharToAPoint(PointOfChar, 0.5f);
        }
        else
        {
            GetAnimCharToAPoint(PointOfChar, 0.5f);
        }
        
    }

    private List<TweenerCore<Vector3,Vector3,VectorOptions>> GetAnimCharToAPoint(float width, float height, float duration)
    {
        var anims = new List<TweenerCore<Vector3, Vector3, VectorOptions>>();
        
        _transformsOfParts.ForEach(pair =>
        {
           var animScale = pair.Key.DOScale(Utilities.ScaleModeToVector(pair.Value,minWidth + ((width - 1) * widthScalingCoef)), duration);
           anims.Add(animScale);
        });

       var anim =  upperBodyMovePoint.DOMove(upperBodyPoint.position + upperBodyMoveVector *  (minHeight + heightScalingCoef * (height-1)), duration);
       anims.Add(anim);

       return anims;
    }
    
    private List<TweenerCore<Vector3,Vector3,VectorOptions>> GetAnimCharToAPoint(Vector2 sizeVector, float duration)
    {
        var anims = new List<TweenerCore<Vector3, Vector3, VectorOptions>>();
        
        _transformsOfParts.ForEach(pair =>
        {
            var animScale = pair.Key.DOScale(Utilities.ScaleModeToVector(pair.Value,minWidth + ((sizeVector.x - 1) * widthScalingCoef)), duration);
            anims.Add(animScale);
        });
        
        
        var anim =  upperBodyMovePoint.DOMove(lowerBodyPoint.position + upperBodyMoveVector *  (minHeight + heightScalingCoef * (sizeVector.y - 1)), duration);
        anims.Add(anim);
        
        return anims;
    }

    private void SetCharToGhostMode()
    {
        _meshRenderersOfParts.ForEach(meshRenderer => meshRenderer.enabled = false);
        IsCharacterGhostMode = true;
    }

    private void SetCharToVisibleMode()
    {
        _meshRenderersOfParts.ForEach(meshRenderer => meshRenderer.enabled = true);
        IsCharacterGhostMode = false;
    }
    
    
}
