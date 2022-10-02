
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MoonGrid : MonoBehaviour
{

    public static MoonGrid Instance;

    public int Width;
    public int Height;

    public GridCell GridCellPrefab;
    public GridCell[,] Cells => _cells;

    private void Awake()
    {
        Instance = this;
        _camera = Camera.main;
        
        _cells = new GridCell[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var tile = Instantiate(GridCellPrefab, transform);
                tile.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, 0);
                _cells[x, y] = tile;
            }
        }
    }

    private GridCell[,] _cells;
    private Camera _camera;

    public Vector2Int EnterPoint;

    private void Start()
    {

    }

    public Vector2Int XY(Vector3 vector)
    {
        return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));        
    }

    public Vector2Int XY(GameObject go)
    {
        return XY(go.transform);
    }
    
    public Vector2Int XY(MonoBehaviour go)
    {
        return XY(go.transform);
    }

    public Vector2Int XY(Transform transform)
    {
        return XY(transform.position);
    }

    public Vector3 CenterOfTile(Vector2Int xy)
    {
        return new Vector3(xy.x + 0.5f, xy.y + 0.5f, 0);
    }

    [CanBeNull]
    public GridCell GetCell(Vector2Int xy)
    {
        if (xy.x < 0 || xy.y < 0) return null;
        if (xy.x >= Width || xy.y >= Height) return null;
        return _cells[xy.x, xy.y];
    }

    public bool CanBuildOn(Vector2Int xy)
    {
        //skip eves
        var isEvenX = xy.x % 2 == 0;
        var isEvenY = xy.y % 2 == 0;
        //if (isEvenX && isEvenY) return false;
        //for editor
        if (_cells == null) return true;
        var tile = GetCell(xy);
        if (tile == null) return false;
        return !tile.Busy;
    }

    public GridCell RandomFreeCell()
    {
        var list = new List<GridCell>();
        
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (!_cells[x, y].Busy) list.Add(_cells[x,y]);
            }
        }

        return list.PickRandom();
    }

    private void OnDrawGizmos()
    {
        
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var tile = _cells == null ? null : _cells[x, y];
                if (CanBuildOn(new Vector2Int(x, y)))
                {
                    Gizmos.color = Color.green.WithAlpha(0.05f);
                }
                else if (tile == null)
                {
                    Gizmos.color = Color.yellow.WithAlpha(0.05f);
                }
                else if (tile.Busy)
                {
                    Gizmos.color = Color.red.WithAlpha(0.05f);
                }
                else
                {
                    Gizmos.color = Color.yellow.WithAlpha(0.05f);
                }

                if (tile != null && tile.Highlighted)
                {
                    Gizmos.color = Gizmos.color.WithAlpha(0.6f);
                }

                if (tile != null && tile.HasRoad)
                {
                    var rs = tile.Road.GetComponentInChildren<RoadSwitch>();
                    if (rs != null && rs.HasTrainOnTop)
                    {
                        Gizmos.color = Color.red.WithAlpha(0.8f);;
                    }
                }

                Gizmos.DrawCube(CenterOfTile(new Vector2Int(x, y)), new Vector3(1, 1, 0.1f));

            }
        }
        Gizmos.color = Color.green.WithAlpha(0.4f);
        Gizmos.DrawSphere(CenterOfTile(EnterPoint), 0.5f);
    }

    
}
