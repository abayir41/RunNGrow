using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    // Start is called before the first frame update
    public static MoneySystem Instance { get; private set; }
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
