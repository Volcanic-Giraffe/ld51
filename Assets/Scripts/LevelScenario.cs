using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScenario : MonoBehaviour
{
    public static float SpawnTime = 10f;
    
    public static LevelScenario Instance;
    
    [SerializeField] private List<WaveConfig> waves;

    public WaveConfig Wave { get; private set; }
    public int WaveIndex { get; private set; }
    
    public int TrainNumber { get; private set; }
    public int TrainsTotal => Wave.Repeats;

    public int TrainsLeft => Wave.Repeats - TrainNumber;

    public event Action OnWaveBegin;
    public event Action OnEveryTenSeconds;


    private float _timer;
    
    private void Awake()
    {
        Instance = this;
        
        WaveIndex = 0;
        Wave = waves[0];
    }

    void Start()
    {
        
    }

    private void Update()
    {
        if (GameController.Instance.Mode == GameController.GameMode.Build) return;
        
        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _timer = SpawnTime;

            UpdateWave();
        }
    }

    public void UpdateWave()
    {
        TrainNumber += 1;

        if (TrainNumber > Wave.Repeats)
        {
            WaveIndex += 1;

            Wave = waves[Math.Min(WaveIndex, waves.Count - 1)];
            TrainNumber = 0;

            _timer = SpawnTime * 0.1f;
            
            OnWaveBegin?.Invoke();
        }
        else
        {
            OnEveryTenSeconds?.Invoke();
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