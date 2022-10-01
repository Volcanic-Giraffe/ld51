
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// Суть есть место в гриде
public class GridCell : MonoBehaviour
{
    public bool Highlighted;
    public bool Busy;

    public IGridElement Element;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        var tileElement = other.GetComponent<IGridElement>();
        if (tileElement != null)
        {
            Busy = true;
            Element = tileElement;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var tileElement = other.GetComponent<IGridElement>();
        if (tileElement != null)
        {
            Busy = false;
            Element = null;
        }

    }

}
