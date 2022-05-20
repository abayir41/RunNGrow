using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource player;
    public static Action<AudioClip> Play;
    public static bool SoundEnabled => PlayerPrefs.GetInt("Sound") == 0;
    public void OnEnable()
    {
        Play += PlaySound;
    }
   
    private void OnDisable()
    {
        Play -= PlaySound;
    }
   
    private void PlaySound(AudioClip clip)
    {
        if(!SoundEnabled) return;
        
        player.clip = clip;
        player.Play();
            
    }
}
