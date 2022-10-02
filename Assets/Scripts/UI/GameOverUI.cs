using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;
    
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform gameOverRect;
    [SerializeField] private RectTransform tryAgainRect;
    [SerializeField] private RectTransform scoreRect;

    [Space]
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        Instance = this;
        
        container.gameObject.SetActive(false);
    }

    public void Show()
    {
        container.gameObject.SetActive(true);

        scoreText.SetText($"{GameController.Instance.Stats.Score}");
    }

    public void Hide()
    {
        container.gameObject.SetActive(false);
    }

    public void OnTryAgainClicked()
    {
        GameController.Instance.RestartGame();
    }
}
