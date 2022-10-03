using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    [SerializeField] private Image overlay;
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform gameOverRect;
    [SerializeField] private RectTransform tryAgainRect;
    [SerializeField] private RectTransform scoreRect;

    [Space]
    [SerializeField] private TextMeshProUGUI scoreText;

    private Vector2 _gameOverPos;
    private Vector2 _tryAgainPos;

    private bool _shown;
    
    private void Awake()
    {
        Instance = this;
        
        container.gameObject.SetActive(false);
        
        _gameOverPos = gameOverRect.anchoredPosition;
        _tryAgainPos = tryAgainRect.anchoredPosition;
    }

    public void Show()
    {
        _shown = true;
        container.gameObject.SetActive(true);

        scoreText.SetText("0");
        gameOverRect.gameObject.SetActive(false);
        tryAgainRect.gameObject.SetActive(false);

        StartCoroutine(AnimateIn());
    }

    private void Update()
    {
        if (_shown && Input.GetKeyDown(KeyCode.R))
        {
            OnTryAgainClicked();
        }
    }

    private IEnumerator AnimateIn()
    {
        overlay.color = Color.clear;
        
        gameOverRect.anchoredPosition += Vector2.up * 1000f;
        tryAgainRect.anchoredPosition += Vector2.down * 1000f;
        
        scoreRect.gameObject.SetActive(false);

        overlay.DOFade(0.6f, 0.18f);
        
        yield return new WaitForSeconds(0.7f);

        gameOverRect.gameObject.SetActive(true);
        gameOverRect.DOAnchorPos(_gameOverPos, 0.23f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.7f);
        scoreText.gameObject.SetActive(true);
        scoreText.SetText($"{GameController.Instance.Stats.Score}");
        scoreRect.DOPunchScale(Vector3.one * 0.2f, 0.17f);
        
        yield return new WaitForSeconds(0.7f);
        
        tryAgainRect.gameObject.SetActive(true);
        tryAgainRect.DOAnchorPos(_tryAgainPos, 0.23f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        _shown = false;
        container.gameObject.SetActive(false);
    }

    public void OnTryAgainClicked()
    {
        GameController.Instance.RestartGame();
    }
}
