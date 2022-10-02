
using System;
using UnityEngine;

[ExecuteAlways]
public class Wheel : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up, 5, Space.Self);
    }
}
