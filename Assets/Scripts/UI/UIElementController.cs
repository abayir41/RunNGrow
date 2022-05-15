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

    private TweenerCore<Vector3, Vector3, VectorOptions> _moveLeftBackwards;
    private TweenerCore<Vector3, Vector3, VectorOptions> _moveRightBackwards;
    private TweenerCore<Vector3, Vector3, VectorOptions> _moveDownBackwards;
    private TweenerCore<Vector3, Vector3, VectorOptions> _moveUpBackwards;
    
    //FadeOutItems
    private List<Image> _fadeOutImages;
    private List<TextMeshProUGUI> _textsFadeOut;
    private List<Button> _buttons;
    private List<TweenerCore<Color, Color, ColorOptions>> _fadeOutImagesBackwards;
    private List<TweenerCore<Color, Color, ColorOptions>> _fadeOutTextsBackwards;

    
    //FadeLoopsItems
    private List<TextMeshProUGUI> _texts;

    private void Awake()
    {
        _rectTransform = parentObj.GetComponent<RectTransform>();

        if (types.HasFlag(UIAnimationTypes.FadeOut))
        {
            _fadeOutImages = parentObj.GetComponentsInChildren<Image>().ToList();
            _buttons = parentObj.GetComponentsInChildren<Button>().ToList();
            _textsFadeOut = parentObj.GetComponentsInChildren<TextMeshProUGUI>().ToList();
            _fadeOutImagesBackwards = new List<TweenerCore<Color, Color, ColorOptions>>();
            _fadeOutTextsBackwards = new List<TweenerCore<Color, Color, ColorOptions>>();
        }

        if (types.HasFlag(UIAnimationTypes.FadeLoop))
        {
            _texts = parentObj.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        }
    }

    private void Start()
    {
        if (types.HasFlag(UIAnimationTypes.FadeLoop))
        {
            _texts.ForEach(text =>
                text.DOFade(0, Config.UIFadeLoopAnimationDuration).SetEase(Ease.InCirc).SetLoops(-1, LoopType.Yoyo));
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
    
    public void MoveLeft(Action action = null)
    {
        if (!types.HasFlag(UIAnimationTypes.MoveLeft))
        {
            Debug.LogWarning("Wrong Call!!: MoveLeft");
            return;
        }

        if (_moveLeftBackwards != null)
        {
            Debug.LogError("Already Moved Left");
            return;
        }
        
        var posX = _rectTransform.position.x;
        _rectTransform.DOMoveX(-_rectTransform.rect.width / 2, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
        _moveLeftBackwards = _rectTransform.DOMoveX(posX, Config.UIMoveAnimationDuration).Pause().OnKill(() => _moveLeftBackwards = null);
    }

    public void MoveLeftBackWard(Action action = null)
    {
        if (_moveLeftBackwards == null)
        {
            Debug.LogError("Not Moved Left");
            return;
        }

        _moveLeftBackwards.Play().OnComplete(() => action?.Invoke());
    }
    
    public void MoveRight(Action action = null)
    {
        if (!types.HasFlag(UIAnimationTypes.MoveRight))
        {
            Debug.Log("Wrong Call!!: MoveRight");
            return;
        }
        
        if (_moveRightBackwards != null)
        {
            Debug.LogError("Already Moved Right");
            return;
        }
        
        var posX = _rectTransform.position.x;
        _rectTransform.DOMoveX(Screen.width + _rectTransform.rect.width / 2, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
        _moveRightBackwards = _rectTransform.DOMoveX(posX, Config.UIMoveAnimationDuration).Pause().OnKill(() => _moveRightBackwards = null);
    }
    
    public void MoveRightBackWard(Action action = null)
    {
        if (_moveRightBackwards == null)
        {
            Debug.LogError("Not Moved Right");
            return;
        }

        _moveRightBackwards.Play().OnComplete(() => action?.Invoke());
    }
    
    public void MoveDown(Action action = null)
    {
        if (!types.HasFlag(UIAnimationTypes.MoveDown))
        {
            Debug.Log("Wrong Call!!: MoveDown");
            return;
        }
        
        if (_moveDownBackwards != null)
        {
            Debug.LogError("Already Moved Down");
            return;
        }
        
        var posY = _rectTransform.position.y;
        _rectTransform.DOMoveY(-_rectTransform.rect.height / 2, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
        _moveDownBackwards = _rectTransform.DOMoveY(posY, Config.UIMoveAnimationDuration).Pause().OnKill(() => _moveDownBackwards = null);
    }
    
    public void MoveDownBackWard(Action action = null)
    {
        if (_moveDownBackwards == null)
        {
            Debug.LogError("Not Moved Down");
            return;
        }

        _moveDownBackwards.Play().OnComplete(() => action?.Invoke());
    }
    
    public void MoveUp(Action action = null)
    {
        if (!types.HasFlag(UIAnimationTypes.MoveUp))
        {
            Debug.Log("Wrong Call!!: MoveUp");
            return;
        }
        
        if (_moveUpBackwards != null)
        {
            Debug.LogError("Already Moved Up");
            return;
        }
        
        var posY = _rectTransform.position.y;
        _rectTransform.DOMoveY(Screen.height+_rectTransform.rect.height / 2, Config.UIMoveAnimationDuration).OnComplete(() => action?.Invoke());
        _moveUpBackwards = _rectTransform.DOMoveY(posY, Config.UIMoveAnimationDuration).Pause().OnKill(() => _moveUpBackwards = null);
    }
    
    public void MoveUpBackWard(Action action = null)
    {
        if (_moveUpBackwards == null)
        {
            Debug.LogError("Not Moved Right");
            return;
        }

        _moveUpBackwards.Play().OnComplete(() => action?.Invoke());
    }

    public void FadeOut(Action action = null)
    {
        if (_fadeOutImagesBackwards.Count > 0)
        {
            Debug.LogError("Already Faded Out");
            return;
        }

        _buttons.ForEach(button => button.interactable = false);
        
        var alphasImages = _fadeOutImages.Select(image => image.color.a).ToList();
        _fadeOutImages.ForEach(image => image.DOFade(0,Config.UIFadeOutAnimationDuration));

        for (var i = 0; i < _fadeOutImages.Count; i++)
        {
            var anim = _fadeOutImages[i].DOFade(alphasImages[i], Config.UIFadeOutAnimationDuration).Pause();
            _fadeOutImagesBackwards.Add(anim);
            anim.OnKill(() => _fadeOutImagesBackwards.Remove(anim));
        }

        
        var alphasTexts = _textsFadeOut.Select(text => text.color.a).ToList();
        _textsFadeOut.ForEach(text => text.DOFade(0,Config.UIFadeOutAnimationDuration).OnComplete(() => action?.Invoke()));

        for (var i = 0; i < _textsFadeOut.Count; i++)
        {
            var anim = _textsFadeOut[i].DOFade(alphasTexts[i], Config.UIFadeOutAnimationDuration).Pause();
            _fadeOutTextsBackwards.Add(anim);
            anim.OnKill(() => _fadeOutTextsBackwards.Remove(anim));
        }
    }

    public void FadeOutBackWard(Action action = null)
    {
        if (_fadeOutImagesBackwards.Count == 0)
        {
            Debug.LogError("Not Faded Out");
            return;
        }
        
        _buttons.ForEach(button => button.interactable = true);
        _fadeOutImagesBackwards.ForEach(core => core.Play());
        _fadeOutImagesBackwards[0].OnComplete(() => action?.Invoke());
    }
}
