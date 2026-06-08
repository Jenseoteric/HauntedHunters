using UnityEngine;

public class PresetBoard : MonoBehaviour
{
    //called by GameManager to write the maze into the grid array
    public void Apply(CellType[,] grid, int gridSize)
    {
        if (gridSize != 6)
        {
            Debug.LogWarning("PresetBoard: designed for 6x6, gridSize mismatch");
            return;
        }

        grid[0, 0] = CellType.CornerSouthEast;
        grid[0, 1] = CellType.TJunctionSouth;
        grid[0, 2] = CellType.CorridorEastWest;
        grid[0, 3] = CellType.TJunctionSouth;
        grid[0, 4] = CellType.CorridorEastWest;
        grid[0, 5] = CellType.CornerSouthWest;


        // ROW 1 

        grid[1, 0] = CellType.TJunctionEast;
        grid[1, 1] = CellType.Open;
        grid[1, 2] = CellType.CorridorEastWest;
        grid[1, 3] = CellType.Open;
        grid[1, 4] = CellType.CornerSouthWest;
        grid[1, 5] = CellType.CorridorNorthSouth;


        // ROW 2

        grid[2, 0] = CellType.TJunctionEast;
        grid[2, 1] = CellType.TJunctionNorth;
        grid[2, 2] = CellType.CornerSouthEast;
        grid[2, 3] = CellType.CornerNorthWest;
        grid[2, 4] = CellType.CorridorNorthSouth;
        grid[2, 5] = CellType.TJunctionWest;


        // ROW 3

        grid[3, 0] = CellType.CorridorNorthSouth;
        grid[3, 1] = CellType.CornerSouthEast;
        grid[3, 2] = CellType.TJunctionWest;
        grid[3, 3] = CellType.CornerSouthEast;
        grid[3, 4] = CellType.CornerNorthWest;
        grid[3, 5] = CellType.TJunctionWest;


        // ROW 4 

        grid[4, 0] = CellType.TJunctionEast;
        grid[4, 1] = CellType.TJunctionNorth;
        grid[4, 2] = CellType.Open;
        grid[4, 3] = CellType.TJunctionNorth;
        grid[4, 4] = CellType.CorridorEastWest;
        grid[4, 5] = CellType.TJunctionWest;


        // ROW 5 

        grid[5, 0] = CellType.CornerNorthEast;
        grid[5, 1] = CellType.CorridorEastWest;
        grid[5, 2] = CellType.TJunctionNorth;
        grid[5, 3] = CellType.CorridorEastWest;
        grid[5, 4] = CellType.CorridorEastWest;
        grid[5, 5] = CellType.CornerNorthWest;
    }
}
