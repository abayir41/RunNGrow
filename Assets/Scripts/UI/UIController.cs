using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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

    [SerializeField] private UIElementController endTouchToContinue;
    [SerializeField] private UIElementController settings;
    
    private List<TweenerCore<Color, Color, ColorOptions>> _touchToStartLoopAnim;

    [SerializeField] private Image unlockSkinFrontImage;
    [SerializeField] private Image unlockSkinBackImage;

    [SerializeField] private UIElementController skinSelectOpenButton;
    [SerializeField] private UIElementController skinSelectMenu;
    [SerializeField] private UIElementController settingsMenu;


    [SerializeField] private UIElementController settingsButton;
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
        
        endTouchToContinue.CloseUI();
        endTouchToContinue.StartFadeLoop();
        
        skinSelectMenu.CloseUI();
    }

    private void OnEnable()
    {
        GameActions.GameStarted += StartTheGameUI;
    }

    private void OnDisable()
    {
        GameActions.GameStarted -= StartTheGameUI;
    }

    #region GameMethods

    public void StartGame()
    {
        _touchToStartLoopAnim.ForEach(core => core.Kill());
        touchToStart.FadeOut(() =>
        {
            GameActions.GameStarted?.Invoke();
        });
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

    public void OpenEndTouchToContinue()
    {
        endTouchToContinue.OpenUI();
        endTouchToContinue.StartFadeLoop();   
    }

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
        skinSelectOpenButton.OpenUI();
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
        
        OpenEndTouchToContinue();
    }

    #endregion
}
