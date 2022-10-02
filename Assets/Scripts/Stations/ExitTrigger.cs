using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wagon"))
        {
            var wagon = other.GetComponentInParent<Wagon>();

            if (wagon.WagonType == WagonType.Locomotive && wagon.TrainSize() == 1)
            {
                wagon.RemoveFromTrain(RemoveReason.ExitGood);
                
                GameController.Instance.Stats.AddScore(Const.ScoreSoloLocomotiveExit);
            }
            else
            {
                wagon.RemoveFromTrain(RemoveReason.ExitBad);
                GameController.Instance.Stats.AddScore( Const.ScorePerWagonExit);
            }
        }
    }
}
