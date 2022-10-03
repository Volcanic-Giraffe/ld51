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

    private List<Wagon> _wagons = new();

    private void Awake()
    {
        Instance = this;
        
        WaveIndex = 0;
        Wave = waves[0];
    }

    void Start()
    {
        GameController.Instance.OnModeChanged += OnModeChange;

        GameController.Instance.Stats.OnLivesChange += OnLivesChange;
    }

    private void OnModeChange()
    {
        _timer = 0;
    }

    private void OnLivesChange(int current, int change)
    {
        if (current <= 0)
        {
            GameController.Instance.FailGame();
        }
    }
    
    private void Update()
    {
        if (GameController.GameOver) return;
        if (GameController.Instance != null && GameController.Instance.Mode == GameController.GameMode.Build) return;

        if (TrainsLeft == 0)
        {
            if (_wagons.Count == 0)
            {
                UpdateWave();
            }
            
            return;
        }
        
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

            OnWaveBegin?.Invoke();
        }
        else
        {
            OnEveryTenSeconds?.Invoke();
        }
    }

    public void RestartWave()
    {
        Wave = waves[Math.Min(WaveIndex, waves.Count - 1)];
        TrainNumber = 0;

        OnWaveBegin?.Invoke();
    }
    
    public void AddWagon(Wagon wagon)
    {
        _wagons.Add(wagon);
    }
    
    public void RemoveWagon(Wagon wagon)
    {
        _wagons.Remove(wagon);
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