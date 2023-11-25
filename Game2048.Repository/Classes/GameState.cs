using Game2048.Config;
using Game2048.Shared.Enums;
using Game2048.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Game2048.Repository;

public class GameState : IRepositoryState, IGameState
{
    IList<IList<int>> grid;
    public IList<IList<int>> Grid { get => grid; protected set { grid = value; } }

    int score;

    public int Score { get => score; protected set { score = value; } }

    /// <summary>
    /// Returns true if movement is possible, else false.
    /// </summary>
    public bool CanMove => CheckIfCanMove();

    public GameState()
    {
        grid = new List<IList<int>>();
        for (int i = 0; i < PlayEnvironment.GridHeight; i++)
        {
            grid.Add(new List<int>());
            for (int j = 0; j < PlayEnvironment.GridWidth; j++)
            {
                grid[i].Add(0);
            }
        }
    }

    public void PlaceTile(int vertical, int horizontal, int tileValue)
    {
        grid[vertical][horizontal] = tileValue;
    }

    public IList<(int Vertical, int Horizontal)> GetEmptyTiles()
    {
        var result = new List<(int Vertical, int Horizontal)>();
        for (int i = 0; i < grid.Count; i++)
        {
            for (int j = 0; j < grid[i].Count; j++)
            {
                if (grid[i][j] == 0)
                {
                    result.Add((i, j));
                }
            }
        }
        return result;
    }

    bool CheckIfCanMove()
    {
        // Check if there is any empty tile on the grid
        if (grid.Any(row => row.Contains(0)))
        {
            return true;
        }
        // Check if there are similar tiles horizontally
        for (int i = 0; i < PlayEnvironment.GridHeight; i++)
        {
            for (int j = 0; j < PlayEnvironment.GridWidth - 1; j++)
            {
                if (grid[i][j] == grid[i][j + 1])
                {
                    return true;
                }
            }
        }
        // Check if there are similar tiles vertically
        for (int i = 0; i < PlayEnvironment.GridWidth; i++)
        {
            for (int j = 0; j < PlayEnvironment.GridHeight - 1; j++)
            {
                if (grid[j][i] == grid[j + 1][i])
                {
                    return true;
                }
            }
        }
        return false;
    }

    public IRepositoryState Copy()
    {
        var result = new GameState();
        IList<IList<int>> resultGrid = new List<IList<int>>();
        for (int i = 0; i < grid.Count; i++)
        {
            resultGrid.Add(new List<int>());
            for (int j = 0; j < grid[i].Count; j++)
            {
                resultGrid[i].Add(grid[i][j]);
            }
        }
        int resultScore = score;
        result.Grid = resultGrid;
        result.Score = resultScore;
        return result;
    }

    public void Deserialize(string deserializee)
    {
        var deserializedData = JsonSerializer.Deserialize<(IList<IList<int>>, int)>(deserializee);
        grid = deserializedData.Item1;
        score = deserializedData.Item2;
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize<(IList<IList<int>>, int)>((grid, score));
    }

