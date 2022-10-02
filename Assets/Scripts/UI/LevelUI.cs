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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameController.Instance.Stats.OnScoreChange += SetScore;
    }

    private void SetScore(int score)
    {
        scoreText.SetText($"Score: {score}");
        scoreText.transform.DOPunchScale(Vector3.one * 0.2f, 0.18f);
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
