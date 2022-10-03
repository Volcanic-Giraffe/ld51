using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TweenMove : MonoBehaviour
{
    public Vector2 AnchoredOffset;

    public float duration;
    public float delay;

    private void Awake()
    {
        if (AnchoredOffset != Vector2.zero)
        {
            var rect = GetComponent<RectTransform>();

            rect.DOAnchorPos(rect.anchoredPosition + AnchoredOffset, duration)
                .SetEase(Ease.InOutSine)
                .SetDelay(delay)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
