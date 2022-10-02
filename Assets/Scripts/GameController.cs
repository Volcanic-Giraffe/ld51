using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public float SpawnTime = 10f;

    public GameMode Mode = GameMode.Build;
    private Camera _camera;

    public RoadTile[] RoadTilePrefabs;

    public List<WagonPrefab> WagonPrefabs;

    public List<UnloadingStation> StationPrefabs;

    private float _timer;

    public PlayerStats Stats { get; private set; }

    private void Awake()
    {
        Stats = new PlayerStats();

        Instance = this;
        _camera = Camera.main;
    }

    private void Start()
    {
        foreach (var road in GetComponentsInChildren<RoadTile>())
        {
            var cell = MoonGrid.Instance.GetCell(MoonGrid.Instance.XY(road));
            if (cell == null) continue;
            cell.Road = road;
        }

        RecalculateRoads();
    }

    public enum GameMode
    {
        Build,
        Sort
    }

    private bool _leftMouseWasDown = false;

    enum DragTo
    {
        Nothing,
        Clear,
        Build
    }

    private DragTo _dragTo = DragTo.Nothing;

    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 mouseWorldPosition = new Vector3(-5, -5, 0);
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Cell")))
        {
            mouseWorldPosition = hit.transform.position;
        }

        var leftMouseDown = Input.GetMouseButton(0);
        
        Vector2Int xy = MoonGrid.Instance.XY(mouseWorldPosition);
        foreach (var tile in MoonGrid.Instance.Cells)
        {
            tile.Highlighted = MoonGrid.Instance.XY(tile) == xy;
        }
        
        switch (Mode)
        {
            case GameMode.Build:
            {

                var thisCell = MoonGrid.Instance.GetCell(xy);
                if (!_leftMouseWasDown && leftMouseDown)
                {
                    //just pressed. remember what we are doing
                    if (MoonGrid.Instance.CanBuildOn(xy))
                    {
                        if (thisCell.HasRoad)
                        {
                            _dragTo = DragTo.Clear;
                        }
                        else
                        {
                            _dragTo = DragTo.Build;
                        }
                    }
                    else
                    {
                        _dragTo = DragTo.Nothing;
                    }
                }

                if (leftMouseDown && MoonGrid.Instance.CanBuildOn(xy))
                {
                    //we are dragging
                    if (_dragTo == DragTo.Build && !thisCell.HasRoad)
                    {
                        BuildRoad(thisCell);
                    }

                    if (_dragTo == DragTo.Clear && thisCell.HasRoad)
                    {
                        RemoveRoad(thisCell);
                    }
                }

                if (!leftMouseDown && _leftMouseWasDown)
                {
                    //we are up
                    _dragTo = DragTo.Nothing;
                }


                break;
            }
            case GameMode.Sort:
            {
                var thisCell = MoonGrid.Instance.GetCell(xy);
                if (thisCell != null && thisCell.HasRoad)
                {
                    if (!_leftMouseWasDown && leftMouseDown)
                    {
                        thisCell.Road.ToggleIfCan();
                    }
                }


                UpdateSort();
                break;
            }
        }

        _leftMouseWasDown = leftMouseDown;
    }

    private void UpdateSort()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _timer = SpawnTime;

            SpawnTrain();
        }
    }

    void BuildRoad(GridCell cell)
    {
        cell.Road = Instantiate(RoadTilePrefabs[0], transform);
        RecalculateRoads();
    }

    void RemoveRoad(GridCell cell)
    {
        var road = cell.Road;
        Destroy(road.gameObject);
        cell.Road = null;
        RecalculateRoads();
    }

    public void OnRun()
    {
        if (Mode == GameMode.Sort)
        {
            // Dev ability to restart the game

            Mode = GameMode.Build;

            var wagons = FindObjectsOfType<Wagon>();

            for (var i = wagons.Length - 1; i >= 0; i--)
            {
                var wagon = wagons[i];

                Destroy(wagon.gameObject);
            }
        }
        else
        {
            Mode = GameMode.Sort;
            _timer = SpawnTime * 0.1f;
        }
    }

    private void SpawnTrain()
    {
        if (Mode == GameMode.Build) return;

        var newTrain = ProduceWagon(WagonType.Locomotive);
        newTrain.transform.position =
            MoonGrid.Instance.CenterOfTile(MoonGrid.Instance.EnterPoint + Vector2Int.left * 3);

        var types = new List<WagonType>() { WagonType.Green, WagonType.Blue, WagonType.Red };
        types = types.Shuffle().ToList();

        if (Random.value < 0.4f) types.RemoveAt(0);
        if (Random.value < 0.4f) types.RemoveAt(0);

        foreach (var wType in types)
        {
            newTrain.AddNewWagon(wType);
        }
    }

    public Wagon ProduceWagon(WagonType type)
    {
        return Instantiate(WagonPrefabs.Find(p => p.Type == type).Wagon, transform);
    }

    private void RecalculateRoads()
    {
        foreach (var cell in MoonGrid.Instance.Cells)
        {
            if (!cell.HasRoad) continue;
            var xy = MoonGrid.Instance.XY(cell);

            var L = MoonGrid.Instance.GetCell(xy + Vector2Int.left)?.HasRoad == true;
            var R = MoonGrid.Instance.GetCell(xy + Vector2Int.right)?.HasRoad == true;
            var U = MoonGrid.Instance.GetCell(xy + Vector2Int.up)?.HasRoad == true;
            var D = MoonGrid.Instance.GetCell(xy + Vector2Int.down)?.HasRoad == true;

            ConnectionType type = xy.y % 2 == 0 ? ConnectionType.Vertical : ConnectionType.Horizontal;
            if ((R || L) && !D && !U)
            {
                type = ConnectionType.Horizontal;
            }
            else if ((D || U) && !L && !R)
            {
                type = ConnectionType.Vertical;
            }
            else if (D && L && R && U)
            {
                type = ConnectionType.Quad;
            }
            else if (D && L && !R && !U)
            {
                type = ConnectionType.DiagDL;
            }
            else if (D && R && !L && !U)
            {
                type = ConnectionType.DiagRD;
            }
            else if (U && L && !R && !D)
            {
                type = ConnectionType.DiagLU;
            }
            else if (U && R && !L && !D)
            {
                type = ConnectionType.DiagUR;
            }
            else if (U && R && D)
            {
                type = ConnectionType.TripleURD;
            }
            else if (U && L && D)
            {
                type = ConnectionType.TripleDLU;
            }
            else if (L && R && U)
            {
                type = ConnectionType.TripleLUR;
            }
            else if (L && R && D)
            {
                type = ConnectionType.TripleRDL;
            }

            SetRoad(cell, type);
        }
    }

    void SetRoad(GridCell cell, ConnectionType type)
    {
        var road = cell.Road;
        if (road.Type == type) return;
        var newRoad = Instantiate(RoadTilePrefabs.First(it => it.Type == type), road.transform.parent);
        newRoad.transform.position = road.transform.position;
        Destroy(road.gameObject);
        cell.Road = newRoad;
    }
}