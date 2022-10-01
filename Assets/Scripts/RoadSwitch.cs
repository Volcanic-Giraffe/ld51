using System;
using UnityEngine;

public class RoadSwitch : MonoBehaviour
{
    public Vector2Int Direction;
    private ConnectionType connection;

    private void Start()
    {
        if (Math.Abs(Direction.x + Direction.y) != 1) Direction = Vector2Int.up;
    }
    public void Toggle()
    {
        if (Direction == Vector2Int.up)
        {
            Direction = connection == ConnectionType.TripleDLU ? Vector2Int.down : Direction = Vector2Int.right;
        }
        else if (Direction == Vector2Int.right)
        {
            Direction = connection == ConnectionType.TripleLUR ? Vector2Int.left : Direction = Vector2Int.down;
        }
        else if (Direction == Vector2Int.down)
        {
            Direction = connection == ConnectionType.TripleURD ? Vector2Int.up : Vector2Int.left;
        }
        else
        {
            Direction = connection == ConnectionType.TripleRDL ? Vector2Int.right : Vector2Int.up;
        }
        // TODO ROTATE ARROW
    }
    internal void SetAllowedConnections(ConnectionType conn)
    {
        if (connection != ConnectionType.TripleURD && connection != ConnectionType.TripleDLU
            && connection != ConnectionType.TripleLUR && connection != ConnectionType.TripleRDL
            && connection != ConnectionType.Quad) throw new Exception($"Switch can only operate on triple/quad connectors, {gameObject}");
        this.connection = conn;
    }
}