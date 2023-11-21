using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Shared.Models;
using Game2048.Shared.Enums;
using Game2048.Shared.EventHandlers;
using System;
using System.Collections.Generic;

namespace ConsoleClient.AppUI.Play;

public class GameDisplay : IGameDisplay
{
    private struct Coord
    {
        internal int Vertical { get; set; }
        internal int Horizontal { get; set; }
    }

    #region Constants and static
    const char GridCornerElement = '+';
    const char GridVerticalElement = '|';
    const char GridHorizontalElement = '-';
    const char GridEmptyElement = ' ';
    const int GridVerticalOffset = 1;
    const int GridHorizontalOffset = 1;
    const int ScoreValueLabelWidth = 11;
    const string ScoreKeyLabel = "Current score: ";
    const string RemainingUndosKeyLabel = "Remaining undos: ";
    const string RemainingLivesKeyLabel = "Remaining lives: ";
    const string HighestNumberKeyLabel = "Highest number: ";
    const string HelpMove1Label = "Use the arrow keys";
    const string HelpMove2Label = "or 'WASD' to move";
    const string HelpUndoLabel = "Use BACKSPACE or 'U' to undo";
    const string HelpPauseLabel = "Use ESCAPE or 'P' to pause";
    static readonly Dictionary<int, (ConsoleColor foregroundColor, ConsoleColor backgroundColor)> tileColorMap =
        new Dictionary<int, (ConsoleColor foregroundColor, ConsoleColor backgroundColor)>()
    {
        { 0, (DisplayManager.DefaultBackgroundColor, DisplayManager.DefaultBackgroundColor) },
        { 2, (ConsoleColor.Green, ConsoleColor.Gray) },
        { 4, (ConsoleColor.Blue, ConsoleColor.Gray) },
        { 8, (ConsoleColor.Red, ConsoleColor.Gray) },
        { 16, (ConsoleColor.Green, ConsoleColor.White) },
        { 32, (ConsoleColor.Blue, ConsoleColor.White) },
        { 64, (ConsoleColor.Red, ConsoleColor.White) },
        { 128, (ConsoleColor.Green, ConsoleColor.DarkGray) },
        { 256, (ConsoleColor.Blue, ConsoleColor.DarkGray) },
        { 512, (ConsoleColor.Red, ConsoleColor.DarkGray) },
        { 1024, (ConsoleColor.Black, ConsoleColor.Green) },
        { 2048, (ConsoleColor.Black, ConsoleColor.Blue) },
        { 4096, (ConsoleColor.Black, ConsoleColor.Red) },
        { 8192, (ConsoleColor.White, ConsoleColor.Green) },
        { 16384, (ConsoleColor.White, ConsoleColor.Blue) },
        { 32768, (ConsoleColor.White, ConsoleColor.Red) },
        { 65536, (ConsoleColor.White, ConsoleColor.Red) },
        { 131072, (ConsoleColor.White, ConsoleColor.Red) }
    };
    static readonly ConsoleColor defaultTileForeground = DisplayManager.DefaultBackgroundColor;
    static readonly ConsoleColor defaultTileBackground = DisplayManager.DefaultForegroundColor;
    #endregion

    #region Size variying variables 
    int highestNumberWidth;
    int gridHeight;
    int gridWidth;
    Coord scoreValueLabelPosition;
    Coord remainingUndosValueLabelPosition;
    Coord remainingLivesValueLabelPosition;
    Coord highestNumberValueLabelPosition;
    Coord[,] tilePositions;
    #endregion

    public IDisplayRow this[int index]
    {
        get { return displayRows[index]; }
        set { displayRows[index] = value; }
    }

    readonly IList<IDisplayRow> displayRows;
    public IList<IDisplayRow> DisplayRows => displayRows;

    public int RowCount => displayRows.Count;

    public GameDisplay()
    {
        displayRows = new List<IDisplayRow>();
        for (int i = 0; i < DisplayManager.Height; i++)
        {
            displayRows.Add(new DisplayRow().PadRight(
                DisplayManager.Width,
                DisplayManager.DefaultForegroundColor,
                DisplayManager.DefaultBackgroundColor,
                DisplayManager.DefaultDisplayPositionValue
            ));
        }
        tilePositions = new Coord[0, 0];
    }

    public void Dispose()
    {
        displayRows.Dispose();
        GC.SuppressFinalize(this);
    }

