using System.Collections.Generic;
using UnityEngine;

public enum ConnectionType
{
    Horizontal, Vertical, DiagLU, DiagUR, DiagRD, DiagDL, TripleLUR, TripleURD, TripleRDL, TripleDLU, Quad
}

public class RoadTile : MonoBehaviour
{
    public ConnectionType Type;
    RoadSwitch RoadSwitch;

    private void Awake()
    {
        RoadSwitch = GetComponentInChildren<RoadSwitch>();
    }

    private void Start()
    {
        if (RoadSwitch != null) RoadSwitch.SetAllowedConnections(Type);
    }
    public Vector2Int Direction(Vector2Int from, Wagon wagon = null)
    {
        switch (Type)
        {
            case ConnectionType.Horizontal:
                return new Vector2Int(from.x * -1, 0);
            case ConnectionType.Vertical:
                return new Vector2Int(0, from.y * -1);
            case ConnectionType.DiagDL:
                if (from == Vector2Int.down) return Vector2Int.left;
                if (from == Vector2Int.left) return Vector2Int.down;
                break;
            case ConnectionType.DiagRD:
                if (from == Vector2Int.down) return Vector2Int.right;
                if (from == Vector2Int.right) return Vector2Int.down;
                break;
            case ConnectionType.DiagLU:
                if (from == Vector2Int.left) return Vector2Int.up;
                if (from == Vector2Int.up) return Vector2Int.left;
                break;
            case ConnectionType.DiagUR:
                if (from == Vector2Int.up) return Vector2Int.right;
                if (from == Vector2Int.right) return Vector2Int.up;
                break;
            case ConnectionType.TripleDLU:
            case ConnectionType.TripleLUR:
            case ConnectionType.TripleRDL:
            case ConnectionType.TripleURD:
            case ConnectionType.Quad:
                if (RoadSwitch.Direction != from) return RoadSwitch.Direction;
                break;
        }
        // cant go!
        return Vector2Int.zero;
    }

    public static Dictionary<ConnectionType, Vector2Int[]> AllowedDirections = new Dictionary<ConnectionType, Vector2Int[]>()
    {
        { ConnectionType.Horizontal, new Vector2Int[] { Vector2Int.up, Vector2Int.down } },
        { ConnectionType.Vertical, new Vector2Int[] { Vector2Int.left, Vector2Int.right } },
        { ConnectionType.DiagDL, new Vector2Int[] { Vector2Int.down, Vector2Int.left} },
        { ConnectionType.DiagLU, new Vector2Int[] { Vector2Int.left, Vector2Int.up} },
        { ConnectionType.DiagRD, new Vector2Int[] { Vector2Int.right, Vector2Int.down} },
        { ConnectionType.DiagUR, new Vector2Int[] { Vector2Int.up, Vector2Int.right} },
        { ConnectionType.TripleDLU, new Vector2Int[] { Vector2Int.up, Vector2Int.left, Vector2Int.down} },
        { ConnectionType.TripleLUR, new Vector2Int[] { Vector2Int.left, Vector2Int.up, Vector2Int.right} },
        { ConnectionType.TripleRDL, new Vector2Int[] { Vector2Int.right, Vector2Int.down, Vector2Int.left} },
        { ConnectionType.TripleURD, new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down} },
        { ConnectionType.Quad, new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left} },
    };
}

