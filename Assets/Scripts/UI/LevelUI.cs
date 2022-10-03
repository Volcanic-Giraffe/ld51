using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public static LevelUI Instance;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI trainsText;

    [Space]
    [SerializeField] private RectTransform indicatorRun;
    [SerializeField] private RectTransform indicatorBuild;
    
    [Space]
    [SerializeField] private Button buttonRun;
    [SerializeField] private Button buttonBuild;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameController.Instance.Stats.OnScoreChange += SetScore;
        GameController.Instance.Stats.OnLivesChange += SetLives;

        LevelScenario.Instance.OnWaveBegin += SetWave;
        LevelScenario.Instance.OnEveryTenSeconds += SetTrains;

        GameController.Instance.OnModeChanged += SetWave;
        
        SetWave();
    }

    private void SetWave()
    {
        waveText.SetText($"Wave: {LevelScenario.Instance.WaveIndex + 1}");
        
        if (GameController.Instance.Mode == GameController.GameMode.Build)
        {
            buttonRun.gameObject.SetActive(true);
            buttonBuild.gameObject.SetActive(false);
            
            indicatorRun.gameObject.SetActive(false);
            indicatorBuild.gameObject.SetActive(true);
        }
        else
        {
            buttonRun.gameObject.SetActive(false);
            buttonBuild.gameObject.SetActive(true);
            
            indicatorRun.gameObject.SetActive(true);
            indicatorBuild.gameObject.SetActive(false);
        }
        
        SetTrains();
    }

    private void SetTrains()
    {
        var newVal = $"{LevelScenario.Instance.TrainsLeft}";

        if (newVal != trainsText.text)
        {
            trainsText.SetText(newVal);
            // too much
            // trainsText.transform.DOPunchScale(Vector3.one * 0.2f, 0.18f).SetDelay(0.34f);
        }
    }
    
    private void SetScore(int score, int change)
    {
        scoreText.SetText($"Score: {score}");
        
        if (change < 0)
        {
            scoreText.transform.DOPunchScale(-Vector3.one * 0.2f, 0.18f);
        }
        else
        {
            scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.18f);
        }
    }

    private void SetLives(int lives, int change)
    {
        livesText.SetText($"{lives}");
        
        livesText.transform.DOPunchScale(Vector3.one * 0.4f, 0.21f);
    }

    public void OnRunClicked()
    {
        GameController.Instance.OnRun();
    }
    
    public void OnBuildClicked()
    {
        // GameController.Instance.OnBuild();
        
        LevelScenario.Instance.RestartWave();
    }

    public void OnSkipClicked()
    {
        LevelScenario.Instance.UpdateWave();

        SetWave();
    }

    public void DevFailLevel()
    {
        GameController.Instance.FailGame();
    }
}
