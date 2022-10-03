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
                wagon.ExitGrid(RemoveReason.ExitGood);
                
                GameController.Instance.Stats.AddScore(Const.ScoreSoloLocomotiveExit);
            }
            else
            {
                // once locomotive destroyed - the rest of the train will be destroyed with LostLocomotive reason
                wagon.ExitGrid(RemoveReason.ExitBad);
                GameController.Instance.Stats.AddScore(Const.ScorePerWagonExit);
            }
        }
    }
}
