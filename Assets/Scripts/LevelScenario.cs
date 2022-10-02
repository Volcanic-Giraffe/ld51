using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScenario : MonoBehaviour
{
    public static LevelScenario Instance;
    
    [SerializeField] private List<WaveConfig> waves;

    public WaveConfig Wave { get; private set; }
    public int WaveIndex { get; private set; }
    
    public int WaveRepeat { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        WaveIndex = 0;
        Wave = waves[0];
    }

    void Start()
    {
        GameController.Instance.OnEveryTenSeconds += UpdateWave;
    }

    public void UpdateWave()
    {
        WaveRepeat += 1;

        if (WaveRepeat > Wave.Repeats)
        {
            WaveIndex += 1;

            Wave = waves[Math.Min(WaveIndex, waves.Count - 1)];
            WaveRepeat = 0;
        }
    }
    
}

[Serializable]
public class WaveConfig
{
    public int Repeats;
    public int TrainsLengthMin;
    public int TrainsLengthMax;
    public int Stations;
}