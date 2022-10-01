using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    public static LevelUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void OnRunClicked()
    {
        GameController.Instance.OnRun();
    }
}
