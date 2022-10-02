using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    public static LevelUI Instance;
    
    [SerializeField] private TextMeshProUGUI runButtonText;
    
    private void Awake()
    {
        Instance = this;
    }

    public void OnRunClicked()
    {
        GameController.Instance.OnRun();

        runButtonText.SetText(GameController.Instance.Mode == GameController.GameMode.Build ? "Run" : "Stop");
    }
}
