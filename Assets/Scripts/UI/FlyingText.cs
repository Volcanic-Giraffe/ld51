using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FlyingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private Color colorGood = new Color(0, 1, 0);
    private Color colorBad = new Color(1, 0, 0);
    
    public void Show(int score)
    {
        var good = score > 0;
        
        var text = (good ? "+" : "") + score;

        label.color = good ? colorGood : colorBad;

        label.SetText(text);

        label.rectTransform.DOAnchorPos(Vector2.up * 1, 0.6f).SetEase(Ease.OutQuart);
        label.DOColor(Color.clear, 0.3f).SetDelay(0.6f);
    }
}
