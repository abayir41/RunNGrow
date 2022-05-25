using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    private static GameConfig Config => GameController.Config;
    
    [SerializeField] private TextMeshProUGUI gameScoreText;
    [SerializeField] private UIElementController gameScorePart;
    [SerializeField] private TextMeshProUGUI coinText;
    
    [SerializeField] private UIElementController touchToStart;

    
    [SerializeField] private UIElementController skinUnlockScreenParent;
    [SerializeField] private UIElementController skinUnlockBackground;
    [SerializeField] private UIElementController skinUnlockImage;
    [SerializeField] private UIElementController skinUnlockedText;
    [SerializeField] private UIElementController skinBackEffect;
    [SerializeField] private UIElementController skinCover;

    [SerializeField] private UIElementController deathScreen;

    private List<TweenerCore<Color, Color, ColorOptions>> _touchToStartLoopAnim;

    [SerializeField] private Image unlockSkinFrontImage;
    [SerializeField] private Image unlockSkinBackImage;
    [SerializeField] private RectTransform holdDragIcon;
    [SerializeField] private UIElementController holdAndDrag;
    [SerializeField] private List<RectTransform> holdAndDragBalls;

    [SerializeField] private UIElementController skinSelectOpenButton;
    [SerializeField] private UIElementController skinSelectMenu;
    [SerializeField] private UIElementController settingsMenu;
    
 
    [SerializeField] private UIElementController settingsButton;
    public GameObject levelText;

    [SerializeField] private UIElementController bossEndScreenContinue;

    [SerializeField] private Image soundImage;
    [SerializeField] private Image vibrationImage;
    [SerializeField] private Sprite soundEnable;
    [SerializeField] private Sprite soundDisable;
    [SerializeField] private Sprite vibrationEnable;
    [SerializeField] private Sprite vibrationDisable;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameScorePart.CloseUI();
        _touchToStartLoopAnim = touchToStart.StartFadeLoop();
        
        skinUnlockScreenParent.CloseUI();
        skinUnlockBackground.FadeOut().ForEach(core => core.Complete());
        skinUnlockImage.Minimize().Complete();
        skinUnlockedText.Minimize().Complete();
        skinBackEffect.Minimize().Complete();
        skinCover.Minimize().Complete();
        
        deathScreen.CloseUI();

        bossEndScreenContinue.FadeOut();

        vibrationImage.sprite = PlayerPrefs.GetInt("Vibration") == 0 ? vibrationEnable : vibrationDisable;
        soundImage.sprite = PlayerPrefs.GetInt("Sound") == 0 ? soundEnable : soundDisable;
        
        skinSelectMenu.CloseUI();

        holdAndDragBalls.ForEach((rectTransform, i) =>
        {
            rectTransform.DOMoveX(Screen.width / 3 * 2, 1).SetDelay(i * 0.07f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            rectTransform.DOMoveY(rectTransform.position.y + 33, 0.5f).SetDelay(i * 0.2f).SetEase(Ease.Linear).OnComplete(() => 
                rectTransform.DOMoveY(rectTransform.position.y, 0.5f).SetEase(Ease.Linear)).SetLoops(-1,LoopType.Yoyo);
        });
        holdDragIcon.DOMoveX(Screen.width / 3 * 2, 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnEnable()
    {
        GameActions.GameStarted += StartTheGameUI;
    }

    private void OnDisable()
    {
        GameActions.GameStarted -= StartTheGameUI;
    }

    public void ToggleVibration()
    {
        PlayerPrefs.SetInt("Vibration",PlayerPrefs.GetInt("Vibration") == 0 ? 1 : 0);
        if(VibrationManager.VibrationEnabled) VibrationManager.Vibrate?.Invoke();
        vibrationImage.sprite = vibrationImage.sprite == vibrationEnable ? vibrationDisable : vibrationEnable;
    }

    public void ToggleSound()
    {
        PlayerPrefs.SetInt("Sound",PlayerPrefs.GetInt("Sound") == 0 ? 1 : 0);
        soundImage.sprite = soundImage.sprite == soundEnable ? soundDisable : soundEnable;
    }
    
    #region GameMethods

    public void StartGame()
    {
        _touchToStartLoopAnim.ForEach(core => core.Kill());
        touchToStart.CloseUI();
        holdAndDrag.CloseUI();
        GameActions.GameStarted?.Invoke();
        
    }

    public void SetGameScoreText(int value)
    {
        gameScoreText.text = "+" + value;
    }

    public void SetCoinText(int value)
    {
        coinText.text = value.ToString();
    }

    public void OpenGameScorePart()
    {
        gameScorePart.OpenUI();
    }

    public void OpenDeathScreen()
    {
        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        
        deathScreen.OpenUI();
        deathScreen.StartFadeLoop();  
    }
    
    //TODO: DeathSCreen

    public void Restart()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void OpenSkinSelectMenu()
    {
        skinSelectOpenButton.CloseUI();
        touchToStart.CloseUI();
        skinSelectMenu.OpenUI();
    }

    public void OpenBossEndContinue()
    {
        bossEndScreenContinue.FadeOutBackWard();
    }
    
    public void CloseSkinSelectMenu()
    {
        skinSelectOpenButton.OpenUI();
        touchToStart.OpenUI();
        skinSelectMenu.CloseUI();
    }

    public void StartTheGameUI()
    {
        skinSelectOpenButton.CloseUI();
        settingsButton.CloseUI();
        levelText.SetActive(true);
        var curLevel = PlayerPrefs.GetInt("total_level") + 1;
        levelText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            "Level " + curLevel;
    }

    public void OpenSettings()
    {
        skinSelectOpenButton.CloseUI();
        settingsButton.CloseUI();
        touchToStart.CloseUI();
        settingsMenu.OpenUI();
    }
    
    public void CloseSettings()
    {
        //skinSelectOpenButton.OpenUI();
        settingsButton.OpenUI();
        touchToStart.OpenUI();
        settingsMenu.CloseUI();
    }
    
    public IEnumerator GetSkinUnlockScreen(float from, float to, bool skinUnlocked)
    {
        unlockSkinBackImage.sprite = SkinSystem.Instance.CurrentSkinImage();
        unlockSkinFrontImage.sprite = SkinSystem.Instance.CurrentSkinImage();
        
        unlockSkinFrontImage.fillAmount = 1 - from;
        
        skinUnlockScreenParent.OpenUI();
        skinUnlockBackground.FadeOutBackWard();
        var anim = skinUnlockImage.NormalSize();
        skinCover.NormalSize();

        while (anim.IsPlaying())
        {
            yield return null;
        }

        
        var anim2 = DOTween.To(() => from, value =>
        {
            from = value;
            unlockSkinFrontImage.fillAmount = 1 - value;
        }, to, Config.DurationOfSkinUnlockAnimation);

        while (anim2.IsPlaying())
        {
            yield return null;
        }

        if (skinUnlocked)
        {
            skinUnlockedText.NormalSize();
            skinBackEffect.NormalSize();
        }
        
    }

    
    
    #endregion
}
