
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// Суть есть место в гриде
public class GridCell : MonoBehaviour
{
    public bool Highlighted;
    public bool Busy;

    public bool HasRoad => Road != null;

    public RoadTile Road = null;

    public IGridElement Element;
    private void OnTriggerEnter(Collider other)
    {
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

    private void Update()
    {
        if (HasRoad)
        {
            Road.transform.position = transform.position;
        }
    }
}
