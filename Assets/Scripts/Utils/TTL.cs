using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTL : MonoBehaviour
{
    public float ttl = 3f;

    private void Awake()
    {
        if (ttl > 0)
        {
            Destroy(gameObject, ttl);
        }
    }
}
