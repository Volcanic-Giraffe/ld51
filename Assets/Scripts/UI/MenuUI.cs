using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private Image overlay;

    private bool _shown;

    private void Awake()
    {
        _shown = true;
    }

    public void Show()
    {
        _shown = true;
        container.gameObject.SetActive(true);
        
        GameController.Instance.SetPaused(true);
    }

    public void Hide()
    {
        _shown = false;
        container.gameObject.SetActive(false);

        GameController.Instance.SetPaused(false);
    }
    
    public void OnAnywhereClicked()
    {
        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_shown) Hide(); else Show();
        }
    }
}
