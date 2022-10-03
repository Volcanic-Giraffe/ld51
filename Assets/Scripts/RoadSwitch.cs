using System;
using UnityEngine;

public class RoadSwitch : MonoBehaviour
{
    Vector2Int _direction;
    private ConnectionType connection;
    private SpriteRenderer _sr;
    private bool _isWagonOnTop;
    private float _enableTimeRemaining = 0;
    private GridCell _mycell;

    public Vector2Int Direction { get => _direction; }

    public bool HasTrainOnTop => _isWagonOnTop;
    
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _direction = Vector2Int.right;
    }

    private void Start()
    {
        _mycell = MoonGrid.Instance.GetCell(MoonGrid.Instance.XY(transform.position));
    }

    private void FixedUpdate()
    {
        var isOnTop = Physics.Raycast(transform.position, Vector3.back, 1f, LayerMask.GetMask("Wagon"), QueryTriggerInteraction.Collide);
        if (isOnTop)
        {
            _enableTimeRemaining = 0.3f;
        }
    }

    private void Update()
    {
        if (_enableTimeRemaining > 0)
        {
            _enableTimeRemaining -= Time.deltaTime;
            _isWagonOnTop = true;
        }
        else
        {
            _isWagonOnTop = false;
        }
        transform.localScale = _mycell.Highlighted ? new Vector3(2, 2, 2) : Vector3.one;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Vector3.back);
    }
#endif

    public void Toggle()
    {
        if (_isWagonOnTop) return;
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