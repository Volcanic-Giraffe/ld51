using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnloadingStation : MonoBehaviour, IGridElement
{
    public WagonType WagonType;

    public List<Transform> ArtGroups;

    private void Start()
    {
        for (var i = 0; i < ArtGroups.Count; i++)
        {
            var artGroup = ArtGroups[i];
            
            // hide
            artGroup.localPosition += Vector3.back * 30f;
            
            // reveal
            artGroup.DOLocalMove(Vector3.zero, 0.47f).SetEase(Ease.OutSine).SetDelay(i * 0.17f + 0.6f);
        }
    }

    // hardcoded, custom gui needed for editor
    public static Dictionary<WagonType, int[,]> Patterns = new()
    {
        [WagonType.Blue] = new[,] {
            { 1, 0, 1 },
            { 0, 0, 0 },
            { 1, 0, 1 }
        },
        [WagonType.Green] = new[,] {
            { 1, 1, 1 },
            { 0, 0, 0 },
            { 0, 1, 0 }
        },
        [WagonType.Red] = new[,] {
            { 1, 0, 0 },
            { 0, 0, 1 },
            { 1, 0, 1 }
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
                
                GameController.Instance.ShowFlyingScore(transform.position, Const.ScorePerWagonUnloaded);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
    }
}
