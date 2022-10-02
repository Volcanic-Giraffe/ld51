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
    
    private void Awake()
    {
        _baseSpeed = Speed;
        
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        RefreshLinks();
    }

    private void Update()
    {
        if (_trainHead == null || _trainHead.WagonType != WagonType.Locomotive)
        {
            RemoveFromTrain(RemoveReason.LostLocomotive);
        }
    }

    private RoadTile _previousRoad = null;

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
                var from = _previousRoad != null ? MoonGrid.Instance.XY(_previousRoad) - myXY : Vector2Int.left;
                //my new direction
                Direction = roadIamAt.Direction(from, this);
                //Debug.Log($"Switch diration of {this} to {Direction} (from = {from})");
            }

            _previousRoad = roadIamAt;
        }
        else
        {
            Debug.Log("Strange.....");
        }

        var targetCell = MoonGrid.Instance.CenterOfTile(myXY + Direction);
        var newLoc = transform.position + (targetCell - transform.position).normalized * (_trainHead.Speed * Time.fixedDeltaTime);
        newLoc.z = 0;
        _rigidBody.MovePosition(newLoc);
            
        var dir = targetCell - transform.position;
        float angleS = Vector3.SignedAngle(dir, transform.right, -Vector3.forward);

        Rotator.eulerAngles = new Vector3(0, 0, angleS);
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

    public void RemoveFromTrain(RemoveReason reason)
    {
        if (RearWagon != null)
        {
            RearWagon.FrontWagon = FrontWagon;
        }

        if (FrontWagon != null)
        {
            FrontWagon.RearWagon = RearWagon;
        }

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

            if (HasWagon(wagon)) return;

            if (WagonType == WagonType.Locomotive)
            {
                wagon.RemoveFromTrain(RemoveReason.Collision);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wagon"))
        {
            var wagon = other.GetComponentInParent<Wagon>();

            if (HasWagon(wagon)) return;
            
            // wagon.SpeedUp();
        }
    }
}
