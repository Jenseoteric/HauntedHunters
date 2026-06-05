using UnityEngine;
using System.Collections.Generic;
public enum CellType
{
    Open = 0,
    Closed = 1,
    CorridorEastWest = 2,
    CorridorNorthSouth = 3,
    CornerNorthEast = 4,
    CornerNorthWest = 5,
    CornerSouthEast = 6,
    CornerSouthWest = 7,
    TJunctionNorth = 8,
    TJunctionSouth = 9,
    TJunctionEast = 10,
    TJunctionWest = 11,
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int gridSize = 6;
    // changed from 8 to 6 for 6x6 grid

    [Header("Maze Preset")]
    //drag PresetBoard component here in the Inspector to load a maze layout
    [SerializeField] private PresetBoard presetBoard;

    [Header("Investigator State")]
    public int investigatorRow = 5;
    public int investigatorCol = 5;
    // no investigators in app . Useless 

    [Header("Ghost State")]
    public int ghostRow = 0;
    public int ghostCol = 0;

    [Header("Turn Resources")]
    public int maxMovesPerTurn = 3;    //how many steps ghost can move per turn
    public int movesRemaining = 3;    //counts down, resets on EndGhostTurn
    public bool hasShiftedBoard = false; //only one row/col shift allowed per turn

    [Header("Round Tracking")]
    public int roundNumber = 1;

    //grid 0 = open passage and 1 = wall
    //not in use yet tho ^
    public CellType[,] grid; // used to be 'int[,] instead of CellType[,]. this change is done so tiles can be realised on the app

    private static readonly Dictionary<CellType, bool[]> exits = new Dictionary<CellType, bool[]>
{
    //directions correspond below:              North  South  East   West
    { CellType.Open,               new bool[] { true,  true,  true,  true  } },
    { CellType.Closed,             new bool[] { false, false, false, false } },
    { CellType.CorridorEastWest,   new bool[] { false, false, true,  true  } },
    { CellType.CorridorNorthSouth, new bool[] { true,  true,  false, false } },
    { CellType.CornerNorthEast,    new bool[] { true,  false, true,  false } },
    { CellType.CornerNorthWest,    new bool[] { true,  false, false, true  } },
    { CellType.CornerSouthEast,    new bool[] { false, true,  true,  false } },
    { CellType.CornerSouthWest,    new bool[] { false, true,  false, true  } },
    { CellType.TJunctionNorth,     new bool[] { true,  false, true,  true  } },
    { CellType.TJunctionSouth,     new bool[] { false, true,  true,  true  } },
    { CellType.TJunctionEast,      new bool[] { true,  true,  true,  false } },
    { CellType.TJunctionWest,      new bool[] { true,  true,  false, true  } },
    //^ the directional names at the end of the cell type's name simply correspond to which directions are accessible for the ghost.
    // example being TJunctionNorth, meaning the opposite of north (south obviously) is where the impassable wall part of the junction is placed.
};

    private const int NORTH = 0, SOUTH = 1, EAST = 2, WEST = 3;

    public bool CanMove(int fromRow, int fromCol, int toRow, int toCol)
    {
        if (toRow < 0 || toRow >= gridSize || toCol < 0 || toCol >= gridSize)

            return false;

        int dRow = toRow - fromRow;
        int dCol = toCol - fromCol;

        int exitDir, entryDir;
        if (dRow == -1) { exitDir = NORTH; entryDir = SOUTH; }
        else if (dRow == 1) { exitDir = SOUTH; entryDir = NORTH; }
        else if (dCol == 1) { exitDir = EAST; entryDir = WEST; }
        else if (dCol == -1) { exitDir = WEST; entryDir = EAST; }
        else return false;

        return exits[grid[fromRow, fromCol]][exitDir] && exits[grid[toRow, toCol]][entryDir];
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitialiseGrid();
    }

    void InitialiseGrid()
    {
        grid = new CellType[gridSize, gridSize];

        if (presetBoard != null)
        {
            //load maze layout from the linked PresetBoard script
            presetBoard.Apply(grid, gridSize);
        }
        else
        {
            //fallback if no preset is assigned - all open so the ghost can still move
            Debug.LogWarning("no PresetBoard assigned in GameManager inspector, defaulting to all Open");
            for (int r = 0; r < gridSize; r++)
                for (int c = 0; c < gridSize; c++)
                    grid[r, c] = CellType.Open;
        }
        //all open placeholder until real maze preset is designed !!!

        //old version with checkered grid.
        /*
        grid = new int[gridSize, gridSize];

        //checkered placeholder until maze is created
        for (int r = 0; r < gridSize; r++)
            for (int c = 0; c < gridSize; c++)
                grid[r, c] = (r + c) % 2 == 0 ? 0 : 1; */
    }

    //called when ghost presses end turn
    public void EndGhostTurn()
    {
        roundNumber++;
        movesRemaining = maxMovesPerTurn;
        hasShiftedBoard = false;
    }

    //below is for maze shifting
    public void ShiftRow(int row, int direction)
    {
        if (direction == 1)
        {
            CellType wrapped = grid[row, gridSize - 1];
            for (int c = gridSize - 1; c > 0; c--)
                grid[row, c] = grid[row, c - 1];
            grid[row, 0] = wrapped;

            //below is so the ghost / possible other items are moved with grid
            if (investigatorRow == row)
                investigatorCol = (investigatorCol + 1) % gridSize;
            if (ghostRow == row)
                ghostCol = (ghostCol + 1) % gridSize;
        }

        else

        {
            CellType wrapped = grid[row, 0];
            for (int c = 0; c < gridSize - 1; c++)
                grid[row, c] = grid[row, c + 1];
            grid[row, gridSize - 1] = wrapped;

            if (investigatorRow == row)
                investigatorCol = (investigatorCol - 1 + gridSize) % gridSize;
            if (ghostRow == row)
                ghostCol = (ghostCol - 1 + gridSize) % gridSize;
        }

        hasShiftedBoard = true;
    }

    //same deal but for column shifting
    public void ShiftColumn(int col, int direction)
    {
        if (direction == 1)
        {
            CellType wrapped = grid[gridSize - 1, col];
            for (int r = gridSize - 1; r > 0; r--)
                grid[r, col] = grid[r - 1, col];
            grid[0, col] = wrapped;

            if (investigatorCol == col)
                investigatorRow = (investigatorRow + 1) % gridSize;
            if (ghostCol == col)
                ghostRow = (ghostRow + 1) % gridSize;
        }

        else //cell as a class, you could have cell celltype, but also cell location which is where you can store B1 cell value, etc. instead of the enum method

        {
            CellType wrapped = grid[0, col];
            for (int r = 0; r < gridSize - 1; r++)
                grid[r, col] = grid[r + 1, col];
            grid[gridSize - 1, col] = wrapped;

            if (investigatorCol == col)
                investigatorRow = (investigatorRow - 1 + gridSize) % gridSize;
            if (ghostCol == col)
                ghostRow = (ghostRow - 1 + gridSize) % gridSize;
        }

        hasShiftedBoard = true;
    }
}
