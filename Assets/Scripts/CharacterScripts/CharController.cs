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

    [ShowIf("sideOfChar", Side.Middle)] 
    [SerializeField] private Transform handPosition;

    //Visuals
    private List<GameObject> _partOfChars;
    private List<MeshRenderer> _meshRenderersOfParts;
    private Dictionary<Transform, ScaleMode> _transformsOfParts;
    
    //configuration
    private float WidthScalingCoefficient => Config.WidthScalingCoefficient;
    private float HeightScalingCoefficient => Config.HeightScalingCoefficient;
    private float MinWidth => Config.MinWidth;
    private float MinHeight => Config.MinHeight;
    
    //Fixing Body
    [Header("BodyFixing")]
    [SerializeField] private Transform lowerBodyPoint;
    [SerializeField] private Transform upperBodyPoint;
    [SerializeField] private Transform cylinderLowerPoint;
    [SerializeField] private Transform cylinderUpPoint;
    [SerializeField] private Transform upperBodyMovePoint;
    [SerializeField] private Vector3 upperBodyMoveVector;
    private Vector3 _diffVectorForMoveBodyPartAndLowerBody;
    private const float ThresholdOfBodyFixing = 0.01f;

    private List<Animator> _animators;
    private static readonly int Started = Animator.StringToHash("GameStarted");

    [Button]
    private void CalculateUpperBodyMoveVector()
    {
        upperBodyMoveVector = upperBodyPoint.position - lowerBodyPoint.position;
        upperBodyMoveVector = upperBodyMoveVector.normalized;
    }


    private void Awake()
    {
        if (sideOfChar != Side.Middle)
        {
            CanCharShrinkMore = true;
        }
        else
        {
            CanCharShrinkMore = false;
        }
        
        _meshRenderersOfParts = new List<MeshRenderer>();

        var charParts = GetComponentsInChildren<CharPart>().ToList();
        _partOfChars = charParts.Select(part => part.gameObject).ToList();
        foreach (var partOfChar in _partOfChars)
        {
            _meshRenderersOfParts.AddRange(partOfChar.GetComponentsInChildren<MeshRenderer>());
        }
        _transformsOfParts = _partOfChars.Select(o => new KeyValuePair<Transform, ScaleMode>(o.GetComponent<Transform>(), o.GetComponent<CharPart>().Mode)).ToDictionary(pair => pair.Key,pair => pair.Value);

        _diffVectorForMoveBodyPartAndLowerBody = lowerBodyPoint.position - upperBodyMovePoint.position;

        _animators = GetComponentsInChildren<Animator>().ToList();
    }

    private void Start()
    {
        if (sideOfChar != Side.Middle)
        {
            PointOfChar = GameController.Config.StartPoint;
        }
        else
        {
            PointOfChar = Vector2.zero;
            SetCharToGhostMode();
        }
        
        var anims = GetAnimCharToAPoint(PointOfChar, 0.5f);
        anims.ForEach(core => core.Complete());
    }

    private void OnEnable()
    {
        GameActions.GameStarted += GameStarted;
        
        if(SideOfChar == Side.Boss) return;
        IntermediateObjectActions.IntermediateObjectArrivedSuccessfully += IntermediateObjectArrivedSuccessfully;
        
        GameActions.NormalObstacleColl += NormalObstacleColl;
    }
    
    
    private void OnDisable()
    {
        GameActions.GameStarted -= GameStarted;
        
        if(SideOfChar == Side.Boss) return;
        IntermediateObjectActions.IntermediateObjectArrivedSuccessfully -= IntermediateObjectArrivedSuccessfully;
        
        GameActions.NormalObstacleColl -= NormalObstacleColl;
    }

    private void GameStarted()
    {
        if(sideOfChar == Side.Left || sideOfChar == Side.Right)
            _animators.ForEach(animator => animator.SetBool(Started, true));
    }

    #region Intermediate Moves

        private void IntermediateObjectArrivedSuccessfully(Vector2 size, GameObject obj, Side side)
    {
        if(side != SideOfChar) return;

        var totalPoint = PointOfChar.x + PointOfChar.y;
        if (PointOfChar == Vector2.zero)
            SetCharToVisibleMode();
        
        PointOfChar += size;
        
        CanCharShrinkMore = true;
        
        
        GetAnimCharToAPoint(PointOfChar,Config.ScalingAnimationDuration);

    }
    
    private void IntermediateStartedToMove(Vector2 size, Side side)
    {
        if(side != SideOfChar) return;
        
        PointOfChar -= size;

        var x = PointOfChar.x;
        var y = PointOfChar.y;

        if (x <= 0 || y <= 0)
        {
            GetAnimCharToAPoint(0,0,Config.ScalingAnimationDuration);
            CanCharShrinkMore = false;
            SetCharToGhostMode();
        }
        else
        {
            GetAnimCharToAPoint(PointOfChar, Config.ScalingAnimationDuration);
        }
    }
    
    
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public IEnumerator StartTransfer(Side goingTo)
    {

        var delayMin = Config.DelayMin;
        var delayMax = Config.DelayMax;
        var scaleMin = Config.MinScaleForDelay;
        var scaleMax = Config.MaxScaleForDelay;

        while (CanCharShrinkMore)
        {
            TransferOne(goingTo);

            var delayScale = PointOfChar.x + PointOfChar.y;
            if (delayScale > scaleMax) delayScale = scaleMax; 
            else if (delayScale < scaleMin) delayScale = scaleMin;
            
            var delay = Utilities.ConvertFloatToIntervalReversed(delayScale, scaleMin, scaleMax, delayMin,
                delayMax);
            
            yield return new WaitForSeconds(delay);

        }
    }
    
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public void TransferOne(Side goingTo)
    {
        var leaveSide = SideOfChar;

        if (CanCharShrinkMore)
        {
            Vector3 startTransformPosition;
            Vector3 endTransformPosition;
            
            if (sideOfChar != Side.Middle)
            {
                startTransformPosition = ControllerInstance.CharacterTransforms[sideOfChar].position;
                endTransformPosition = ControllerInstance.CharacterTransforms[goingTo].position;
            }
            else
            {
                startTransformPosition = handPosition.position;
                endTransformPosition = ControllerInstance.CharacterTransforms[goingTo].position;
            }
            
            var sizeX = 0;
            var sizeY = 0;

            if (PointOfChar.x > 1.0f)
                sizeX = 1;
            
            if (PointOfChar.y > 1.0f)
                sizeY = 1;
            
            if (PointOfChar.x == 1 && PointOfChar.y == 1)
            {
                sizeX = 1;
                sizeY = 1;
            }
            
            
            var resultSize = new Vector2(sizeX, sizeY);
            
            IntermediateStartedToMove(resultSize, leaveSide);

            IOCInstance.MoveIntermediateObject(startTransformPosition, endTransformPosition, Config.DurationOfAnimation, goingTo, resultSize);
        }
    }

    
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public void TransferOneToPos( FinalPlatform platform, float duration , int ballPower)
    {
        var leaveSide = SideOfChar;

        if (CanCharShrinkMore)
        {
            Vector3 startTransformPosition;
            Vector3 endTransformPosition;
            
            if (sideOfChar != Side.Middle)
            {
                startTransformPosition = ControllerInstance.CharacterTransforms[sideOfChar].position;
                endTransformPosition = platform.pos.position;
            }
            else
            {
                startTransformPosition = handPosition.position;
                endTransformPosition = platform.pos.position;
            }
            
            var sizeX = 0;
            var sizeY = 0;

            if (PointOfChar.x > ballPower)
                sizeX = ballPower;
            
            if (PointOfChar.y > ballPower)
                sizeY = ballPower;
            
            if (PointOfChar.x <= ballPower && PointOfChar.y <= ballPower)
            {
                sizeX = (int)PointOfChar.x;
                sizeY = (int)PointOfChar.y;
            }
            
            
            var resultSize = new Vector2(sizeX, sizeY);
            
            IntermediateStartedToMove(resultSize, leaveSide);

            IOCInstance.MoveIntermediateObject(startTransformPosition, endTransformPosition, duration,platform, resultSize);
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
           var animScale = pair.Key.DOScale(Utilities.ScaleModeToVector(pair.Value,MinWidth + ((width - 1) * WidthScalingCoefficient)), duration);
           anims.Add(animScale);
        });

       var anim =  upperBodyMovePoint.DOMove(upperBodyPoint.position - _diffVectorForMoveBodyPartAndLowerBody + upperBodyMoveVector *  (MinHeight + HeightScalingCoefficient * (height-1)), duration);
       anims.Add(anim);

       return anims;
    }
    
    private List<TweenerCore<Vector3,Vector3,VectorOptions>> GetAnimCharToAPoint(Vector2 sizeVector, float duration)
    {
        var anims = new List<TweenerCore<Vector3, Vector3, VectorOptions>>();
        
        _transformsOfParts.ForEach(pair =>
        {
            var animScale = pair.Key.DOScale(Utilities.ScaleModeToVector(pair.Value,MinWidth + ((sizeVector.x - 1) * WidthScalingCoefficient)), duration);
            anims.Add(animScale);
        });
        
        
        var anim =  upperBodyMovePoint.DOMove(lowerBodyPoint.position - _diffVectorForMoveBodyPartAndLowerBody + upperBodyMoveVector *  (MinHeight + HeightScalingCoefficient * (sizeVector.y - 1)), duration);
        anims.Add(anim);
        
        
        return anims;
    }
    
    private void SetCharToGhostMode()
    {
        _meshRenderersOfParts.ForEach(meshRenderer => meshRenderer.enabled = false);
        CanCharShrinkMore = false;
        IsCharacterGhostMode = true;
    }

    private void SetCharToVisibleMode()
    {
        _meshRenderersOfParts.ForEach(meshRenderer => meshRenderer.enabled = true);
        CanCharShrinkMore = false;
        IsCharacterGhostMode = false;
    }
    
    
}
