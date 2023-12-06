using Game2048.Config;
using Game2048.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Game2048.Shared.Models;

namespace Game2048.Repository.Helpers;

internal static class GameStateExtensions
{
    /// <summary>
    /// Sets the tile on the playing grid specified by <paramref name="vertical"/> and <paramref name="horizontal"/> to <paramref name="tileValue"/>.
    /// </summary>
    /// <param name="vertical">The row number on the playing grid to place a new tile to.</param>
    /// <param name="horizontal">The column number on the playing grid to place a new tile to.</param>
    /// <param name="tileValue">The value to place on the playing grid.</param>
    internal static void PlaceTile(this GameState state, int vertical, int horizontal, int tileValue)
    {
        state.Grid[vertical][horizontal] = tileValue;
    }

    /// <summary>
    /// Gets the empty tiles of a playing grid.
    /// </summary>
    /// <returns></returns>
    internal static IList<(int Vertical, int Horizontal)> GetEmptyTiles(this GameState state)
    {
        var result = new List<(int Vertical, int Horizontal)>();
        for (int i = 0; i < state.Grid.Count; i++)
        {
            for (int j = 0; j < state.Grid[i].Count; j++)
            {
                if (state.Grid[i][j] == 0)
                {
                    result.Add((i, j));
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Performs and returns a copy of an <see cref="GameState"/> object.
    /// </summary>
    /// <returns>A new <see cref="GameState"/> object.</returns>
    internal static GameState Copy(this GameState state)
    {
        IList<IList<int>> resultGrid = new List<IList<int>>();
        for (int i = 0; i < state.Grid.Count; i++)
        {
            resultGrid.Add(new List<int>());
            for (int j = 0; j < state.Grid[i].Count; j++)
            {
                resultGrid[i].Add(state.Grid[i][j]);
            }
        }
        int resultScore = state.Score;
        GameState copyState = new()
        {
            Grid = resultGrid,
            Score = resultScore
        };
        return copyState;
    }

    /// <summary>
    /// Performs a move on a playing grid.
    /// </summary>
    /// <param name="direction">The direction to move towards.</param>
    internal static void Move(this GameState state, MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Right:
                {
                    state.MoveRigth();
                    break;
                }
            case MoveDirection.Down:
                {
                    state.MoveDown();
                    break;
                }
            case MoveDirection.Left:
                {
                    state.MoveLeft();
                    break;
                }
            case MoveDirection.Up:
                {
                    state.MoveUp();
                    break;
                }
            default:
                {
                    throw new ArgumentException("Invalid direction.");
                }
        }
    }

    internal static bool StateEquals(this GameState state, GameState other)
    {
        for (int i = 0; i < state.Grid.Count; i++)
        {
            for (int j = 0; j < state.Grid[i].Count; j++)
            {
                if (state.Grid[i][j] != other.Grid[i][j])
                {
                    return false;
                }
            }
        }
        if (state.Score != other.Score)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Performs movement on the grid to the right.
    /// </summary>
    static void MoveRigth(this GameState state)
    {
        // We need to iterate over all rows
        for (int row = 0; row < state.Grid.Count; row++)
        {
            //Setting first column to be the rightmost index - 1
            int firstColumn = state.Grid[row].Count - 2;
            //Setting last column to be the leftmost index
            int lastColumn = 0;
            int rightmostColumn = state.Grid[row].Count - 1;
            // First we need to sweep all tiles right, to remove empty tiles
            for (int column = firstColumn; column >= lastColumn; column--)
            {
                int currentColumn = column;
                while (currentColumn < rightmostColumn && state.EmptyLogic(row, currentColumn, row, currentColumn + 1))
                {
                    int numberToMove = state.Grid[row][currentColumn];
                    state.Grid[row][currentColumn] = 0;
                    state.Grid[row][currentColumn + 1] = numberToMove;
                    currentColumn++;
                }
            }
            // Then we need to check and perform additions
            for (int column = firstColumn; column >= lastColumn; column--)
            {
                if (state.AdditionLogic(row, column, row, column + 1))
                {
                    int numberToMove = state.Grid[row][column];
                    state.Grid[row][column] = 0;
                    state.Grid[row][column + 1] = numberToMove * 2;
                    state.Score += numberToMove * 2;
                    column--;
                }
            }
            // And last we need to perform an empty check again
            for (int column = firstColumn - 1; column >= lastColumn; column--)
            {
                int currentColumn = column;
                while (currentColumn < rightmostColumn - 1 && state.EmptyLogic(row, currentColumn, row, currentColumn + 1))
                {
                    int numberToMove = state.Grid[row][currentColumn];
                    state.Grid[row][currentColumn] = 0;
                    state.Grid[row][currentColumn + 1] = numberToMove;
                    currentColumn++;
                }
            }
        }
    }

    /// <summary>
    /// Performs movement on the grid to the left.
    /// </summary>
    static void MoveLeft(this GameState state)
    {
        // We need to iterate over all rows
        for (int row = state.Grid.Count - 1; row >= 0; row--)
        {
            //Setting first column to be the leftmost index + 1
            int firstColumn = 1;
            //Setting last column to be the rightmost index
            int lastColumn = state.Grid[row].Count - 1;
            int leftmostColumn = 0;
            // First we need to sweep all tiles left, to remove empty tiles
            for (int column = firstColumn; column <= lastColumn; column++)
            {
                int currentColumn = column;
                while (currentColumn > leftmostColumn && state.EmptyLogic(row, currentColumn, row, currentColumn - 1))
                {
                    int numberToMove = state.Grid[row][currentColumn];
                    state.Grid[row][currentColumn] = 0;
                    state.Grid[row][currentColumn - 1] = numberToMove;
                    currentColumn--;
                }
            }
            // Then we need to check and perform additions
            for (int column = firstColumn; column <= lastColumn; column++)
            {
                if (state.AdditionLogic(row, column, row, column - 1))
                {
                    int numberToMove = state.Grid[row][column];
                    state.Grid[row][column] = 0;
                    state.Grid[row][column - 1] = numberToMove * 2;
                    state.Score += numberToMove * 2;
                    column++;
                }
            }
            // And last we need to perform an empty check and sweep again
            for (int column = firstColumn + 1; column <= lastColumn; column++)
            {
                int currentColumn = column;
                while (currentColumn > leftmostColumn + 1 && state.EmptyLogic(row, currentColumn, row, currentColumn - 1))
                {
                    int numberToMove = state.Grid[row][currentColumn];
                    state.Grid[row][currentColumn] = 0;
                    state.Grid[row][currentColumn - 1] = numberToMove;
                    currentColumn--;
                }
            }
        }
    }

    /// <summary>
    /// Performs movement on the grid upwards.
    /// </summary>
    static void MoveUp(this GameState state)
    {
        // We need to iterate over all columns
        for (int column = 0; column < state.Grid[0].Count; column++)
        {
            //Setting first row to be the upmost index + 1
            int firstRow = 1;
            //Setting last row to be the downmost index
            int lastRow = state.Grid.Count - 1;
            int upmostRow = 0;
            // First we need to sweep all tiles up, to remove empty tiles
            for (int row = firstRow; row <= lastRow; row++)
            {
                int currentRow = row;
                while (currentRow > upmostRow && state.EmptyLogic(currentRow, column, currentRow - 1, column))
                {
                    int numberToMove = state.Grid[currentRow][column];
                    state.Grid[currentRow][column] = 0;
                    state.Grid[currentRow - 1][column] = numberToMove;
                    currentRow--;
                }
            }
            // Then we need to check and perform additions
            for (int row = firstRow; row <= lastRow; row++)
            {
                if (state.AdditionLogic(row, column, row - 1, column))
                {
                    int numberToMove = state.Grid[row][column];
                    state.Grid[row][column] = 0;
                    state.Grid[row - 1][column] = numberToMove * 2;
                    state.Score += numberToMove * 2;
                    row++;
                }
            }
            // And last we need to perform an empty check and sweep again
            for (int row = firstRow + 1; row <= lastRow; row++)
            {
                int currentRow = row;
                while (currentRow > upmostRow + 1 && state.EmptyLogic(currentRow, column, currentRow - 1, column))
                {
                    int numberToMove = state.Grid[currentRow][column];
                    state.Grid[currentRow][column] = 0;
                    state.Grid[currentRow - 1][column] = numberToMove;
                    currentRow--;
                }
            }
        }
    }

    /// <summary>
    /// Performs movement on the grid downwards.
    /// </summary>
    static void MoveDown(this GameState state)
    {
        // We need to iterate over all columns
        for (int column = state.Grid[0].Count - 1; column >= 0; column--)
        {
            //Setting first row to be the downmost index - 1
            int firstRow = state.Grid.Count - 2;
            //Setting last row to be the upmost index
            int lastRow = 0;
            int downmostRow = state.Grid.Count - 1;
            // First we need to sweep all tiles down, to remove empty tiles
            for (int row = firstRow; row >= lastRow; row--)
            {
                int currentRow = row;
                while (currentRow < downmostRow && state.EmptyLogic(currentRow, column, currentRow + 1, column))
                {
                    int numberToMove = state.Grid[currentRow][column];
                    state.Grid[currentRow][column] = 0;
                    state.Grid[currentRow + 1][column] = numberToMove;
                    currentRow++;
                }
            }
            // Then we need to check and perform additions
            for (int row = firstRow; row >= lastRow; row--)
            {
                if (state.AdditionLogic(row, column, row + 1, column))
                {
                    int numberToMove = state.Grid[row][column];
                    state.Grid[row][column] = 0;
                    state.Grid[row + 1][column] = numberToMove * 2;
                    state.Score += numberToMove * 2;
                    row--;
                }
            }
            // And last we need to perform an empty check and sweep again
            for (int row = firstRow - 1; row >= lastRow; row--)
            {
                int currentRow = row;
                while (currentRow < downmostRow - 1 && state.EmptyLogic(currentRow, column, currentRow + 1, column))
                {
                    int numberToMove = state.Grid[currentRow][column];
                    state.Grid[currentRow][column] = 0;
                    state.Grid[currentRow + 1][column] = numberToMove;
                    currentRow++;
                }
            }
        }
    }

    /// <summary>
    /// Checks if the tile at the current position and the tile at the next position are the same and can be added.
    /// </summary>
    /// <param name="currentVerticalPosition">The vertical position of the current tile.</param>
    /// <param name="currentHorizontalPosition">The horizontal position of the current tile.</param>
    /// <param name="nextVerticalPosition">The vertical position of the next tile.</param>
    /// <param name="nextHorizontalPosition">The horizontal position of the next tile.</param>
    /// <returns>True if the current and the next tile are the same and can be added.</returns>
    static bool AdditionLogic(this GameState state, int currentVerticalPosition, int currentHorizontalPosition,
            int nextVerticalPosition, int nextHorizontalPosition)
    {
        return state.Grid[currentVerticalPosition][currentHorizontalPosition] != 0 &&
            state.Grid[nextVerticalPosition][nextHorizontalPosition] ==
            state.Grid[currentVerticalPosition][currentHorizontalPosition];
    }

    /// <summary>
    /// Checks if the tile at the next location is empty.
    /// </summary>
    /// <param name="currentVerticalPosition">The vertical position of the current tile.</param>
    /// <param name="currentHorizontalPosition">The horizontal position of the current tile.</param>
    /// <param name="nextVerticalPosition">The vertical position of the next tile.</param>
    /// <param name="nextHorizontalPosition">The horizontal position of the next tile.</param>
    /// <returns>True if the next tile is empty.</returns>
    static bool EmptyLogic(this GameState state, int currentVerticalPosition, int currentHorizontalPosition,
            int nextVerticalPosition, int nextHorizontalPosition)
    {
        return state.Grid[nextVerticalPosition][nextHorizontalPosition] == 0 &&
            state.Grid[currentVerticalPosition][currentHorizontalPosition] != 0;
    }
}
