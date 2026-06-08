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

    [Header("Maze Presets")]
    //drag both PresetBoard components here in the Inspector
    [SerializeField] private PresetBoard presetBoard;
    [SerializeField] private PresetBoard2 presetBoard2;
    [SerializeField] private PresetBoard3 presetBoard3;
    //which map to load. set by the map select screen. 0 = first preset, 1 = second
    public static int selectedMap = 0;

    [Header("Investigator State")]
    public int investigatorRow = 5;
    public int investigatorCol = 5;
    // no investigators in app . Useless 

    [Header("Ghost State")]
    public int ghostRow = 0;
    public int ghostCol = 0;
    public int maxMovesPerTurn = 3;
    public int movesRemaining = 3;
    public bool hasShiftedBoard = false;
    public int roundNumber = 1;
    //round 0 is the free placement round. once the ghost ends turn 0, real rounds begin at 1
    public bool isPlacementRound = true; //true during round 0, lets ghost move freely with no limits

    //grid 0 = open passage and 1 = wall
    //not in use yet tho ^
    public CellType[,] grid; // used to be 'int[,] instead of CellType[,]. this change is done so tiles can be realised on the app


    // pressing undo restores the player state to the state it was at the beginning of the turn
    private CellType[,] snapGrid;
    private int snapGhostRow, snapGhostCol;
    private int snapMoves, snapRound;
    private bool snapShifted, snapPlacement;
    private bool hasSnapshot = false; //false until at least one turn has begun

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

        //during placement round the ghost can move/teleport anywhere, skip the wall check
        if (isPlacementRound)
            return true;

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
        TakeSnapshot(); //snapshot the freshly built board so undo works from turn 1
    }

    void InitialiseGrid()
    {
        grid = new CellType[gridSize, gridSize];

        //load whichever preset the map select screen chose
        if (selectedMap == 1 && presetBoard2 != null)
        {
            presetBoard2.Apply(grid, gridSize);
        }
        else if (selectedMap == 2 && presetBoard3 != null)
        {
            presetBoard3.Apply(grid, gridSize);
        }
        else if (presetBoard != null)
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

    //
    // SNAPSHOT SYSTEM
    // TakeSnapshot saves the current state. called at the start of each turn.
    // UndoToSnapshot rolls everything back to that saved state (one turn back).
    //
    public void TakeSnapshot()
    {
        snapGrid = new CellType[gridSize, gridSize];
        for (int r = 0; r < gridSize; r++)
            for (int c = 0; c < gridSize; c++)
                snapGrid[r, c] = grid[r, c];

        snapGhostRow = ghostRow;
        snapGhostCol = ghostCol;
        snapMoves = movesRemaining;
        snapRound = roundNumber;
        snapShifted = hasShiftedBoard;
        snapPlacement = isPlacementRound;
        hasSnapshot = true;
    }

    //returns false if theres nothing to undo to
    public bool UndoToSnapshot()
    {
        if (!hasSnapshot)
            return false;

        for (int r = 0; r < gridSize; r++)
            for (int c = 0; c < gridSize; c++)
                grid[r, c] = snapGrid[r, c];

        ghostRow = snapGhostRow;
        ghostCol = snapGhostCol;
        movesRemaining = snapMoves;
        roundNumber = snapRound;
        hasShiftedBoard = snapShifted;
        isPlacementRound = snapPlacement;
        return true;
    }

    //called when ghost presses end turn
    public void EndGhostTurn()
    {
        //leaving round 0 (placement) means the real game starts now
        if (isPlacementRound)
            isPlacementRound = false;

        roundNumber++;
        movesRemaining = maxMovesPerTurn;
        hasShiftedBoard = false;

        TakeSnapshot(); //save the state at the start of the new turn so undo can return here
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
