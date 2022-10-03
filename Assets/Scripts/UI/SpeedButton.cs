using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpeedButton : MonoBehaviour
{
    public KeyCode hotkey;
    public Button button; 
    public float SpeedMod;
    public Sprite imgDefault;
    public Sprite imgSelected;

    private List<SpeedButton> _btns;

    private void Awake()
    {
        _btns = FindObjectsOfType<SpeedButton>().ToList();
    }

    public void SetSelected(bool set)
    {
        button.image.sprite = set ? imgSelected : imgDefault;
    }

    private void Update()
    {
        if (Input.GetKeyDown(hotkey))
        {
            OnClicked();
        }
    }

    public void OnClicked()
    {
        foreach (var btn in _btns)
        {
            btn.SetSelected(false);
        }
        
        SetSelected(true);
        Wagon.GlobalSpeedMod = SpeedMod;

    }
}
