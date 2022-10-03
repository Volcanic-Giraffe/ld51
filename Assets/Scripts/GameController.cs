using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameMode Mode = GameMode.Build;
    private Camera _camera;

    public RoadTile[] RoadTilePrefabs;

    public List<WagonPrefab> WagonPrefabs;

    public List<UnloadingStation> StationPrefabs;

    public PlayerStats Stats { get; private set; }

    private List<UnloadingStation> _stations;

    public GameObject BuildCursor;

    public event Action OnModeChanged;

    private bool _gameOver;

    public static bool GameOver => Instance != null && Instance._gameOver;
    
    private void Awake()
    {
        _stations = FindObjectsOfType<UnloadingStation>().ToList();

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
        SpawnStation();

        LevelScenario.Instance.OnWaveBegin += PrepareForWave;
        LevelScenario.Instance.OnEveryTenSeconds += OnWaveUpdate;
    }

    public enum GameMode
    {
        Build,
        Sort
    }

    private bool _leftMouseWasDown = false;
    private bool _rightMouseWasDown = false;

    enum DragTo
    {
        Nothing,
        Clear,
        Build
    }

    private DragTo _dragTo = DragTo.Nothing;

    private void Update()
    {
        if (GameOver) return;
        
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 mouseWorldPosition = new Vector3(-5, -5, 0);
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Cell")))
        {
            mouseWorldPosition = hit.transform.position;

        }

        var leftMouseDown = Input.GetMouseButton(0);
        var rightMouseDown = Input.GetMouseButton(1);

        Vector2Int xy = MoonGrid.Instance.XY(mouseWorldPosition);
        BuildCursor.transform.position = new Vector3(xy.x, xy.y, 0);
        foreach (var tile in MoonGrid.Instance.Cells)
        {
            if (tile.Locked) continue;

            tile.Highlighted = MoonGrid.Instance.XY(tile) == xy;
        }

        switch (Mode)
        {
            case GameMode.Build:
                {
                    // BuildStyleA(xy, leftMouseDown,rightMouseDown); // - old
                    BuildStyleB(xy, leftMouseDown,rightMouseDown); // - new

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

                    break;
                }
        }

        _leftMouseWasDown = leftMouseDown;
        _rightMouseWasDown = rightMouseDown;
    }

    // Original. LMB mostly
    private void BuildStyleA(Vector2Int xy, bool lmb, bool rmb)
    {
        var thisCell = MoonGrid.Instance.GetCell(xy);
        if (!_leftMouseWasDown && lmb)
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

        if (lmb && MoonGrid.Instance.CanBuildOn(xy))
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

        if (!lmb && _leftMouseWasDown)
        {
            //we are up
            _dragTo = DragTo.Nothing;
        }
    }
    
    // New. LMB and RMB
    private void BuildStyleB(Vector2Int xy, bool lmb, bool rmb)
    {
        var thisCell = MoonGrid.Instance.GetCell(xy);

        if (lmb && MoonGrid.Instance.CanBuildOn(xy))
        {
            if (!thisCell.HasRoad)
            {
                BuildRoad(thisCell);
            }
            else
            {
                if (!_leftMouseWasDown)
                {
                    thisCell.Road.ToggleIfCan();
                }
            }
        }

        if (rmb)
        {
            if (thisCell.HasRoad)
            {
                RemoveRoad(thisCell);
            }
        }
    }
    
    public void PrepareForWave()
    {
        SpawnStation();
        
        SetMode(GameMode.Build);
        BuildCursor.SetActive(true);
        var wagons = FindObjectsOfType<Wagon>();

        for (var i = wagons.Length - 1; i >= 0; i--)
        {
            var wagon = wagons[i];

            Destroy(wagon.gameObject);
        }
    }

    private void SetMode(GameMode mode)
    {
        Mode = mode;
        
        OnModeChanged?.Invoke();
    }

    public void OnWaveUpdate()
    {
        SpawnTrain();
    }

    void BuildRoad(GridCell cell)
    {
        cell.Road = Instantiate(RoadTilePrefabs[0], transform);
        cell.Road.transform.position = cell.transform.position;
        
        RecalculateRoads();
    }

    void RemoveRoad(GridCell cell)
    {
        if (cell == null || cell.Road == null) return;
        
        var road = cell.Road;
        Destroy(road.gameObject);
        cell.Road = null;
        RecalculateRoads();
    }

    public void OnRun()
    {
        BuildCursor.SetActive(false);
        SetMode(GameMode.Sort);
    }

    public void OnBuild()
    {
        PrepareForWave();
    }

    private void SpawnStation()
    {
        var wave = LevelScenario.Instance.Wave;

        if (_stations.Count < wave.Stations)
        {
            for (var i = _stations.Count; i < wave.Stations; i++)
            {
                var newStation = Instantiate(StationPrefabs[i % StationPrefabs.Count]);

                var cell = MoonGrid.Instance.RandomFreeCell(12, 2, 2);

                newStation.transform.position = cell.transform.position;
                cell.Busy = false; // busy makes impossible to place roads.
                cell.Station = true;

                var pattern = UnloadingStation.Patterns[newStation.WagonType];
                
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        var dx = x - 1;
                        var dy = y - 1;

                        var neih = MoonGrid.Instance.GetCell(cell.X + dx, cell.Y + dy);

                        if (neih != null)
                        {
                            neih.Station = true;
                            
                            if (pattern[x, y] == 1)
                            {
                                neih.Busy = true;
                                
                                RemoveRoad(neih);
                            }
                        }
                    }
                }

                cell.Element = newStation;

                _stations.Add(newStation);
            }
        }
    }
    private void SpawnTrain()
    {
        if (Mode == GameMode.Build) return;

        var newTrain = ProduceWagon(WagonType.Locomotive);
        newTrain.transform.position =
            MoonGrid.Instance.CenterOfTile(MoonGrid.Instance.EnterPoint + Vector2Int.left * 3);

        var wave = LevelScenario.Instance.Wave;
        var length = Random.Range(wave.TrainsLengthMin, wave.TrainsLengthMax);

        var types = _stations.Select(s => s.WagonType).ToList();

        if (_stations.Count == 0)
        {
            types = new List<WagonType>() { WagonType.Green, WagonType.Red, WagonType.Blue };
            Debug.LogWarning("NO STATIONS, WAVE IS UNBEATABLE");
        }

        var wTypes = new List<WagonType>();
        for (int i = 0; i < length; i++)
        {
            wTypes.Add(types.PickRandom());
        }

        foreach (var wType in wTypes)
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

    public void FailGame()
    {
        if (GameOver) return;
        
        _gameOver = true;
        GameOverUI.Instance.Show();
    }
    
    public void RestartGame()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}