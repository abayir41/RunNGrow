using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }
    public static int TotalPoints
    {
        get => PlayerPrefs.GetInt("Money");
        private set => PlayerPrefs.SetInt("Money", value);
    }


    private void Awake()
    {
        Instance = this;
    }

    public void AddPoint(int point)
    {
        TotalPoints += point;
    }
}
