using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private Image overlay;


    public void Show()
    {
        container.gameObject.SetActive(true);
        
        GameController.Instance.SetPaused(true);
    }
    public void OnAnywhereClicked()
    {
        container.gameObject.SetActive(false);

        GameController.Instance.SetPaused(false);
    }
}
