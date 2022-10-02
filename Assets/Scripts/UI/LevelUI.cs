using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    public static LevelUI Instance;
    
    [SerializeField] private TextMeshProUGUI runButtonText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameController.Instance.Stats.OnScoreChange += SetScore;
        GameController.Instance.Stats.OnLivesChange += SetLives;
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
        livesText.SetText($"Lives: {lives}");
        
        livesText.transform.DOPunchScale(Vector3.one * 0.4f, 0.21f);
    }

    public void OnRunClicked()
    {
        GameController.Instance.OnRun();

        runButtonText.SetText(GameController.Instance.Mode == GameController.GameMode.Build ? "Run" : "Stop");
    }

    private void Update()
    {
        
    }
}