    public bool IsPositionSet(int relativeVerticalPosition, int relativeHorizontalPosition)
    {
        return displayRows.Count >= relativeVerticalPosition
            && displayRows[relativeVerticalPosition].ColumnCount >= relativeHorizontalPosition
            && displayRows[relativeVerticalPosition][relativeHorizontalPosition].IsSet;
    }

#region Initializer/grid drawer
    void InitializeInfos()
    {
        var helpInfos = new string[]
        {
            HelpMove1Label, HelpMove2Label, HelpUndoLabel, HelpPauseLabel
        };
        for (int i = 0; i < helpInfos.Length; i++)
        {
            DisplayManager.PrintText(
                helpInfos[i],
                DisplayManager.Height - helpInfos.Length -1 + i,
                DisplayManager.Width - helpInfos[i].Length - 1,
                DisplayManager.DefaultForegroundColor,
                DisplayManager.DefaultBackgroundColor
            );
        }

        Coord scoreKeyLabelPosition = new Coord()
        {
            Vertical = DisplayManager.Height - 8,
            Horizontal = 1
        };
        DisplayManager.PrintText(
            ScoreKeyLabel,
            scoreKeyLabelPosition.Vertical,
            scoreKeyLabelPosition.Horizontal,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
        scoreValueLabelPosition = new Coord()
        {
            Vertical = scoreKeyLabelPosition.Vertical,
            Horizontal = scoreKeyLabelPosition.Horizontal + ScoreKeyLabel.Length
        };

        Coord remainingUndosKeyLabelPosition = new Coord()
        {
            Vertical = DisplayManager.Height - 6,
            Horizontal = 1
        };
        DisplayManager.PrintText(
            RemainingUndosKeyLabel,
            remainingUndosKeyLabelPosition.Vertical,
            remainingUndosKeyLabelPosition.Horizontal,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
        remainingUndosValueLabelPosition = new Coord()
        {
            Vertical = remainingUndosKeyLabelPosition.Vertical,
            Horizontal = remainingUndosKeyLabelPosition.Horizontal + RemainingUndosKeyLabel.Length
        };

        Coord remainingLivesKeyLabelPosition = new Coord()
        {
            Vertical = DisplayManager.Height - 4,
            Horizontal = 1
        };
        DisplayManager.PrintText(
            RemainingLivesKeyLabel,
            remainingLivesKeyLabelPosition.Vertical,
            remainingLivesKeyLabelPosition.Horizontal,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
        remainingLivesValueLabelPosition = new Coord()
        {
            Vertical = remainingLivesKeyLabelPosition.Vertical,
            Horizontal = remainingLivesKeyLabelPosition.Horizontal + RemainingLivesKeyLabel.Length
        };

        Coord highestNumberKeyLabelPosition = new Coord()
        {
            Vertical = DisplayManager.Height - 2,
            Horizontal = 1
        };
        DisplayManager.PrintText(
            HighestNumberKeyLabel,
            highestNumberKeyLabelPosition.Vertical,
            highestNumberKeyLabelPosition.Horizontal,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
        highestNumberValueLabelPosition = new Coord()
        {
            Vertical = highestNumberKeyLabelPosition.Vertical,
            Horizontal = highestNumberKeyLabelPosition.Horizontal + HighestNumberKeyLabel.Length
        };
    }

    void ConstructGridFrame()
    {
        string rowfull = "";
        tilePositions = new Coord[gridHeight, gridWidth];
        for (int i = 0; i < gridHeight; i++)
        {
            rowfull = "" +  GridCornerElement;
            string rowEmpty = "" + GridVerticalElement;
            for (int j = 0; j < gridWidth; j++)
            {
                rowfull += new string(GridHorizontalElement, highestNumberWidth) + GridCornerElement;
                rowEmpty += new string(GridEmptyElement, highestNumberWidth) + GridVerticalElement;
                tilePositions[i, j] = new Coord()
                {
                    Vertical = GridVerticalOffset + 1 + i * 2,
                    Horizontal = GridHorizontalOffset + 1  + j * (highestNumberWidth + 1)
                };
            }
            DisplayManager.PrintText(
                rowfull,
                GridVerticalOffset + i * 2,
                GridHorizontalOffset,
                DisplayManager.DefaultForegroundColor,
                DisplayManager.DefaultBackgroundColor
            );
            DisplayManager.PrintText(
                rowEmpty,
                GridVerticalOffset + 1 * 2 + 1,
                GridHorizontalOffset,
                DisplayManager.DefaultForegroundColor,
                DisplayManager.DefaultBackgroundColor
            );
        }
        DisplayManager.PrintText(
            rowfull,
            GridVerticalOffset + gridHeight * 2,
            GridHorizontalOffset,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
    }
#endregion

#region Print individual values
    void PrintTile(int tileValue, Coord tilePosition)
    {
        ConsoleColor foregroundColor = defaultTileForeground;
        ConsoleColor backgroundColor = defaultTileBackground;
        if (tileColorMap.ContainsKey(tileValue))
        {
            foregroundColor = tileColorMap[tileValue].foregroundColor;
            backgroundColor = tileColorMap[tileValue].backgroundColor;
        }
        DisplayManager.PrintText(
            $"{tileValue}".PadLeft(highestNumberWidth),
            tilePosition.Vertical,
            tilePosition.Horizontal,
            foregroundColor,
            backgroundColor
        );
    }
    
    void PrintScore(int score)
    {
        DisplayManager.PrintText(
            $"{score}".PadLeft(ScoreValueLabelWidth),
            scoreValueLabelPosition.Vertical,
            scoreValueLabelPosition.Horizontal,
            defaultTileForeground,
            defaultTileBackground
        );
    }

    void PrintRemainingUndos(int remainingUndos)
    {
        DisplayManager.PrintText(
            $"{remainingUndos}",
            remainingUndosValueLabelPosition.Vertical,
            remainingUndosValueLabelPosition.Horizontal,
            defaultTileForeground,
            defaultTileBackground
        );
    }

    void PrintRemainingLives(int remainingLives)
    {
        DisplayManager.PrintText(
            $"{remainingLives}",
            remainingLivesValueLabelPosition.Vertical,
            remainingLivesValueLabelPosition.Horizontal,
            defaultTileForeground,
            defaultTileBackground
        );
    }
#endregion

    public void MiscEventHappenedDispatcher(object? sender, MiscEventHappenedEventArgs args)
    {
        switch (args.Event)
        {
            case MiscEvent.GameOver:
            {
                DisplayManager.RollBackOverLay();
                // TODO do something, like print error message
                break;
            }
            case MiscEvent.GoalReached:
            {
                // TODO print congrats
                break;
            }
            case MiscEvent.MaxNumberChanged:
            {
                if ($"{args.NumberArg}".Length > highestNumberWidth)
                {
                    highestNumberWidth = $"{args.NumberArg}".Length;
                    ConstructGridFrame();
                }
                PrintTile(args.NumberArg, highestNumberValueLabelPosition);
                break;
            }
            case MiscEvent.UndoCountChanged:
            {
                PrintRemainingUndos(args.NumberArg);
                break;
            }
            case MiscEvent.MaxLivesChanged:
            {
                PrintRemainingLives(args.NumberArg);
                break;
            }
            default:
                break;
        }
    }

    public void OnErrorHappened(object? sender, ErrorHappenedEventArgs args)
    {
        new PlayErrorMessageOverlay(args.ErrorMessage).PrintErrorMessage();
    }

    #region Handle move event
    public void OnMoveHappened(object? sender, MoveHappenedEventArgs args)
    {
        switch (args.Direction)
        {
            case MoveDirection.Up:
            {
                MoveUp(args.Position.Grid);
                break;
            }
            case MoveDirection.Down:
            {
                MoveDown(args.Position.Grid);
                break;
            }
            case MoveDirection.Left:
            {
                MoveLeft(args.Position.Grid);
                break;
            }
            case MoveDirection.Right:
            {
                MoveRight(args.Position.Grid);
                break;
            }
            default:
                break;
        }
        PrintScore(args.Position.Score);
    }

    void MoveUp(IList<IList<int>> actualGrid)
    {
        for (int j = 0; j < gridWidth; j++)
        {
            for (int i = 0; i < gridHeight; i++)
            {
                PrintTile(actualGrid[i][j], tilePositions[i, j]);
            }
        }
    }

    void MoveDown(IList<IList<int>> actualGrid)
    {
        for (int j = gridWidth - 1; j >= 0; j--)
        {
            for (int i = gridHeight - 1; i >= 0; i--)
            {
                PrintTile(actualGrid[i][j], tilePositions[i, j]);
            }
        }
    }

    void MoveLeft(IList<IList<int>> actualGrid)
    {
        for (int i = gridHeight - 1; i >= 0; i--)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                PrintTile(actualGrid[i][j], tilePositions[i, j]);
            }
        }
    }

    void MoveRight(IList<IList<int>> actualGrid)
    {
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = gridWidth - 1; j >= 0; j--)
            {
                PrintTile(actualGrid[i][j], tilePositions[i, j]);
            }
        }
    }
    #endregion

    public void OnPlayStarted(object? sender, PlayStartedEventArgs args)
    {
        gridHeight = args.GridHeight;
        gridWidth = args.GridWidth;
        highestNumberWidth = $"{args.HighestNumber}".Length;
        
        DisplayManager.NewOverlay(this);

        InitializeInfos();
        ConstructGridFrame();

        for (int i = 0; i < args.Position.Grid.Count; i++)
        {
            for (int j = 0; j < args.Position.Grid[i].Count; j++)
            {
                PrintTile(args.Position.Grid[i][j], tilePositions[i, j]);
            }
        }
        PrintScore(args.Position.Score);
        PrintRemainingUndos(args.RemainingUndos);
        PrintRemainingLives(args.RemainingLives);
        PrintTile(args.HighestNumber, highestNumberValueLabelPosition);
    }

    public void OnUndoHappened(object? sender, UndoHappenedEventArgs args)
    {
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                PrintTile(args.Position.Grid[i][j], tilePositions[i, j]);
            }
        }
        PrintScore(args.Position.Score);
    }
}
