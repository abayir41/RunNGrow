using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapKeeper : MonoBehaviour
{
    public static MapKeeper Instance { get; private set; }

    public List<MapConfig> maps;
    private int _currentMapIndex = -1;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public MapConfig GetMap()
    {
        _currentMapIndex += 1;
        
        if (_currentMapIndex == maps.Count)
            _currentMapIndex = 0;

        return maps[_currentMapIndex];
    }
}
