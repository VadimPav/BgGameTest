using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



public class LabyrinthCreator : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    private Vector3[] Path { get; set; }

    public event Action<Vector3[]> OnPathChanged;

    [SerializeField] private int labyrinthSize = 10;
    [SerializeField] private int deathZonesCount = 10;


    [SerializeField] private Finish finishZone;
    [SerializeField] private Trap deathZone;
    [SerializeField] private GameObject pathPrefab;
    [SerializeField] private LabyrinthSquare labyrinthCell;

    private Finish _finish;
    private List<Trap> _traps = new List<Trap>();

    private float _cellSize = 3;
    private float _borderSize = 3;

    private LabyrinthGeneratorCell[,] _labyrinth;



    private void Start()
    {
        var cell = labyrinthCell;
        if (!cell) return;

        _cellSize = cell.cellSize;
        _borderSize = cell.borderSize;

        GenerateLabyrinth();
    }

    public void GenerateLabyrinth()
    {
        ClearLabyrinth();
        InitializeMaze();
        BuildMaze();
        InstantiateLabyrinth();
        GenerateDeathZones();
        
        _finish = Instantiate(finishZone, ConvertToPositionXZ(labyrinthSize - 1, labyrinthSize - 1), Quaternion.identity, transform);
        _finish.OnGameFinished += OnGameFinished;
        
        _gameManager.OnLabyrinthGenerated();
    }

    private void OnGameFinished()
    {
        _gameManager.Finish();
    }

    private void ClearLabyrinth()
    {
        if (transform.childCount == 0) return;

        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    private void InitializeMaze()
    {
        _labyrinth = new LabyrinthGeneratorCell[labyrinthSize, labyrinthSize];

        for (int x = 0; x < labyrinthSize; x++)
            for (int y = 0; y < labyrinthSize; y++)
                _labyrinth[x, y] = new LabyrinthGeneratorCell(x, y);
    }

    private void BuildMaze()
    {
        var currentCell = _labyrinth[0, 0];
        currentCell.IsVisited = true;
        bool isPeek = false;


        Stack<LabyrinthGeneratorCell> stack = new Stack<LabyrinthGeneratorCell>();
        do
        {
            Utilites.GetUnvisitedCellsAround(currentCell.X, currentCell.Y, out var unvisitedCells, _labyrinth, labyrinthSize);

            if (unvisitedCells.Count > 0)
            {
                var nextCell = unvisitedCells[Random.Range(0, unvisitedCells.Count)];
                nextCell.IsVisited = true;
                stack.Push(nextCell);
                ChangeBordersVisibility(ref currentCell, ref nextCell);
                currentCell = nextCell;

                if (nextCell.X == labyrinthSize - 1 && nextCell.Y == labyrinthSize - 1) GeneratePath(ref stack);

                isPeek = false;
            }
            else
            {
                if (!isPeek)
                {
                    currentCell = stack.Peek();
                    isPeek = true;
                }
                else
                {
                    currentCell = stack.Pop();
                    isPeek = false;
                }
            }
        } while (stack.Count > 0);
    }



    private void ChangeBordersVisibility(ref LabyrinthGeneratorCell firstCell, ref LabyrinthGeneratorCell secondCell)
    {
        if (firstCell.X == secondCell.X)
        {
            if (firstCell.Y > secondCell.Y)
            {
                firstCell.IsWallDownActive = false;
                secondCell.IsWallTopActive = false;
            }
            else
            {
                secondCell.IsWallDownActive = false;
                firstCell.IsWallTopActive = false;
            }
        }
        else
        {
            if (firstCell.X > secondCell.X)
            {
                firstCell.IsWallLeftActive = false;
                secondCell.IsWallRightActive = false;
            }
            else
            {
                secondCell.IsWallLeftActive = false;
                firstCell.IsWallRightActive = false;
            }
        }
    }

    private void GeneratePath(ref Stack<LabyrinthGeneratorCell> stack)
    {
        if (pathPrefab == null) return;

        var position = new Vector3(0, -0.49f, 0);
        var line = Instantiate(pathPrefab, position, Quaternion.identity, transform).GetComponent<LineRenderer>();
        int index = 0;
        line.positionCount = stack.Count + 1;
        Path = new Vector3[stack.Count];

        foreach (var cell in stack)
        {
            line.SetPosition(index, ConvertToPositionXY(cell.X, cell.Y));
            Path[index] = ConvertToPositionXZ(cell.X, cell.Y);
            index++;
        }

        line.SetPosition(index, ConvertToPositionXY(0, 0));
        line.transform.rotation = Quaternion.Euler(90f, 0, 0);

        OnPathChanged?.Invoke(Path);
    }

    private void InstantiateLabyrinth()
    {
        for (int x = 0; x < labyrinthSize; x++)
        {
            for (int y = 0; y < labyrinthSize; y++)
            {
                var cell = Instantiate(labyrinthCell, ConvertToPositionXZ(x, y), Quaternion.identity, transform);

                cell.wallLeft.SetActive(_labyrinth[x, y].IsWallLeftActive);
                cell.wallTop.SetActive(_labyrinth[x, y].IsWallTopActive);
                cell.wallRight.SetActive(_labyrinth[x, y].IsWallRightActive);
                cell.wallDown.SetActive(_labyrinth[x, y].IsWallDownActive);
            }
        }
    }
    private Vector3 ConvertToPositionXZ(int x, int y)
    {
        float cellSize = _cellSize - _borderSize;
        return new Vector3(x * cellSize, 0f, y * cellSize);
    }

    private Vector3 ConvertToPositionXY(int x, int y)
    {
        float cellSize = _cellSize - _borderSize;
        return new Vector3(x * cellSize, y * cellSize, 0);
    }
    private void GenerateDeathZones()
    {
        for (int i = 0; i < deathZonesCount; i++)
        {
            int x = Random.Range(0, labyrinthSize);
            int y = Random.Range(x == 0 ? 1 : 0, x == labyrinthSize - 1 ? labyrinthSize - 1 : labyrinthSize);

           var trap = Instantiate(deathZone, ConvertToPositionXZ(x, y), Quaternion.identity, transform);
           trap.OnPlayerDeath += OnPlayerDeath;
           
           _traps.Add(trap);
        }
    }

    private void OnPlayerDeath()
    {
        _gameManager._player.TryToKill();
    }

    private void OnDestroy()
    {
        _finish.OnGameFinished = null;
        foreach (var trap in _traps)
        {
            trap.OnPlayerDeath = null;
        }
    }
}
