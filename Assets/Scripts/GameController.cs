using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    
    public static GameConfig Config { get; private set; }
    [SerializeField] private GameConfig config;


    public Axis MapAlongAxis => alongAxis;
    [SerializeField] private Axis alongAxis;
    
    #region Controllers

    private IntermediateObjectController IOCInstance => IntermediateObjectController.Instance;
    private ObstacleController OController => ObstacleController.Instance;

    #endregion
    
    #region Character

    public GameObject Boss => boss;
    [SerializeField] private GameObject boss;
    
    public Transform BossTransform { get; private set; }
    
    public List<GameObject> characters;
    private Dictionary<Side, GameObject> CharactersDict { get; set; }
    private Dictionary<Side, CharController> CharacterControllers { get; set; }
    public Dictionary<Side, Transform> CharacterTransforms { get; private set; }

    private Dictionary<Side, bool> LeftAndRightCanShrink => new Dictionary<Side, bool>()
    {
        {Side.Left, CharacterControllers[Side.Left].IsCharacterGhostMode},
        {Side.Right, CharacterControllers[Side.Right].IsCharacterGhostMode}
    };
    #endregion

    [SerializeField] private List<MapConfig> maps;
    [SerializeField] private Transform leftSide;
    [SerializeField] private Transform rightSide;
    [SerializeField] private Transform middleSide;
    [SerializeField] private Transform finalPlatform;

    public Vector3 LeftSideVec { get; private set; }
    public Vector3 RightSideVec { get; private set; }
    public Vector3 MiddleSideVec { get; private set; }

    private Coroutine _currentIntermediateObjectSpawner;
    private bool IsAnyIntermediateObjAlive => IOCInstance.IsThereAnyIntermediateObjectMoving();
    private bool _gameFinishAnimationStarted;
    private bool _gameFailed;
    
    //Animations
    private static readonly int RunBack = Animator.StringToHash("RunBack");
    private static readonly int SpeedOfThrow = Animator.StringToHash("SpeedOfThrow");
    private static readonly int Die = Animator.StringToHash("Die");


    [SerializeField] private List<GameObject> multiplierBlocks;
    private float speedOfFinalPart;

    private void Awake()
    {
        Instance = this;
        Config = config;

        LeftSideVec = leftSide.position;
        RightSideVec = rightSide.position;
        MiddleSideVec = middleSide.position;

        BossTransform = Boss.GetComponent<Transform>();

        speedOfFinalPart = Config.SpeedOfFinalPart;

        CharactersDict = characters
            .Select(o => new KeyValuePair<Side, GameObject>(o.GetComponent<CharController>().SideOfChar, o))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        CharacterControllers = CharactersDict
            .Select(pair => new KeyValuePair<Side, CharController>(pair.Key, pair.Value.GetComponent<CharController>()))
            .ToDictionary(pair => pair.Key,pair => pair.Value);
        CharacterTransforms = CharactersDict
            .Select(pair => new KeyValuePair<Side, Transform>(pair.Key, pair.Value.GetComponent<Transform>()))
            .ToDictionary(pair => pair.Key,pair => pair.Value);

        var step = 0.0f;
        var matBlock = new MaterialPropertyBlock();
        foreach (var multiplierBlock in multiplierBlocks)
        {
            var color = Color.HSVToRGB(step, 1, 1);
            matBlock.SetColor("_Color", color);
            
            multiplierBlock.GetComponent<Renderer>().SetPropertyBlock(matBlock);
            
            step += 1.0f / multiplierBlocks.Count;
        }
    }
    

    private void Start()
    {
        GameActions.GameFailed += () => Debug.Log("Game Failed");
        GameActions.BossRunningStarted += () => Debug.Log("BossRunningStarted");
        GameActions.GameFinishAnimationStarted += () => Debug.Log("Game Finish Anim Started");
        GameActions.GameEndedWithWinning += () => Debug.Log("Game Win");


        CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.speed = 0);
        OController.SpawnMapObstacles(maps[0]);

        switch (MapAlongAxis)
        {
            case Axis.X:
                finalPlatform.position = new Vector3(OController.LastObstaclePos.x + Config.DistanceBetweenFinalAndLastObstacle, MiddleSideVec.y, MiddleSideVec.z);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        DOTween.SetTweensCapacity(1000,50);
    }

    void Update()
    {
        
        var along = MapAlongAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };

        finalPlatform.position -= along * (speedOfFinalPart * Time.deltaTime);
        
        
        
        if (LeftAndRightCanShrink.All(pair => pair.Value == true) && !_gameFinishAnimationStarted && !IsAnyIntermediateObjAlive)
        {
            GameActions.GameFailed?.Invoke();
        }

        var lastObjPos = OController.LastObstacleTransform.position;
        
        switch (MapAlongAxis)
        {
            case Axis.X:
                if(_gameFinishAnimationStarted || _gameFailed) break;
                
                if (lastObjPos.x < CharacterTransforms[Side.Left].position.x)
                {
                    _gameFinishAnimationStarted = true;
                    if(_currentIntermediateObjectSpawner != null)
                        StopCoroutine(_currentIntermediateObjectSpawner);

                    speedOfFinalPart = 0;
                    StartCoroutine(StartEndAnimation());
                    GameActions.GameFinishAnimationStarted?.Invoke();
                }
                break;
            default:
                throw new NotImplementedException();
        }
        
        if (Input.touchCount > 0 && !_gameFinishAnimationStarted && !_gameFailed)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                var touchPos = touch.position;
                ScreenTouched(touchPos.x > (float) Screen.width / 2 ? Side.Right : Side.Left);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                StopCoroutine(_currentIntermediateObjectSpawner);
            }
        }
        
    }


    private IEnumerator StartEndAnimation()
    {
        while (IsAnyIntermediateObjAlive)
        {
            yield return null;
        }

        yield return new WaitForSeconds(Config.DurationOfAnimation + 0.1f);
        
        StartCoroutine(CharacterControllers[Side.Left].StartTransfer(Side.Middle));
        StartCoroutine(CharacterControllers[Side.Right].StartTransfer(Side.Middle));

        while (LeftAndRightCanShrink.Any(pair => pair.Value != true) || IsAnyIntermediateObjAlive)
        {
            yield return null;
        }
        
        CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.speed = 1);
        CharactersDict[Side.Boss].GetComponentsInChildren<Animator>()
            .ForEach(animator => animator.SetBool(RunBack, true));

        speedOfFinalPart = Config.SpeedOfFinalPart;
        CharacterTransforms[Side.Boss].parent = null;
        GameActions.BossRunningStarted?.Invoke();

        while (!CharacterControllers[Side.Middle].IsCharacterGhostMode || IsAnyIntermediateObjAlive)
        {
            var scale = CharacterControllers[Side.Middle].PointOfChar.x +
                        CharacterControllers[Side.Middle].PointOfChar.y;
            
            if (scale < Config.MinScaleForThrow)
                scale = Config.MinScaleForThrow;
            else if (scale > Config.MaxScaleForThrow)
                scale = Config.MaxScaleForThrow;

            var speed = Utilities.ConvertFloatToInterval(scale, Config.MinScaleForThrow,
                Config.MaxScaleForThrow, Config.MinThrowSpeed, Config.MaxThrowSpeed);
            
            CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.SetFloat(SpeedOfThrow, speed));
            yield return null;
        }
        CharactersDict[Side.Boss].GetComponentsInChildren<Animator>()
            .ForEach(animator => animator.SetBool(Die, true));

        speedOfFinalPart = 0;
        //duration of Die
        yield return new WaitForSeconds(2.3f);
        
        GameActions.GameEndedWithWinning?.Invoke();
    }

    public void MiddleCharSendOneIntermediate()
    {
        //the parametre Side Middle Doesnt important
        CharacterControllers[Side.Middle].TransferOne(Side.Boss);
    }

    private void ScreenTouched(Side side)
    {
        if (IOCInstance.IsThereAnyIntermediateObjectMoving()) return;
        if(CharacterControllers[side].IsCharacterGhostMode) return;

        var goingTo = Utilities.GetOtherSide(side);
        _currentIntermediateObjectSpawner = StartCoroutine(CharacterControllers[side].StartTransfer(goingTo));
    }
    
}
