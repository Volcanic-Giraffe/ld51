using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class Wagon : MonoBehaviour
{
    public WagonType WagonType;
    
    public const float LinkLength = 1.5f;

    [SerializeField] private Transform Rotator;

    public Wagon FrontWagon;
    public Wagon RearWagon;
    
    public float Speed;
    public Vector2Int Direction = Vector2Int.right;

    [Space]
    public GameObject RemoveEffectGood;
    public GameObject RemoveEffectBad;
    
    private Rigidbody _rigidBody;

    // Basically a locomotive of this train.
    private Wagon _trainHead;

    private float _baseSpeed;

    private float _currentSpeed;
    
    private bool _removed;
    private bool _exited;

    public static float GlobalSpeedMod = 1f;
    
    private void Awake()
    {
        _baseSpeed = Speed;
        _currentSpeed = Speed;
        
        _rigidBody = GetComponent<Rigidbody>();

        _wagonItself = GetComponentsInChildren<MeshRenderer>().First(it => it.GetComponent<Wheel>() == null);
    }

    private void OnEnable()
    {
        Awake();
    }

    private void Start()
    {
        RefreshLinks();
        
        if (LevelScenario.Instance != null) LevelScenario.Instance.AddWagon(this);
    }

    private void Update()
    {
        if (GameController.GameOver) return;
        
        if (FrontWagon == null && WagonType != WagonType.Locomotive)
        {
            RemoveFromTrain(RemoveReason.LostLocomotive);
        }

        if (Mathf.Abs(_currentSpeed - Speed) > 0.05f)
        {
            var delta = Speed - _currentSpeed;
            if (Mathf.Abs(delta) > 0.2f)
            {
                delta = MathF.Sign(delta) * 0.2f;
            }

            _currentSpeed += delta;
        }

        if (_wagonItself != null)
        {
            _wagonItself.transform.localPosition = new Vector3(
                0,
                MathF.Cos( GetInstanceID() + Time.time*9.1112f)*0.02f, 
                MathF.Sin(GetInstanceID() - Time.time*4.017444f)*0.02f
                );
        }
    }

    private RoadTile _previousRoad = null;
    private Vector2Int _fromPrevious;
    private MeshRenderer _wagonItself;

    private void FixedUpdate()
    {
        if (GameController.GameOver) return;

        var myXY = MoonGrid.Instance.XY(this);
        var myCell = MoonGrid.Instance.GetCell(myXY);

        if (myXY.x < 0)
        {
            //do nothing
        }
        else if (myCell != null && myCell.HasRoad)
        {
            var roadIamAt = myCell.Road;
            if (roadIamAt != _previousRoad)
            {
                _fromPrevious = _previousRoad != null ? MoonGrid.Instance.XY(_previousRoad) - myXY : Vector2Int.left;
                //my new direction
                Direction = roadIamAt.Direction(_fromPrevious, this);
                    //Debug.Log($"prevFrom = {_fromPrevious}, direction={Direction}");
            }

            if (IsFirstWagon() && !_exited)
            {
                _checkedPoints.Clear();
                var cellsToObstacle = CellsToObstacle(myXY, _fromPrevious, 5);

                if (cellsToObstacle <= 1)
                {
                    Stop();
                }
                else if (cellsToObstacle <= 3)
                {
                    SlowDown();
                }
                else
                {
                    SpeedUp();
                }

            }

            _previousRoad = roadIamAt;
        }
        else
        {
            RemoveFromTrain(RemoveReason.OffTrack);

            if (!_exited)
            {
                Debug.Log("Strange.....");
            }
            Stop();
            return;
        }

        
        
        var targetCell = (myCell != null && myCell.HasRoad) 
            ? myCell.Road.GetNextPoint(transform.position, _fromPrevious, Direction)
            : MoonGrid.Instance.CenterOfTile(myXY + Direction);

        var trainSpeed = _trainHead._currentSpeed * GlobalSpeedMod;
        
        var newLoc = transform.position + (targetCell - transform.position).normalized * (trainSpeed * Time.fixedDeltaTime);
        newLoc.z = 0;
        _rigidBody.MovePosition(newLoc);
            
        var dir = targetCell - transform.position;
        float angleS = Vector3.SignedAngle(dir, transform.right, -Vector3.forward);

        Rotator.eulerAngles = new Vector3(0, 0, angleS);
    }

    private List<Vector2Int> _checkedPoints = new List<Vector2Int>();  
    private int CellsToObstacle(Vector2Int xy, Vector2Int from, int max)
    {
        _checkedPoints.Add(xy);
        if (max == 0) return 0;
        var cell = MoonGrid.Instance.GetCell(xy);
        if (cell == null || !cell.HasRoad) return 0;
        var newDirection = cell.Road.Direction(from);
        if (newDirection == Vector2Int.zero) return 0;
        return 1 + CellsToObstacle(xy + newDirection, -newDirection, max - 1);

    }
    

    public void AddNewWagon(WagonType type)
    {
        var lastWagon = LastWagon();

        var pos = lastWagon.transform.position;
        
        var newWagon = GameController.Instance.ProduceWagon(type);
        newWagon.FrontWagon = lastWagon;
        lastWagon.RearWagon = newWagon;

        newWagon.transform.position = new Vector3(pos.x - LinkLength, pos.y, 0f);

        RefreshLinks();
    }

    public Wagon LastWagon()
    {
        if (RearWagon != null)
        {
            return RearWagon.LastWagon();
        }
        else
        {
            return this;
        }
    }
    
    public Wagon FirstWagon()
    {
        if (FrontWagon != null)
        {
            return FrontWagon.FirstWagon();
        }
        else
        {
            return this;
        }
    }

    public void SlowDown()
    {
        _trainHead.Speed = _baseSpeed * 0.3f;
    }

    public void SpeedUp()
    {
        _trainHead.Speed = _baseSpeed;
    }

    public void Stop()
    {
        _trainHead.Speed = 0;
    }

    public void RemoveFromTrain(RemoveReason reason)
    {
        if (_removed) return;
        _removed = true;

        if (_exited)
        {
            Destroy(gameObject);
            return;
        }
        
        // >>> re-linking is disabled, only tail carts can be removed for good score.
        // if (RearWagon != null) RearWagon.FrontWagon = FrontWagon;
        // if (FrontWagon != null) FrontWagon.RearWagon = RearWagon;
        // <<<
        
        // null linking helps removing wagons with no locomotive
        if (RearWagon != null) RearWagon.FrontWagon = null;
        if (FrontWagon != null) FrontWagon.RearWagon = null;
        

        // todo: move positions on tail wagons

        var effect = RemoveReasons.IsGood(reason) ? RemoveEffectGood : RemoveEffectBad;

        Instantiate(effect, transform.position, Quaternion.identity);

        if (!RemoveReasons.IsGood(reason) && WagonType == WagonType.Locomotive)
        {
            GameController.Instance.Stats.AddLife(-1);
        }
        
        Destroy(gameObject);
        
    }

    public bool IsLastWagon()
    {
        return RearWagon == null;
    }
    
    public bool IsFirstWagon()
    {
        return FrontWagon == null;
    }

    public bool HasWagon(Wagon wagon)
    {
        return this == wagon || FrontHas(wagon) || RearHas(wagon);
    }

    private bool FrontHas(Wagon wagon)
    {
        if (FrontWagon == wagon) return true;
        return FrontWagon != null && FrontWagon.FrontHas(wagon);
    }
    
    private bool RearHas(Wagon wagon)
    {
        if (RearWagon == wagon) return true;
        return RearWagon != null && RearWagon.RearHas(wagon);
    }

    public int TrainSize()
    {
        var count = 1;

        var current = _trainHead.RearWagon;

        while (current != null)
        {
            count += 1;
            current = current.RearWagon;
        }

        return count;
    }
    
    public void ExitGrid(RemoveReason reason)
    {
        if (_exited) return;
        
        _exited = true;

        var colliders = GetComponentsInChildren<Collider>();

        foreach (var cldr in colliders)
        {
            cldr.enabled = false;
        }
        
        var effect = RemoveReasons.IsGood(reason) ? RemoveEffectGood : RemoveEffectBad;

        Instantiate(effect, transform.position, Quaternion.identity);
        
        Destroy(gameObject, 10f); // unexpected, but just in case
    }

    
    private void RefreshLinks()
    {
        _trainHead = FirstWagon();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wagon"))
        {
            var wagon = other.GetComponentInParent<Wagon>();

            if (WagonType == WagonType.Locomotive && !HasWagon(wagon))
            {
                wagon.RemoveFromTrain(RemoveReason.Collision);
            }
            
            // self collision mostly
            if (wagon != FrontWagon && wagon != RearWagon && WagonType == WagonType.Locomotive)
            {
                wagon.RemoveFromTrain(RemoveReason.Collision);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wagon"))
        {
            // var wagon = other.GetComponentInParent<Wagon>();
            // ..
        }
    }

    private void OnDestroy()
    {
        if (LevelScenario.Instance != null) LevelScenario.Instance.RemoveWagon(this);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (var vi in _checkedPoints)
        {
            Gizmos.DrawSphere(MoonGrid.Instance.CenterOfTile(vi), 0.3f);
        }
        Handles.Label(transform.position, $"Speed={_currentSpeed} Dir={Direction}");
    }
#endif

}
