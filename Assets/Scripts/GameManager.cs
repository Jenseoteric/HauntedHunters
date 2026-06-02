using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int gridSize = 6;
    // changed from 8 to 6 for 6x6 grid

    [Header("Investigator State")]
    public int investigatorRow = 5;
    public int investigatorCol = 5;
    // no investigators in app . Useless 

    [Header("Ghost State")]
    public int ghostRow = 0;
    public int ghostCol = 0;

    [Header("Turn Resources")]
    public int maxMovesPerTurn  = 3;    // how many steps ghost can move per turn
    public int movesRemaining   = 3;    // counts down, resets on EndGhostTurn
    public bool hasShiftedBoard = false; // only one row/col shift allowed per turn

    [Header("Round Tracking")]
    public int roundNumber = 1;

    //grid 0 = open passage and 1 = wall
    //not in use yet tho ^
    public int[,] grid;

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
        grid = new int[gridSize, gridSize];

        //checkered placeholder until maze is created
        for (int r = 0; r < gridSize; r++)
            for (int c = 0; c < gridSize; c++)
                grid[r, c] = (r + c) % 2 == 0 ? 0 : 1;
    }

    //called when ghost presses end turn
    public void EndGhostTurn()
    {
        roundNumber++;
        movesRemaining  = maxMovesPerTurn;
        hasShiftedBoard = false;
    }

    //below is for maze shifting
    public void ShiftRow(int row, int direction)
    {
        if (direction == 1)
        {
            int wrapped = grid[row, gridSize - 1];
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
            int wrapped = grid[row, 0];
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
            int wrapped = grid[gridSize - 1, col];
            for (int r = gridSize - 1; r > 0; r--)
                grid[r, col] = grid[r - 1, col];
            grid[0, col] = wrapped;

            if (investigatorCol == col)
                investigatorRow = (investigatorRow + 1) % gridSize;
            if (ghostCol == col)
                ghostRow = (ghostRow + 1) % gridSize;
        }

        else

        {
            int wrapped = grid[0, col];
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
