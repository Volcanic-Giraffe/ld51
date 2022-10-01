
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Grid : MonoBehaviour
{

    public static Grid Instance;

    public int Width;
    public int Height;

    public GridCell GridCellPrefab;

    private void Awake()
    {
        Instance = this;
        _camera = Camera.main;
    }

    private GridCell[,] _tiles;
    private Camera _camera;

    private void Start()
    {
        
        _tiles = new GridCell[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var tile = Instantiate(GridCellPrefab, transform);
                tile.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, 0);
                _tiles[x, y] = tile;
            }
        }
    }

    public Vector2Int XY(Vector3 vector)
    {
        return new Vector2Int((int)(vector.x), (int)(vector.y));        
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
    public GridCell GetTile(Vector2Int xy)
    {
        if (xy.x < 0 || xy.y < 0) return null;
        if (xy.x >= Width || xy.y >= Height) return null;
        return _tiles[xy.x, xy.y];
    }

    public bool CanBuildOn(Vector2Int xy)
    {
        //skip eves
        var isEvenX = xy.x % 2 == 0;
        var isEvenY = xy.y % 2 == 0;
        if (isEvenX && isEvenY) return false;
        var tile = GetTile(xy);
        if (tile == null) return false;
        return !tile.Busy;
    }

    private void Update()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = -_camera.transform.position.z;
        Vector3 worldPosition = _camera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0;
        Vector2Int xy = XY(worldPosition);
        foreach (var tile in _tiles)
        {
            tile.Highlighted = XY(tile) == xy;
        }
    }

    private void OnDrawGizmos()
    {
        if (_tiles == null) return;
        foreach (var tile in _tiles)
        {
            if (tile.Busy)
            {
                Gizmos.color = Color.red.WithAlpha(0.2f);
            }
            else if (CanBuildOn(XY(tile)))
            {
                Gizmos.color = Color.green.WithAlpha(0.2f);
            }
            else
            {
                Gizmos.color = Color.yellow.WithAlpha(0.2f);
            }

            if (tile.Highlighted)
            {
                Gizmos.color = Gizmos.color.WithAlpha(0.6f);
            }

            Gizmos.DrawCube(tile.transform.position, new Vector3(1, 1, 0.1f));
        }
    }

    
}
