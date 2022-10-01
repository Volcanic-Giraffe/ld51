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
            var wagon = other.GetComponentInParent<Wagon>();

            if (wagon.WagonType == WagonType && wagon.IsLastWagon())
            {
                wagon.RemoveFromTrain();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
    }
}
