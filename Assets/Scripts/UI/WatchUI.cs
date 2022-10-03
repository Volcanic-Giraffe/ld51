using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WatchUI : MonoBehaviour
{
    public static float Speed = 360f / LevelScenario.SpawnTime;
    
    [SerializeField] private RectTransform arrowRect;

    private bool _paused;

    private bool _doneOnce;

    private void Start()
    {
        _paused = true;

        GameController.Instance.OnModeChanged += Refresh;
    }

    private void Update()
    {
        if (GameController.GameOver || GameController.Paused) return;
        
        if (!_paused)
        {
            var before = arrowRect.localRotation.eulerAngles.z;

            arrowRect.Rotate(Vector3.back, Speed * Time.deltaTime);
            
            var after = arrowRect.localRotation.eulerAngles.z;

            // jump to 360 means passed 00:00
            if (after > before && _doneOnce)
            {
                transform.DOPunchScale(Vector3.one * 0.1f, 0.17f);

                if (LevelScenario.Instance.TrainsLeft <= 1)
                {
                    Pause();
                }
            }

            _doneOnce = true;
        }
    }

    public void Pause()
    {
        _paused = true;
    }

    public void Resume()
    {
        _paused = false;
    }
    
    public void Reset()
    {
        arrowRect.rotation = Quaternion.identity;
    }
    
    private void Refresh()
    {
        if (GameController.Instance.Mode == GameController.GameMode.Build)
        {
            Reset();
            Pause();
        }
        else
        {
            Resume();
        }
    }
}
