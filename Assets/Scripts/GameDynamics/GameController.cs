using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    
    public static GameConfig Config { get; private set; }
    [SerializeField] private GameConfig config;


    public Axis MapAlongAxis => alongAxis;
    [SerializeField] private Axis alongAxis;

    public static int LevelIndex
    {
        get => PlayerPrefs.GetInt("Level");
        private set => PlayerPrefs.SetInt("Level", value);
    } 
    
    #region Controllers

    private IntermediateObjectController IOCInstance => IntermediateObjectController.Instance;
    private ObstacleController OController => ObstacleController.Instance;

    private UIController UIControl => UIController.Instance;
    
    private FinalPartController FinalController => FinalPartController.Instance;

    #endregion
    
    #region Character

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
    
    [SerializeField] private Transform leftSide;
    [SerializeField] private Transform rightSide;
    [SerializeField] private Transform middleSide;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraSideView;

    private Vector3 LeftSideVec { get; set; }
    private Vector3 RightSideVec { get; set; }
    public Vector3 MiddleSideVec { get; set; }

    private Coroutine _currentIntermediateObjectSpawner;
    private Side _sideOfCurrentIntermediateObjectsGoing;
    
    private bool IsAnyIntermediateObjAlive => IOCInstance.IsThereAnyIntermediateObjectMoving();
    private bool _gameFinishAnimationStarted;
    public bool gameFailed;
    
    private static readonly int SpeedOfThrow = Animator.StringToHash("SpeedOfThrow");


    [HorizontalGroup("Base")] [BoxGroup("Base/Left")]
    [SerializeField] private List<GameObject> multiplierBlocks;
    [BoxGroup("Base/Right")]
    [SerializeField] private List<float> multiplyAmount;
    
    public float SpeedOfFinalPart { get; private set; }
    private float _gameTotalPoints;
    private bool _bossHitTheWall;
    public bool gameStarted;

    public Transform pathLastPoint;
    public Transform finalPartStartPoint;

    [Button]
    public void RemovePlayerCache()
    {
        PlayerPrefs.DeleteAll();
    }

    private void Awake()
    {
        //Float make comma the dot
        var customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        
        //Cache
        Instance = this;
        Config = config;
        LeftSideVec = leftSide.position;
        RightSideVec = rightSide.position;
        MiddleSideVec = middleSide.position;
        SpeedOfFinalPart = Config.SpeedOfFinalPart;

        //List Cache
        CharactersDict = characters
            .Select(o => new KeyValuePair<Side, GameObject>(o.GetComponent<CharController>().SideOfChar, o))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        CharacterControllers = CharactersDict
            .Select(pair => new KeyValuePair<Side, CharController>(pair.Key, pair.Value.GetComponent<CharController>()))
            .ToDictionary(pair => pair.Key,pair => pair.Value);
        CharacterTransforms = CharactersDict
            .Select(pair => new KeyValuePair<Side, Transform>(pair.Key, pair.Value.GetComponent<Transform>()))
            .ToDictionary(pair => pair.Key,pair => pair.Value);
    }
    

    private void Start()
    {
        GameActions.GameFailed += () => Debug.Log("Game Failed");
        GameActions.BossRunningStarted += () => Debug.Log("BossRunningStarted");
        GameActions.GameFinishAnimationStarted += () => Debug.Log("Game Finish Anim Started");
        GameActions.GameEndedWithWinning += () => Debug.Log("Game Win");

        
        //middleChar Stop
        CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.speed = 0);
        
        //Spawn Obstacles
        var map = MapKeeper.Instance.GetMap();
        OController.SpawnMapObstacles(map);
        FinalController.InstantiateFinalPart();

        var position = pathLastPoint.position;
        var position1 = finalPartStartPoint.position;
        leftSide.position -= new Vector3((position - position1).x,0,0);
        rightSide.position -= new Vector3((position - position1).x,0,0);


        DOTween.SetTweensCapacity(1000,50);
        
        UIControl.SetCoinText(MoneySystem.TotalPoints);
    }

    private void OnEnable()
    {
        GameActions.BossHitTheMax += BossHitTheMax;
        GameActions.GameStarted += GameStarted;
        GameActions.GameEndedWithWinning += GameEndedWithWinning;
    }

    

    private void OnDisable()
    {
        GameActions.BossHitTheMax -= BossHitTheMax;
        GameActions.GameStarted -= GameStarted;
        GameActions.GameEndedWithWinning -= GameEndedWithWinning;
    }

    private void GameEndedWithWinning()
    {
        PlayerPrefs.SetInt("total_level",PlayerPrefs.GetInt("total_level") + 1);
        LevelIndex++;
    }

    private void BossHitTheMax()
    {
        CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.speed = 0);
        _bossHitTheWall = true;
    }
    
    private void GameStarted()
    {
        gameStarted = true;
    }
    void Update()
    {
        
        if(!gameStarted) return;
        if(gameFailed) return;
        
        //Check if two char die
        if (LeftAndRightCanShrink.All(pair => pair.Value) && !_gameFinishAnimationStarted && !IsAnyIntermediateObjAlive)
        {
            gameFailed = true;
            UIControl.OpenDeathScreen();
            SoundManager.PlaySpecificSound?.Invoke(SoundTypes.Lose);
            VibrationManager.VibrationSpecific?.Invoke(HapticTypes.Failure);
            GameActions.GameFailed?.Invoke();
        }
    
        //check if the game was finished, if it is, make some steps
        var lastObjPos = OController.LastObstacleTransform.position;
        
        switch (MapAlongAxis)
        {
            case Axis.X:
                if(_gameFinishAnimationStarted || gameFailed) break;
                
                if (lastObjPos.x + config.DistanceLastObstacleFinishPoint < CharacterTransforms[Side.Left].position.x)
                {
                    _gameFinishAnimationStarted = true;
                    if(_currentIntermediateObjectSpawner != null)
                        StopCoroutine(_currentIntermediateObjectSpawner);

                    FinalController.finishLine.SetActive(false);
                    SpeedOfFinalPart = 0;
                    
                    StartCoroutine(StartEndAnimation());
                    GameActions.GameFinishAnimationStarted?.Invoke();
                }
                break;
            default:
                throw new NotImplementedException();
        }
        
        
        //Screen Touch Control
        if (Input.touchCount > 0 && !_gameFinishAnimationStarted && !gameFailed)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                var touchPos = touch.position;
                ScreenTouched(touchPos.x > (float) Screen.width / 2 ? Side.Right : Side.Left);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if(_currentIntermediateObjectSpawner != null)
                    StopCoroutine(_currentIntermediateObjectSpawner);
            }
        }
        
    }


    private IEnumerator StartEndAnimation()
    {
        
        bool cameraMoved = false;
        cameraTransform.DOMove(cameraSideView.position, Config.CameraAnimDuration).OnComplete(() => cameraMoved = true);
        cameraTransform.DORotateQuaternion(cameraSideView.rotation, Config.CameraAnimDuration);
        
        //Wait for any intermediate obj
        while (IsAnyIntermediateObjAlive)
        {
            yield return null;
        }

        //wait for growin animation
        

        //CalculateGame Points
        _gameTotalPoints += CharacterControllers[Side.Left].PointOfChar.x +
                           CharacterControllers[Side.Left].PointOfChar.y;
        _gameTotalPoints += CharacterControllers[Side.Right].PointOfChar.x +
                            CharacterControllers[Side.Right].PointOfChar.y;
        
        
        //Send points to middle char
        StartCoroutine(CharacterControllers[Side.Left].StartTransfer(Side.Middle));
        StartCoroutine(CharacterControllers[Side.Right].StartTransfer(Side.Middle));

        //wait for intermedie objs
        while (LeftAndRightCanShrink.Any(pair => pair.Value != true) || IsAnyIntermediateObjAlive || !cameraMoved)
        {
            yield return null;
        }
        
        //Start Boss throw balls
        CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.speed = 1);
        SpeedOfFinalPart = Config.SpeedOfFinalPart;

        GameActions.BossRunningStarted?.Invoke();

        CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.SetFloat(SpeedOfThrow, config.ThrowAnimationSpeed));
        
        
        //set throwing ball speed while throwing

        while (!(CharacterControllers[Side.Middle].IsCharacterGhostMode || !IsAnyIntermediateObjAlive && _bossHitTheWall))
        {
            yield return null;
        }
        
        /*
        while ((!CharacterControllers[Side.Middle].IsCharacterGhostMode || IsAnyIntermediateObjAlive) && !_bossHitTheWall)
        {
            yield return null;
        }
        */
        
        //Boss Die

        //stop the movement of last part
        SoundManager.PlaySpecificSound?.Invoke(SoundTypes.Win);
        VibrationManager.VibrationSpecific?.Invoke(HapticTypes.Success);
        SpeedOfFinalPart = 0;
        FinalPlatform finalPlatform;
        if (FinalController.finalPlatforms.Any(platform => !platform.gotHit))
        {
            finalPlatform = FinalController.finalPlatforms.Last(platform => platform.gotHit);
        }
        else
        {
            finalPlatform = FinalController.finalPlatforms.Last();
        }
        finalPlatform.gameObject.transform.DOScale(finalPlatform.gameObject.transform.localScale * 1.1f, 1).SetLoops(-1, LoopType.Yoyo);
        var addPoint = (int) (_gameTotalPoints * finalPlatform.multiplier);
        //duration of Die
        yield return new WaitForSeconds(1f);

        UIControl.OpenGameScorePart();
        
        float tempCoinText = MoneySystem.TotalPoints;
        float tempGameScoreText = 0;

        var anim = DOTween.To(() => tempCoinText, value =>
        {
            tempCoinText = value;
            UIControl.SetCoinText((int)tempCoinText);
        }, MoneySystem.TotalPoints + addPoint, addPoint * Config.UITimeBetweenPerScoreAdding);
        DOTween.To(() => tempGameScoreText, value =>
        {
            tempGameScoreText = value;
            UIControl.SetGameScoreText((int) tempGameScoreText);
        }, addPoint, addPoint * Config.UITimeBetweenPerScoreAdding);

        while (anim.IsPlaying())
        {
            yield return null;
        }

        
        //Skin Unlock Animation
        /*if (SkinSystem.Instance.IsThereAnyNewSkin())
        {
            int skinMinPoint;
            var skinIndex = SkinSystem.Instance.CurrentSkinIndex();
            var skinValue = SkinSystem.Instance.PriceOfSkins[skinIndex];
            if (skinIndex == 0)
            {
                skinMinPoint = 0;
            }
            else
            {
                skinMinPoint = SkinSystem.Instance.PriceOfSkins[skinIndex - 1];
            }

            var current = Utilities.ConvertFloatToInterval(ScoreSystem.TotalPoints, skinMinPoint, skinValue, 0, 1);
            var target =
                Utilities.ConvertFloatToInterval(ScoreSystem.TotalPoints + addPoint, skinMinPoint, skinValue, 0, 1);

            if (target >= 1)
            {
                StartCoroutine(UIControl.GetSkinUnlockScreen(current, 1, true));
            }
            else
            {
                StartCoroutine(UIControl.GetSkinUnlockScreen(current, target, false));
            }

        }
        else
        {
            UIControl.OpenEndTouchToContinue();
        }
        */
        
        UIControl.OpenBossEndContinue();
        
        ScoreSystem.Instance.AddPoint(addPoint);
        MoneySystem.Instance.AddPoint(addPoint);
        

        GameActions.GameEndedWithWinning?.Invoke();
    }

    private int _platformIndex;
    public void MiddleCharSendOneIntermediate()
    {
        if(_platformIndex == FinalController.finalPlatforms.Count - 1)
            BossHitTheMax();
        //the parametre Side Middle Doesnt important
        CharacterControllers[Side.Middle].TransferOneToPos(FinalController.finalPlatforms[_platformIndex], config.BallDuration, config.BallPower);
        _platformIndex++;
    }

    private void ScreenTouched(Side side)
    {
        var goingTo = Utilities.GetOtherSide(side);
        
        if (IOCInstance.IsThereAnyIntermediateObjectMoving() && goingTo != _sideOfCurrentIntermediateObjectsGoing) return;
        if(CharacterControllers[side].IsCharacterGhostMode) return;

        
        _sideOfCurrentIntermediateObjectsGoing = goingTo;
        _currentIntermediateObjectSpawner = StartCoroutine(CharacterControllers[side].StartTransfer(goingTo));
    }
    
}