    public void Move(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Right:
                {
                    MoveRigth();
                    break;
                }
            case MoveDirection.Down:
                {
                    MoveDown();
                    break;
                }
            case MoveDirection.Left:
                {
                    MoveLeft();
                    break;
                }
            case MoveDirection.Up:
                {
                    MoveUp();
                    break;
                }
            default:
                {
                    throw new ArgumentException("Invalid direction.");
                }
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is not null && obj is GameState other)
        {
            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0; j < grid[i].Count; j++)
                {
                    if (grid[i][j] != other.Grid[i][j])
                    {
                        return false;
                    }
                }
            }
            if (score != other.Score)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Performs movement on the grid to the right.
    /// </summary>
    void MoveRigth()
    {
        // We need to iterate over all rows
        for (int row = 0; row < grid.Count; row++)
        {
            //Setting first column to be the rightmost index - 1
            int firstColumn = grid[row].Count - 2;
            //Setting last column to be the leftmost index
            int lastColumn = 0;
            int rightmostColumn = grid[row].Count - 1;
            // First we need to sweep all tiles right, to remove empty tiles
            for (int column = firstColumn; column >= lastColumn; column--)
            {
                int currentColumn = column;
                while (currentColumn < rightmostColumn && EmptyLogic(row, currentColumn, row, currentColumn + 1))
                {
                    int numberToMove = grid[row][currentColumn];
                    grid[row][currentColumn] = 0;
                    grid[row][currentColumn + 1] = numberToMove;
                    currentColumn++;
                }
            }
            // Then we need to check and perform additions
            for (int column = firstColumn; column >= lastColumn; column--)
            {
                if (AdditionLogic(row, column, row, column + 1))
                {
                    int numberToMove = grid[row][column];
                    grid[row][column] = 0;
                    grid[row][column + 1] = numberToMove * 2;
                    score += numberToMove * 2;
                    column--;
                }
            }
            // And last we need to perform an empty check again
            for (int column = firstColumn - 1; column >= lastColumn; column--)
            {
                int currentColumn = column;
                while (currentColumn < rightmostColumn - 1 && EmptyLogic(row, currentColumn, row, currentColumn + 1))
                {
                    int numberToMove = grid[row][currentColumn];
                    grid[row][currentColumn] = 0;
                    grid[row][currentColumn + 1] = numberToMove;
                    currentColumn++;
                }
            }
        }
    }

    /// <summary>
    /// Performs movement on the grid to the left.
    /// </summary>
    void MoveLeft()
    {
        // We need to iterate over all rows
        for (int row = grid.Count - 1; row >= 0; row--)
        {
            //Setting first column to be the leftmost index + 1
            int firstColumn = 1;
            //Setting last column to be the rightmost index
            int lastColumn = grid[row].Count - 1;
            int leftmostColumn = 0;
            // First we need to sweep all tiles left, to remove empty tiles
            for (int column = firstColumn; column <= lastColumn; column++)
            {
                int currentColumn = column;
                while (currentColumn > leftmostColumn && EmptyLogic(row, currentColumn, row, currentColumn - 1))
                {
                    int numberToMove = grid[row][currentColumn];
                    grid[row][currentColumn] = 0;
                    grid[row][currentColumn - 1] = numberToMove;
                    currentColumn--;
                }
            }
            // Then we need to check and perform additions
            for (int column = firstColumn; column <= lastColumn; column++)
            {
                if (AdditionLogic(row, column, row, column - 1))
                {
                    int numberToMove = grid[row][column];
                    grid[row][column] = 0;
                    grid[row][column - 1] = numberToMove * 2;
                    score += numberToMove * 2;
                    column++;
                }
            }
            // And last we need to perform an empty check and sweep again
            for (int column = firstColumn + 1; column <= lastColumn; column++)
            {
                int currentColumn = column;
                while (currentColumn > leftmostColumn + 1 && EmptyLogic(row, currentColumn, row, currentColumn - 1))
                {
                    int numberToMove = grid[row][currentColumn];
                    grid[row][currentColumn] = 0;
                    grid[row][currentColumn - 1] = numberToMove;
                    currentColumn--;
                }
            }
        }
    }

    /// <summary>
    /// Performs movement on the grid upwards.
    /// </summary>
    void MoveUp()
    {
        // We need to iterate over all columns
        for (int column = 0; column < grid[0].Count; column++)
        {
            //Setting first row to be the upmost index + 1
            int firstRow = 1;
            //Setting last row to be the downmost index
            int lastRow = grid.Count - 1;
            int upmostRow = 0;
            // First we need to sweep all tiles up, to remove empty tiles
            for (int row = firstRow; row <= lastRow; row++)
            {
                int currentRow = row;
                while (currentRow > upmostRow && EmptyLogic(currentRow, column, currentRow - 1, column))
                {
                    int numberToMove = grid[currentRow][column];
                    grid[currentRow][column] = 0;
                    grid[currentRow - 1][column] = numberToMove;
                    currentRow--;
                }
            }
            // Then we need to check and perform additions
            for (int row = firstRow; row <= lastRow; row++)
            {
                if (AdditionLogic(row, column, row - 1, column))
                {
                    int numberToMove = grid[row][column];
                    grid[row][column] = 0;
                    grid[row - 1][column] = numberToMove * 2;
                    score += numberToMove * 2;
                    row++;
                }
            }
            // And last we need to perform an empty check and sweep again
            for (int row = firstRow + 1; row <= lastRow; row++)
            {
                int currentRow = row;
                while (currentRow > upmostRow + 1 && EmptyLogic(currentRow, column, currentRow - 1, column))
                {
                    int numberToMove = grid[currentRow][column];
                    grid[currentRow][column] = 0;
                    grid[currentRow - 1][column] = numberToMove;
                    currentRow--;
                }
            }
        }
    }

    /// <summary>
    /// Performs movement on the grid downwards.
    /// </summary>
    void MoveDown()
    {
        // We need to iterate over all columns
        for (int column = grid[0].Count - 1; column >= 0; column--)
        {
            //Setting first row to be the downmost index - 1
            int firstRow = grid[0].Count - 2;
            //Setting last row to be the upmost index
            int lastRow = 0;
            int downmostRow = grid[0].Count - 1;
            // First we need to sweep all tiles down, to remove empty tiles
            for (int row = firstRow; row >= lastRow; row--)
            {
                int currentRow = row;
                while (currentRow < downmostRow && EmptyLogic(currentRow, column, currentRow + 1, column))
                {
                    int numberToMove = grid[currentRow][column];
                    grid[currentRow][column] = 0;
                    grid[currentRow + 1][column] = numberToMove;
                    currentRow++;
                }
            }
            // Then we need to check and perform additions
            for (int row = firstRow; row >= lastRow; row--)
            {
                if (AdditionLogic(row, column, row + 1, column))
                {
                    int numberToMove = grid[row][column];
                    grid[row][column] = 0;
                    grid[row + 1][column] = numberToMove * 2;
                    score += numberToMove * 2;
                    row--;
                }
            }
            // And last we need to perform an empty check and sweep again
            for (int row = firstRow - 1; row >= lastRow; row--)
            {
                int currentRow = row;
                while (currentRow < downmostRow - 1 && EmptyLogic(currentRow, column, currentRow + 1, column))
                {
                    int numberToMove = grid[currentRow][column];
                    grid[currentRow][column] = 0;
                    grid[currentRow + 1][column] = numberToMove;
                    currentRow++;
                }
            }
        }
    }

    /// <summary>
    /// Checks if the tile at the current position and the tile at the next position are the same and can be added
    /// </summary>
    /// <param name="currentVerticalPosition"></param>
    /// <param name="currentHorizontalPosition"></param>
    /// <param name="nextVerticalPosition"></param>
    /// <param name="nextHorizontalPosition"></param>
    /// <returns>Returns true if the current and the next tile are the same and can be added, else false</returns>
    bool AdditionLogic(int currentVerticalPosition, int currentHorizontalPosition,
            int nextVerticalPosition, int nextHorizontalPosition)
    {
        return grid[currentVerticalPosition][currentHorizontalPosition] != 0 &&
            grid[nextVerticalPosition][nextHorizontalPosition] ==
            grid[currentVerticalPosition][currentHorizontalPosition];
    }

    /// <summary>
    /// Checks if the tile at the next location is empty
    /// </summary>
    /// <param name="currentVerticalPosition"></param>
    /// <param name="currentHorizontalPosition"></param>
    /// <param name="nextVerticalPosition"></param>
    /// <param name="nextHorizontalPosition"></param>
    /// <returns>Returns true if the next tile is empty, else false.</returns>
    bool EmptyLogic(int currentVerticalPosition, int currentHorizontalPosition,
            int nextVerticalPosition, int nextHorizontalPosition)
    {
        return grid[nextVerticalPosition][nextHorizontalPosition] == 0 &&
            grid[currentVerticalPosition][currentHorizontalPosition] != 0;
    }

    public override int GetHashCode()
    {
        return grid.GetHashCode() ^ score;
    }
}
