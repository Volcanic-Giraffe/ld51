
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// Суть есть место в гриде
public class GridCell : MonoBehaviour
{
    public bool Highlighted;
    public bool Busy;
    public bool Station;

    public bool HasRoad => Road != null;
    public bool Locked { get; private set; }

    public RoadTile Road = null;

    public bool CanPlaceStation => !Locked && !Station && !Busy;
    public int X { get; set; }
    public int Y { get; set; }

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

    public void SetLocked(bool locked)
    {
        Locked = locked;

        // change layer to exclude it from mouse raycast
        gameObject.layer = locked ? LayerMask.NameToLayer("CellLocked") : LayerMask.NameToLayer("Cell");
    }
}
