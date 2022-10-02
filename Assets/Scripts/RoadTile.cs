using System;
using System.Collections.Generic;
using System.Linq;
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

    public void ToggleIfCan()
    {
        if (RoadSwitch != null)
        {
            RoadSwitch.Toggle();
        }
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
    
    private static Vector3[] L2D = new[]
    {
        new Vector3(-0.5f, 0, 0),
        new Vector3(-0.35f, 0, 0),
        new Vector3(-0.15f, -0.15f, 0),
        new Vector3(0, -0.35f, 0),
        new Vector3(0, -0.5f, 0),
    };

    private static Vector3[] R2D = L2D.Select(it => new Vector3(-it.x, it.y, 0)).ToArray();
    private static Vector3[] L2U = L2D.Select(it => new Vector3(it.x, -it.y, 0)).ToArray();
    private static Vector3[] R2U = R2D.Select(it => new Vector3(it.x, -it.y, 0)).ToArray();
    
    private static Vector3[] D2L = L2D.Reverse().ToArray();
    private static Vector3[] D2R = R2D.Reverse().ToArray();
    private static Vector3[] U2R = R2U.Reverse().ToArray();
    private static Vector3[] U2L = L2U.Reverse().ToArray();

    private static Vector3[] L2R = { new(-0.5f, 0), new(0.5f, 0), };
    private static Vector3[] R2L = L2R.Reverse().ToArray();
    private static Vector3[] U2D = { new(0, 0.5f), new(0, -0.5f), };
    private static Vector3[] D2U = U2D.Reverse().ToArray();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < L2D.Length - 1; i++)
        {
            Gizmos.DrawLine(L2D[i], L2D[i+1]);
        }
    }

    public Vector3[] Rotation(Vector2Int from, Vector2Int to)
    {
        if (from == Vector2Int.left && to == Vector2Int.down) return L2D;
        if (from == Vector2Int.left && to == Vector2Int.up) return L2U;
        if (from == Vector2Int.right && to == Vector2Int.down) return R2D;
        if (from == Vector2Int.right && to == Vector2Int.up) return R2U;
        
        if (from == Vector2Int.down && to == Vector2Int.left) return D2L;
        if (from == Vector2Int.down && to == Vector2Int.right) return D2R;
        if (from == Vector2Int.up && to == Vector2Int.left) return U2L;
        if (from == Vector2Int.up && to == Vector2Int.right) return U2R;
        
        //just direct line i guess
        if (from == Vector2Int.left && to == Vector2Int.right) return L2R;
        if (from == Vector2Int.right && to == Vector2Int.left) return R2L;
        if (from == Vector2Int.up && to == Vector2Int.down) return U2D;
        if (from == Vector2Int.down && to == Vector2Int.up) return D2U;
        throw new Exception("Damn");
    }

    public Vector3 GetNextPoint(Vector3 ourPoint, Vector2Int from, Vector2Int to)
    {
        var rot = Rotation(from, to);
        var localPoint = ourPoint - transform.position;
        var idx = -1;
        var minL = 9999999999f;
        for (int i = 0; i < rot.Length; i++)
        {
            var distance = Vector3.Distance(localPoint, rot[i]);
            if (distance < minL)
            {
                minL = distance;
                idx = i;
            }
        }

        return transform.position + rot[idx == rot.Length - 1 ? idx : idx + 1];
    }
}

