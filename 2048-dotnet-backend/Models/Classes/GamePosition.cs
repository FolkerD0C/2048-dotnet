using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Game2048.Backend.Helpers.Enums;

namespace Game2048.Backend.Models;

public class GamePosition : IGamePosition
{
    IList<IList<int>> grid;
    public IList<IList<int>> Grid { get => grid; protected set { grid = value; } }

    int score;

    public int Score { get => score; protected set { score = value; } }

    public GamePosition()
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

    /// <summary>
    /// Returns true if movement is possible, else false.
    /// </summary>
    public bool CanMove => CheckIfCanMove();

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
        // Check if there are similar tiles horizontally
        for (int i = 0; i < PlayEnvironment.GridWidth; i++)
        {
            for (int j = 0; j < PlayEnvironment.GridHeight - 1; j++)
            {
                if (grid[j][i] == grid[j][i + 1])
                {
                    return true;
                }
            }
        }
        return false;
    }

    public IGamePosition Copy()
    {
        var result = new GamePosition();
        IList<IList<int>> resultGrid = new List<IList<int>>();
        for (int i = 0; i < grid.Count; i++)
        {
            resultGrid.Add(new List<int>());
            for (int j = 0; j < grid[i].Count; j++)
            {
                resultGrid[i][j] = grid[i][j];
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
        // Default movement direction is right, so we need to rotate
        switch (direction)
        {
            case MoveDirection.Right:
            {
                Move();
                break;
            }
            case MoveDirection.Down:
            {
                Rotate();
                Move();
                RotateCounter();
                break;
            }
            case MoveDirection.Left:
            {
                RotateHalf();
                Move();
                RotateHalf();
                break;
            }
            case MoveDirection.Up:
            {
                RotateCounter();
                Move();
                Rotate();
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
        if (obj is not null && obj is GamePosition other)
        {
            return grid.Equals(other.Grid) && score == other.Score;
        }
        return false;
    }

    /// <summary>
    /// Rotate the grid 90 degrees clockwise
    /// </summary>
    void Rotate()
    {
        // Moving down
        IList<IList<int>> result = new List<IList<int>>();
        for (int i = 0; i < grid[0].Count; i++)
        {
            result.Add(new List<int>());
            for (int j = grid.Count - 1; j > -1; j--)
            {
                result[i].Add(grid[j][i]);
            }
        }
        grid = result;
    }

    /// <summary>
    /// Rotate the grid 180 degrees
    /// </summary>
    void RotateHalf()
    {
        // Moving left
        IList<IList<int>> result = new List<IList<int>>();
        for (int i = grid.Count - 1; i > -1; i--)
        {
            result.Add(new List<int>());
            for (int j = grid[0].Count - 1; j > -1; j--)
            {
                result[grid.Count - i - 1].Add(grid[i][j]);
            }
        }
        grid = result;
    }

    /// <summary>
    /// Rotate the grid 90 degrees counter clockwise
    /// </summary>
    void RotateCounter()
    {
        // Moving up
        IList<IList<int>> result = new List<IList<int>>();
        for (int i = grid[0].Count - 1; i > -1; i--)
        {
            result.Add(new List<int>());
            for (int j = 0; j < grid.Count; j++)
            {
                result[grid[0].Count - i - 1].Add(grid[j][i]);
            }
        }
    }

    /// <summary>
    /// Performs the movement on the grid, to the right direction.
    /// </summary>
    void Move()
    {
        // We need to iterate over all rows
        for (int row = 0; row < Grid.Count; row++)
        {
            int firstColumn = PlayEnvironment.GridWidth - 2;
            int lastColumn = 0;
            int rightmostColumn = PlayEnvironment.GridWidth - 1;
            // First we need to sweep all tiles right, to remove empty tiles
            for (int column = firstColumn; column >= lastColumn; column--)
            {
                int currentColumn = column;
                while (currentColumn < rightmostColumn && EmptyLogic(row, currentColumn, row, currentColumn + 1))
                {
                    int numberToMove = Grid[row][currentColumn];
                    Grid[row][currentColumn] = 0;
                    Grid[row][currentColumn + 1] = numberToMove;
                    currentColumn++;
                }
            }
            // Then we need to check and perform additions
            for (int column = firstColumn; column >= lastColumn; column--)
            {
                if (AdditionLogic(row, column, row, column + 1))
                {
                    int numberToMove = Grid[row][column];
                    Grid[row][column] = 0;
                    Grid[row][column + 1] = numberToMove * 2;
                    column--;
                }
            }
            // And last we need to perform an empty check again
            for (int column = firstColumn - 1; column >= lastColumn; column--)
            {
                int currentColumn = column;
                while (currentColumn < rightmostColumn - 1 && EmptyLogic(row, currentColumn, row, currentColumn + 1))
                {
                    int numberToMove = Grid[row][currentColumn];
                    Grid[row][currentColumn] = 0;
                    Grid[row][currentColumn + 1] = numberToMove;
                    currentColumn++;
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
        return Grid[currentVerticalPosition][currentHorizontalPosition] != 0 &&
            Grid[nextVerticalPosition][nextHorizontalPosition] ==
            Grid[currentVerticalPosition][currentHorizontalPosition];
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
        return Grid[nextVerticalPosition][nextHorizontalPosition] == 0 &&
            Grid[currentVerticalPosition][currentHorizontalPosition] != 0;
    }

    public override int GetHashCode()
    {
        return grid.GetHashCode() ^ score;
    }
}
