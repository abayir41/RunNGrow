using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMethods : MonoBehaviour
{
    [SerializeField] private UIElementController touchToStart;
    
    public void StartGame()
    {
        touchToStart.FadeOut(() =>
        {
            touchToStart.CloseUI();
            GameActions.GameStarted?.Invoke();
        });
    }
}
