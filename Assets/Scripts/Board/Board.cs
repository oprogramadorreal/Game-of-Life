using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public sealed class Board : MonoBehaviour, IBoard
{
    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private Vector2 boardSize = new Vector2(640, 480);

    [SerializeField]
    private Vector2 cellSize = new Vector2(1, 1);

    [SerializeField]
    private float cellsSpacing = 0.5f;

    [SerializeField]
    private float updateTime = 0.2f;

    [SerializeField]
    private BoardSwitch boardSwitch;

    /// <summary>
    /// Shapes added to the board only once.
    [SerializeField]
    private List<BoardObject> stampObjects = new List<BoardObject>();

    /// <summary>
    /// Shapes added to the board every frame.
    /// </summary>
    [SerializeField]
    private List<BoardObject> persistentObjects = new List<BoardObject>();

    [SerializeField]
    private List<InitialStamp> initialStamps = new List<InitialStamp>();

    [Serializable]
    private struct InitialStamp
    {
        public GameObject stampPrefab;
        public BoardCellIndex initialPosition;
    }

    [SerializeField]
    private GameObject coinCreationParticlesPrefab;

    private float timeAcc = 0.0f;

    private bool killAllCellsRequest = false;

    private Vector3 boardOrigin;
    private Vector2Int numberOfCells;
    
    private Mesh mesh;
    private Vector2[] meshUVsBuffer;

    private readonly List<BoardCell> cells = new List<BoardCell>();

    private readonly List<MetaCell> metaCells = new List<MetaCell>();

    private ObjectsPool coinsPool;

    private bool gameOver = false;

    public void OnGameOver()
    {
        gameOver = true;
    }

    public void SpeedUp()
    {
        const float step = 0.01f;
        const float minValue = 0.01f;

        var newValue = updateTime - step;

        updateTime = newValue < minValue ? minValue : newValue;
    }

    private void Awake()
    {
        SetupBoard();
        coinsPool = GetComponent<ObjectsPool>();
    }

    private void Start()
    {
        foreach (var s in initialStamps)
        {
            var gameObject = Instantiate(s.stampPrefab);
            gameObject.transform.position += CalculateGlobalCellCenter(s.initialPosition);
            AddStampObject(gameObject.GetComponent<BoardObject>());
        }
    }

    private void SetupBoard()
    {
        numberOfCells = new Vector2Int(
            Mathf.FloorToInt(boardSize.x / cellSize.x) + 2,
            Mathf.FloorToInt(boardSize.y / cellSize.y) + 2
        );

        var actualBoardSize = new Vector2(
            numberOfCells.x * cellSize.x + (numberOfCells.x - 1) * cellsSpacing,
            numberOfCells.y * cellSize.y + (numberOfCells.y - 1) * cellsSpacing
        );

        boardOrigin = new Vector3(
            -actualBoardSize.x / 2.0f,
            -actualBoardSize.y / 2.0f,
            0.0f
        );

        cells.Clear();

        for (var x = 0; x < numberOfCells.x; ++x)
        {
            for (var y = 0; y < numberOfCells.y; ++y)
            {
                cells.Add(new BoardCell(new BoardCellIndex(x, y)));
            }
        }

        mesh = CreateMesh();

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        var meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    private Mesh CreateMesh()
    {
        var totalNumberOfCells = numberOfCells.x * numberOfCells.y;

        var vertices = new List<Vector3>(totalNumberOfCells * 4);
        var triangles = new List<int>(totalNumberOfCells * 6);
        var uvs = new List<Vector2>(totalNumberOfCells * 4);

        ForEachCell(cell =>
        {
            var cellCenter = CalculateLocalCellCenter(cell.Index);

            var firstVertex = vertices.Count;

            vertices.Add(cellCenter + new Vector3(-cellSize.x / 2.0f, -cellSize.y / 2.0f, 0.0f));
            vertices.Add(cellCenter + new Vector3(cellSize.x / 2.0f, -cellSize.y / 2.0f, 0.0f));
            vertices.Add(cellCenter + new Vector3(cellSize.x / 2.0f, cellSize.y / 2.0f, 0.0f));
            vertices.Add(cellCenter + new Vector3(-cellSize.x / 2.0f, cellSize.y / 2.0f, 0.0f));

            triangles.Add(firstVertex);
            triangles.Add(firstVertex + 2);
            triangles.Add(firstVertex + 1);
                
            triangles.Add(firstVertex + 2);
            triangles.Add(firstVertex);
            triangles.Add(firstVertex + 3);

            if (IsBorder(cell.Index))
            {
                uvs.AddRange(BoardCellUVsFactory.GetTransparentUVs());
            }
            else
            {
                uvs.AddRange(BoardCellUVsFactory.GetUVsFor(cell.Type));
            }
        });

        var mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32; // needed?
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        return mesh;
    }

    private void ForEachCell(Action<BoardCell> action)
    {
        foreach (var c in cells) // TODO: parallel for
        {
            action(c);
        }
    }

    private int ToIndex1D(BoardCellIndex index)
    {
        return index.X * numberOfCells.y + index.Y;
    }

    private BoardCellIndex ToIndex2D(int index)
    {
        var numberOfColumns = numberOfCells.y;
        return new BoardCellIndex(index/ numberOfColumns, index % numberOfColumns);
    }

    public Vector3 CalculateGlobalCellCenter(BoardCellIndex index)
    {
        return transform.TransformPoint(CalculateLocalCellCenter(index));
    }

    private Vector3 CalculateLocalCellCenter(BoardCellIndex index)
    {
        return boardOrigin
            + new Vector3(index.X * cellSize.x, index.Y * cellSize.y, 0)
            + new Vector3(cellSize.x / 2.0f, cellSize.y / 2.0f, 0.0f)
            + new Vector3(index.X * cellsSpacing, index.Y * cellsSpacing, 0.0f);
    }

    private void Update()
    {
        if (!gameOver)
        {
            meshUVsBuffer = mesh.uv;

            if (killAllCellsRequest)
            {
                ForEachCell(cell => ChangeCellType(cell, BoardCellType.Dead));
                killAllCellsRequest = false;
            }

            TurnPlayerCellsIntoAliveCells();
            ProcessBoardObjects();
            ProcessMetaCells();
            UpdateBoardStateIfNecessary();
        }
    }

    private void ProcessMetaCells()
    {
        foreach (var c in metaCells)
        {
            ChangeCellType(c.Index, c.Type);
        }

        metaCells.Clear();
    }

    public void Add(MetaCell cell)
    {
        metaCells.Add(cell);
    }

    private void TurnPlayerCellsIntoAliveCells()
    {
        ForEachCell(cell =>
        {
            if (cell.Type == BoardCellType.Player)
            {
                ChangeCellType(cell, BoardCellType.Alive);
            }
        });
    }

    private void ProcessBoardObjects()
    {
        ChangeCellTypes(stampObjects);
        DestroyObjects(stampObjects);
        stampObjects.Clear();

        ChangeCellTypes(persistentObjects);
    }

    public void AddStampObject(BoardObject obj)
    {
        stampObjects.Add(obj);
    }

    public void AddPersistentObject(BoardObject obj)
    {
        persistentObjects.Add(obj);
    }

    private static void DestroyObjects(IEnumerable<BoardObject> objects)
    {
        foreach (var o in objects)
        {
            Destroy(o.gameObject);
        }
    }

    private void ChangeCellTypes(IEnumerable<IBoardObject> objects)
    {
        foreach (var obj in objects)
        {
            var objCells = obj.GetCellsOn(this);

            foreach (var c in objCells)
            {
                ChangeCellType(c.Index, c.Type);
            }
        }
    }

    private void LateUpdate()
    {
        mesh.uv = meshUVsBuffer;
    }

    public BoardCellIndex? PickCell(Ray ray)
    {
        BoardCellIndex? index = null;

        if (Physics.Raycast(ray, out var hit))
        {
            index = PickCell(hit.point);
            //Debug.Log(index.Value.X + " " + index.Value.Y);
        }

        return index;
    }

    public BoardCellIndex PickCell(Vector3 point)
    {
        var p = transform.InverseTransformPoint(point) - boardOrigin;

        var index = new BoardCellIndex(
            Mathf.FloorToInt(p.x / (cellSize.x + cellsSpacing)),
            Mathf.FloorToInt(p.y / (cellSize.y + cellsSpacing))
        );

        return index;
    }

    private void ChangeCellType(BoardCell cell, BoardCellType newCellType)
    {
        var cellIndex1d = ToIndex1D(cell.Index);
        ChangeCellType(cell, cellIndex1d, newCellType);
    }

    private void ChangeCellType(BoardCellIndex index, BoardCellType newCellType)
    {
        if (IsValidCellIndex(index))
        {
            var cellIndex1d = ToIndex1D(index);
            var cell = cells[cellIndex1d];
            ChangeCellType(cell, cellIndex1d, newCellType);
        }
    }

    private void ChangeCellType(BoardCell cell, int cellIndex1d, BoardCellType newCellType)
    {
        var newUVs = GetNewUVs(newCellType, cell.Index);
        var firstUvIndex = cellIndex1d * 4;

        for (var i = 0; i < 4; ++i)
        {
            meshUVsBuffer[firstUvIndex + i] = newUVs[i];
        }

        cell.Type = newCellType;
    }

    private Vector2[] GetNewUVs(BoardCellType newCellType, BoardCellIndex cellIndex)
    {
        if (IsBorder(cellIndex))
        {
            return BoardCellUVsFactory.GetTransparentUVs();
        }
        else
        {
            return BoardCellUVsFactory.GetUVsFor(newCellType);
        }
    }

    public bool IsBorder(BoardCellIndex cellIndex)
    {
        return IsLeftBorder(cellIndex)
            || IsRightBorder(cellIndex)
            || IsBottomBorder(cellIndex)
            || IsTopBorder(cellIndex);
    }

    private bool IsLeftBorder(BoardCellIndex cellIndex)
    {
        return cellIndex.X == 0;
    }

    private bool IsRightBorder(BoardCellIndex cellIndex)
    {
        return cellIndex.X == numberOfCells.x - 1;
    }

    private bool IsBottomBorder(BoardCellIndex cellIndex)
    {
        return cellIndex.Y == 0;
    }

    private bool IsTopBorder(BoardCellIndex cellIndex)
    {
        return cellIndex.Y == numberOfCells.y - 1;
    }

    private bool IsValidCellIndex(BoardCellIndex index)
    {
        return index.X >= 0
            && index.X < numberOfCells.x
            && index.Y >= 0
            && index.Y < numberOfCells.y;
    }

    public void KillAllCells()
    {
        killAllCellsRequest = true;
    }

    private void UpdateBoardStateIfNecessary()
    {
        if (boardSwitch == null || boardSwitch.IsOn())
        {
            timeAcc += Time.deltaTime;

            if (timeAcc >= updateTime)
            {
                UpdateBoardState();
                timeAcc = 0.0f;
            }
        }
    }

    private void UpdateBoardState()
    {
        // Calculate the "NextType" for each cell.
        ForEachCell(cell =>
        {
            cell.NextType = cell.Type;

            var numberOfAliveNeighbors = CountAliveNeighborsOf(cell.Index);

            if (cell.Type == BoardCellType.Alive)
            {
                if (numberOfAliveNeighbors <= 1 || numberOfAliveNeighbors >= 4)
                {
                    cell.NextType = BoardCellType.Dead;
                }
                else
                {
                    cell.NextType = BoardCellType.Alive; // Still alive. Deserves a cake.
                }
            }
            else if (cell.Type == BoardCellType.Dead)
            {
                if (numberOfAliveNeighbors == 3)
                {
                    if (IsBorder(cell.Index))
                    {
                        CreateCoin(cell.Index);
                    }

                    cell.NextType = BoardCellType.Alive;
                }
            }
        });

        // Change each cell "Type" based on the calculated "NextType".
        ForEachCell(cell =>
        {
            if (cell.Type != cell.NextType)
            {
                ChangeCellType(cell, cell.NextType);
            }
        });
    }

    private void CreateCoin(BoardCellIndex cellIndex)
    {
        if (coinCreationParticlesPrefab != null && audioManager != null)
        {
            var position = CalculateGlobalCellCenter(cellIndex);
            var coin = coinsPool.Instantiate(position);
            coin.transform.localScale = cellSize;

            if (IsLeftBorder(cellIndex))
            {
                coin.GetComponent<Rigidbody2D>().AddForce(Vector2.left * UnityEngine.Random.Range(5.0f, 25.0f), ForceMode2D.Impulse);
            }
            else if (IsRightBorder(cellIndex))
            {
                coin.GetComponent<Rigidbody2D>().AddForce(Vector2.right * UnityEngine.Random.Range(5.0f, 25.0f), ForceMode2D.Impulse);
            }
            else if (IsTopBorder(cellIndex))
            {
                coin.GetComponent<Rigidbody2D>().AddForce(Vector2.up * UnityEngine.Random.Range(15.0f, 25.0f), ForceMode2D.Impulse);
            }
            else // is bottom border
            {
                coin.GetComponent<Rigidbody2D>().AddForce(Vector2.down * UnityEngine.Random.Range(0.0f, 10.0f), ForceMode2D.Impulse);
            }

            var particles = Instantiate(coinCreationParticlesPrefab, position, Quaternion.identity);
            Destroy(particles, 0.5f);

            audioManager.CreateTemporaryAudioSource("CoinCreation");
        }
    }

    private int CountAliveNeighborsOf(BoardCellIndex index)
    {
        var neighborsTypes = new BoardCellType[]
        {
            GetCellType(new BoardCellIndex(index.X - 1, index.Y - 1)),
            GetCellType(new BoardCellIndex(index.X - 1, index.Y)),
            GetCellType(new BoardCellIndex(index.X - 1, index.Y + 1)),
            GetCellType(new BoardCellIndex(index.X, index.Y + 1)),
            GetCellType(new BoardCellIndex(index.X, index.Y - 1)),
            GetCellType(new BoardCellIndex(index.X + 1, index.Y - 1)),
            GetCellType(new BoardCellIndex(index.X + 1, index.Y)),
            GetCellType(new BoardCellIndex(index.X + 1, index.Y + 1))
        };

        var aliveCount = 0;

        foreach (var nType in neighborsTypes)
        {
            if (nType == BoardCellType.Alive)
            {
                ++aliveCount;
            }
        }

        return aliveCount;
    }

    private BoardCellType GetCellType(BoardCellIndex index)
    {
        return IsValidCellIndex(index) ?
            cells[ToIndex1D(index)].Type : BoardCellType.Dead;
    }
}
