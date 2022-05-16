using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    
    #region Controllers

    private IntermediateObjectController IOCInstance => IntermediateObjectController.Instance;
    private ObstacleController OController => ObstacleController.Instance;

    private UIController UIControl => UIController.Instance;

    #endregion
    
    #region Character

    private GameObject Boss => boss;
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
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraSideView;

    private Vector3 LeftSideVec { get; set; }
    private Vector3 RightSideVec { get; set; }
    private Vector3 MiddleSideVec { get; set; }

    private Coroutine _currentIntermediateObjectSpawner;
    private Side _sideOfCurrentIntermediateObjectsGoing;
    
    private bool IsAnyIntermediateObjAlive => IOCInstance.IsThereAnyIntermediateObjectMoving();
    private bool _gameFinishAnimationStarted;
    private bool _gameFailed;
    
    //Animations
    private static readonly int RunBack = Animator.StringToHash("RunBack");
    private static readonly int SpeedOfThrow = Animator.StringToHash("SpeedOfThrow");
    private static readonly int Die = Animator.StringToHash("Die");


    [HorizontalGroup("Base")] [BoxGroup("Base/Left")]
    [SerializeField] private List<GameObject> multiplierBlocks;
    [BoxGroup("Base/Right")]
    [SerializeField] private List<float> multiplyAmount;
    
    private float _speedOfFinalPart;
    private float _gameTotalPoints;
    private bool _bossHitTheWall;
    private bool _gameStarted;

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
        BossTransform = Boss.GetComponent<Transform>();
        _speedOfFinalPart = Config.SpeedOfFinalPart;

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


        //painting and text setting
        var step = 0.0f;
        var matBlock = new MaterialPropertyBlock();
        for (var i = 0; i < multiplierBlocks.Count; i++)
        {
            var multiplierBlock = multiplierBlocks[i];
            var color = Color.HSVToRGB(step, 1, 1);
            matBlock.SetColor("_Color", color);
            multiplierBlock.GetComponent<Renderer>().SetPropertyBlock(matBlock);
            step += 1.0f / multiplierBlocks.Count;

            //Make one decimal
            multiplierBlock.GetComponentInChildren<TextMeshPro>().text = $"{multiplyAmount[i]:0.0}" + "X";
        }
        
        //middleChar Stop
        CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.speed = 0);
        
        //Spawn Obstacles
        var map = MapKeeper.Instance.GetMap();
        OController.SpawnMapObstacles(map);

        //set their position
        switch (MapAlongAxis)
        {
            case Axis.X:
                finalPlatform.position = new Vector3(OController.LastObstaclePos.x + Config.DistanceBetweenFinalAndLastObstacle, MiddleSideVec.y, MiddleSideVec.z);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        DOTween.SetTweensCapacity(1000,50);
        
        UIControl.SetCoinText(MoneySystem.TotalPoints);
    }

    private void OnEnable()
    {
        GameActions.BossHitTheMax += BossHitTheMax;
        GameActions.GameStarted += GameStarted;
    }

    

    private void OnDisable()
    {
        GameActions.BossHitTheMax -= BossHitTheMax;
        GameActions.GameStarted -= GameStarted;
    }
    
    private void BossHitTheMax()
    {
        CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.speed = 0);
        _bossHitTheWall = true;
    }
    
    private void GameStarted()
    {
        _gameStarted = true;
    }

    void Update()
    {
        
        if(!_gameStarted) return;

        //Move Final Platform
        var along = MapAlongAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };

        finalPlatform.position -= along * (_speedOfFinalPart * Time.deltaTime);
        
        
        //Check if two char died
        if (LeftAndRightCanShrink.All(pair => pair.Value) && !_gameFinishAnimationStarted && !IsAnyIntermediateObjAlive)
        {
            _gameFailed = true;
            GameActions.GameFailed?.Invoke();
        }
    
        //check if the game was finished, if it is, make some steps
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

                    _speedOfFinalPart = 0;
                    
                    StartCoroutine(StartEndAnimation());
                    GameActions.GameFinishAnimationStarted?.Invoke();
                }
                break;
            default:
                throw new NotImplementedException();
        }
        
        
        //Screen Touch Control
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
                if(_currentIntermediateObjectSpawner != null)
                    StopCoroutine(_currentIntermediateObjectSpawner);
            }
        }
        
    }


    private IEnumerator StartEndAnimation()
    {
        
        //Wait for any intermediate obj
        while (IsAnyIntermediateObjAlive)
        {
            yield return null;
        }

        //wait for growin animation
        yield return new WaitForSeconds(Config.DurationOfAnimation + 1.0f);
        
        //Camera Movement
        bool cameraMoved = false;
        cameraTransform.DOMove(cameraSideView.position, Config.CameraAnimDuration).OnComplete(() => cameraMoved = true);
        cameraTransform.DORotateQuaternion(cameraSideView.rotation, Config.CameraAnimDuration);

        while (!cameraMoved)
        {
            yield return null;
        }

        //CalculateGame Points
        _gameTotalPoints += CharacterControllers[Side.Left].PointOfChar.x +
                           CharacterControllers[Side.Left].PointOfChar.y;
        _gameTotalPoints += CharacterControllers[Side.Right].PointOfChar.x +
                            CharacterControllers[Side.Right].PointOfChar.y;
        
        
        //Send points to middle char
        StartCoroutine(CharacterControllers[Side.Left].StartTransfer(Side.Middle));
        StartCoroutine(CharacterControllers[Side.Right].StartTransfer(Side.Middle));

        //wait for intermedie objs
        while (LeftAndRightCanShrink.Any(pair => pair.Value != true) || IsAnyIntermediateObjAlive)
        {
            yield return null;
        }
        
        //Start Boss throw balls
        CharactersDict[Side.Middle].GetComponentsInChildren<Animator>().ForEach(animator => animator.speed = 1);
        CharactersDict[Side.Boss].GetComponentsInChildren<Animator>()
            .ForEach(animator => animator.SetBool(RunBack, true));
        
        _speedOfFinalPart = Config.SpeedOfFinalPart;
        CharacterTransforms[Side.Boss].parent = null;
        GameActions.BossRunningStarted?.Invoke();

        //set throwing ball speed while throwing
        while ((!CharacterControllers[Side.Middle].IsCharacterGhostMode || IsAnyIntermediateObjAlive) && !_bossHitTheWall)
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
        
        //Boss Die
        CharactersDict[Side.Boss].GetComponentsInChildren<Animator>()
            .ForEach(animator => animator.SetBool(Die, true));

        //stop the movement of last part
        _speedOfFinalPart = 0;
        
        //duration of Die
        yield return new WaitForSeconds(2.3f);

        var bossTrans = boss.transform.position;
        var multiplierBlock = multiplierBlocks.OrderBy(o => Vector3.Distance(bossTrans, o.transform.position)).First();
        var multiplier = multiplyAmount[multiplierBlocks.IndexOf(multiplierBlock)];
        
        var addPoint = (int) (_gameTotalPoints * multiplier);
        
        
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

        if (SkinSystem.Instance.IsThereAnyNewSkin())
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
        
        ScoreSystem.Instance.AddPoint(addPoint);
        MoneySystem.Instance.AddPoint(addPoint);
        

        GameActions.GameEndedWithWinning?.Invoke();
    }

    public void MiddleCharSendOneIntermediate()
    {
        //the parametre Side Middle Doesnt important
        CharacterControllers[Side.Middle].TransferOne(Side.Boss);
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
