using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
    
    private void Awake()
    {
        _baseSpeed = Speed;
        _currentSpeed = Speed;
        
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        RefreshLinks();
    }

    private void Update()
    {
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
    }

    private RoadTile _previousRoad = null;
    private Vector2Int _fromPrevious;

    private void FixedUpdate()
    {
        
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

            }

            if (IsFirstWagon())
            {
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
            Debug.Log("Strange.....");
            Stop();
        }

        var targetCell = MoonGrid.Instance.CenterOfTile(myXY + Direction);
        var newLoc = transform.position + (targetCell - transform.position).normalized * (_trainHead._currentSpeed * Time.fixedDeltaTime);
        newLoc.z = 0;
        _rigidBody.MovePosition(newLoc);
            
        var dir = targetCell - transform.position;
        float angleS = Vector3.SignedAngle(dir, transform.right, -Vector3.forward);

        Rotator.eulerAngles = new Vector3(0, 0, angleS);
    }

    private int CellsToObstacle(Vector2Int xy, Vector2Int from, int max)
    {
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
}
