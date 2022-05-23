using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource player;
    public static Action<AudioClip> Play;
    
    public static Action<SoundTypes> PlaySpecificSound;
    public static bool SoundEnabled => PlayerPrefs.GetInt("Sound") == 0;

    public AudioClip good;
    public AudioClip wrong;
    public AudioClip lose;
    public AudioClip win;
    public AudioClip tabela;

    public void OnEnable()
    {
        Play += PlaySound;
        PlaySpecificSound += PlaySpecific;
    }

   

    private void OnDisable()
    {
        Play -= PlaySound;
        PlaySpecificSound -= PlaySpecific;
    }
   
    
    private void PlaySpecific(SoundTypes obj)
    {
        if(!SoundEnabled) return;

        AudioClip clip;
        
        switch (obj)
        {
            case SoundTypes.Good:
                clip = good;
                break;
            case SoundTypes.Wrong:
                clip = wrong;
                break;
            case SoundTypes.Lose:
                clip = lose;
                break;
            case SoundTypes.Win:
                clip = win;
                break;
            case SoundTypes.Tabela:
                clip = tabela;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
        }

        player.clip = clip;
        player.Play();
    }
    
    private void PlaySound(AudioClip clip)
    {
        if(!SoundEnabled) return;
        
        player.clip = clip;
        player.Play();
            
    }
}

public enum SoundTypes
{
    Good,
    Wrong,
    Lose,
    Win,
    Tabela
}