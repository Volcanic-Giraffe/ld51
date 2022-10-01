using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadingStation : MonoBehaviour
{
    public WagonType WagonType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wagon"))
        {
            var train = other.GetComponentInParent<Wagon>();
            var lastWagon = train.LastWagon();

            if (lastWagon.WagonType == WagonType)
            {
                lastWagon.RemoveFromTrain();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
    }
}
