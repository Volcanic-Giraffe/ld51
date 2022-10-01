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
    
    private Rigidbody _rigidBody;

    public float Speed;
    public Vector2Int Direction = Vector2Int.right;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        
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
        var newLoc = transform.position + (targetCell - transform.position).normalized * (Speed * Time.fixedDeltaTime);
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
}
