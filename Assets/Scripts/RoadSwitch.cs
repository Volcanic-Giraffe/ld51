using System;
using UnityEngine;

public class RoadSwitch : MonoBehaviour
{
    Vector2Int _direction;
    private ConnectionType connection;
    private SpriteRenderer _sr;

    public Vector2Int Direction { get => _direction; }

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _direction = Vector2Int.right;
    }

    void OnMouseDown()
    {
        Toggle();
    }

    public void Toggle()
    {
        if (_direction == Vector2Int.up)
        {
            _direction = connection == ConnectionType.TripleDLU ? Vector2Int.down : _direction = Vector2Int.right;
        }
        else if (_direction == Vector2Int.right)
        {
            _direction = connection == ConnectionType.TripleLUR ? Vector2Int.left : _direction = Vector2Int.down;
        }
        else if (_direction == Vector2Int.down)
        {
            _direction = connection == ConnectionType.TripleURD ? Vector2Int.up : Vector2Int.left;
        }
        else
        {
            _direction = connection == ConnectionType.TripleRDL ? Vector2Int.right : Vector2Int.up;
        }
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        _sr.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    internal void SetAllowedConnections(ConnectionType conn)
    {
        if (conn != ConnectionType.TripleURD && conn != ConnectionType.TripleDLU
            && conn != ConnectionType.TripleLUR && conn != ConnectionType.TripleRDL
            && conn != ConnectionType.Quad)
        {
            throw new Exception($"Switch can only operate on triple/quad connectors, {gameObject.transform.parent}");
        }
        connection = conn;
        if (connection == ConnectionType.TripleDLU) Toggle();
    }
}