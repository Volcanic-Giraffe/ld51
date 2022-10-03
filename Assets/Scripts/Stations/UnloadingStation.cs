using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadingStation : MonoBehaviour, IGridElement
{
    public WagonType WagonType;

    // hardcoded, custom gui needed for editor
    public static Dictionary<WagonType, int[,]> Patterns = new()
    {
        [WagonType.Blue] = new[,] {
            { 1, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 }
        },
        [WagonType.Green] = new[,] {
            { 1, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 }
        },
        [WagonType.Red] = new[,] {
            { 1, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 }
        },
    };

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wagon"))
        {
            var wagon = other.GetComponentInParent<Wagon>();

            if (wagon.WagonType == WagonType && wagon.IsLastWagon())
            {
                wagon.RemoveFromTrain(RemoveReason.Station);
                
                GameController.Instance.Stats.AddScore(Const.ScorePerWagonUnloaded);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
    }
}
