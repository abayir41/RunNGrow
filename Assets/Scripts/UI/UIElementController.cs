using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIElementController : MonoBehaviour
{
    private static GameConfig Config => GameController.Config;
    
    [EnumToggleButtons]
    public UIAnimationTypes types;
    
    [SerializeField] private GameObject parentObj;
    private RectTransform _rectTransform;

    //FadeOutItems
    private List<Image> _fadeOutImages;
    private List<TextMeshProUGUI> _textsFadeOut;
    private List<Button> _buttons;
    
    //FadeLoopsItems
    private List<TextMeshProUGUI> _texts;

    private void Awake()
    {
        _rectTransform = parentObj.GetComponent<RectTransform>();


        _fadeOutImages = parentObj.GetComponentsInChildren<Image>().ToList();
        _buttons = parentObj.GetComponentsInChildren<Button>().ToList();
        _textsFadeOut = parentObj.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        
        
        _texts = parentObj.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        
    }

    private void Start()
    {
        if (types.HasFlag(UIAnimationTypes.FadeLoop))
        {
            
        }
    }

    public void OpenUI()
    {
        parentObj.SetActive(true);
    }
    
    public void CloseUI()
    {
        parentObj.SetActive(false);
    }


    #region Move
    private float _originalX;
    private float _originalY;
        public TweenerCore<Vector3,Vector3,VectorOptions> Move(UIAnimationTypes direction, Action action = null)
    {
        if (!types.HasFlag(direction))
        {
            Debug.LogWarning("Wrong Call!!");
            return null;
        }
        

        if (_originalX == 0)
            _originalX = _rectTransform.position.x;

        if (_originalY == 0)
            _originalY = _rectTransform.position.y;

        switch (direction)
        {
            case UIAnimationTypes.MoveLeft:
                return _rectTransform.DOMoveX(-_rectTransform.rect.width / 2, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
            case UIAnimationTypes.MoveRight:
                return _rectTransform.DOMoveX(Screen.width + _rectTransform.rect.width / 2, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
            case UIAnimationTypes.MoveUp:
                 return _rectTransform.DOMoveY(Screen.height+_rectTransform.rect.height / 2, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
            case UIAnimationTypes.MoveDown:
                return _rectTransform.DOMoveY(-_rectTransform.rect.height / 2, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
            default:
                Debug.Log("Unknown Direction!!");
                return null;
        }
    }
    
    public TweenerCore<Vector3,Vector3,VectorOptions> MoveBackward(UIAnimationTypes direction, Action action = null)
    {

        if (_originalX == 0|| _originalY == 0)
        {
            Debug.LogWarning("Positions seems not cached, It may cause a something unhandled!!");
        }

        switch (direction)
        {
            case UIAnimationTypes.MoveLeft:
                return _rectTransform.DOMoveX(_originalX, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
            case UIAnimationTypes.MoveRight:
                return _rectTransform.DOMoveX(_originalX, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
            case UIAnimationTypes.MoveUp:
                return _rectTransform.DOMoveY(_originalY, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
            case UIAnimationTypes.MoveDown:
                return _rectTransform.DOMoveY(_originalY, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
            default:
                Debug.Log("Unknown Direction!!");
                return null;
        }
    }

    #endregion
    
    #region FadeOut

    private List<float> _originalImgAlphas;
    private List<float> _originalTextAlphas;
    public List<TweenerCore<Color,Color,ColorOptions>> FadeOut(Action action = null)
    {
        var anims = new List<TweenerCore<Color, Color, ColorOptions>>();
        
        if(_originalImgAlphas == null)
            _originalImgAlphas = _fadeOutImages.Select(image => image.color.a).ToList();
        
        if(_originalTextAlphas == null)
            _originalTextAlphas = _textsFadeOut.Select(text => text.color.a).ToList();
        
        _fadeOutImages.ForEach(image =>
        {
            var anim = image.DOFade(0, Config.UIFadeOutAnimationDuration);
            anims.Add(anim);
        });
        
        _textsFadeOut.ForEach(text =>
        {
            var anim = text.DOFade(0, Config.UIFadeOutAnimationDuration);
            anims.Add(anim);
        });

        anims[0].OnComplete(() => action?.Invoke());
        _buttons.ForEach(button => button.interactable = false);
        
        return anims;
    }

    public List<TweenerCore<Color,Color,ColorOptions>> FadeOutBackWard(Action action = null)
    {
        var anims = new List<TweenerCore<Color, Color, ColorOptions>>();
        
        _fadeOutImages.ForEach((image,index) =>
        {
            var anim = image.DOFade(_originalImgAlphas[index], Config.UIFadeOutAnimationDuration);
            anims.Add(anim);
        });
        
        _textsFadeOut.ForEach((text,index) =>
        {
            var anim = text.DOFade(_originalTextAlphas[index], Config.UIFadeOutAnimationDuration);
            anims.Add(anim);
        });

        anims[0].OnComplete(() => action?.Invoke());
        _buttons.ForEach(button => button.interactable = true);
        
        return anims;
    }

    #endregion

    public TweenerCore<Vector3, Vector3, VectorOptions> Minimize()
    {
        return _rectTransform.DOScale(Vector3.zero, Config.UIMinimizeTime);
    }
    
    public TweenerCore<Vector3, Vector3, VectorOptions> NormalSize()
    {
        return _rectTransform.DOScale(Vector3.one, Config.UIMinimizeTime);
    }
    
    public List<TweenerCore<Color, Color, ColorOptions>> StartFadeLoop()
    {
        var anims = new List<TweenerCore<Color, Color, ColorOptions>>();
        
        _texts.ForEach(text =>
        {
            var anim = text.DOFade(0, Config.UIFadeLoopAnimationDuration).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo);
            anims.Add(anim);
        });

        return anims;
    }
}
