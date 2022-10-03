using System;
using UnityEngine;

public class DevOnly : MonoBehaviour
{
    private void Awake()
    {
        if (!Application.isEditor)
        {
            Destroy(gameObject);
        }
    }
}
