using ConsoleClient.AppUI.Enums;
using ConsoleClient.AppUI.Misc;
using ConsoleClient.Display;
using ConsoleClient.Display.Helpers;
using ConsoleClient.Display.Models;
using _2048ish.Base.Enums;
using Game2048.Managers.Enums;
using Game2048.Managers.EventHandlers;
using System;
using System.Collections.Generic;

namespace ConsoleClient.AppUI.Play;

/// <summary>
/// A class used for displaying a play and holds methods that can be subscribed to an <see cref="Game2048.Managers.IPlayInstance"/> object's events.
/// </summary>
public class GameDisplay : IGameDisplay
{
    /// <summary>
    /// A struct for storing positional information.
    /// </summary>
    private struct Coord
    {
        /// <summary>
        /// The vertical position.
        /// </summary>
        internal int Vertical { get; set; }
        /// <summary>
        /// Tho horizontal position.
        /// </summary>
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

    const string PlayerNameKeyLabel = "Player: ";
    const string ScoreKeyLabel = "Current score: ";
    const string RemainingUndosKeyLabel = "Remaining undos: ";
    const string RemainingLivesKeyLabel = "Remaining lives: ";
    const string HighestNumberKeyLabel = "Highest number: ";

    const string HelpMove1Label = "Use the arrow keys";
    const string HelpMove2Label = "or 'WASD' to move";
    const string HelpUndoLabel = "Use BACKSPACE or 'U' to undo";
    const string HelpPauseLabel = "Use ESCAPE or 'P' to pause";

    static readonly Dictionary<int, (ConsoleColor foregroundColor, ConsoleColor backgroundColor)> tileColorMap =
        new()
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

    #region Size varying variables 
    int highestNumberWidth;
    int gridHeight;
    int gridWidth;
    Coord playerNameValueLabelPosition;
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

    /// <summary>
    /// If true then the printing of the overlay under this is suppressed.
    /// </summary>
    bool suppressPrintingPreviousOverlay;

    /// <summary>
    /// Creates a new instance of the <see cref="GameDisplay"/> class.
    /// </summary>
    public GameDisplay()
    {
        displayRows = new List<IDisplayRow>();
        for (int i = 0; i < DisplayManager.Height; i++)
        {
            displayRows.Add(new DisplayRow().PadRight(
                DisplayManager.Width,
                DisplayManager.DefaultForegroundColor,
                DisplayManager.DefaultBackgroundColor,
                DisplayManager.DefaultDisplayPositionValue,
                true
            ));
        }
        tilePositions = new Coord[0, 0];
        suppressPrintingPreviousOverlay = false;
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
    /// <summary>
    /// Prints some helping info and labels and sets their positional value.
    /// </summary>
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
                DisplayManager.Height - helpInfos.Length - 1 + i,
                DisplayManager.Width - helpInfos[i].Length - 1,
                DisplayManager.DefaultForegroundColor,
                DisplayManager.DefaultBackgroundColor
            );
        }

        Coord playerNameKeyLabelPosition = new()
        {
            Vertical = DisplayManager.Height - 10,
            Horizontal = 1
        };
        DisplayManager.PrintText(
            PlayerNameKeyLabel,
            playerNameKeyLabelPosition.Vertical,
            playerNameKeyLabelPosition.Horizontal,
            DisplayManager.DefaultForegroundColor,
            DisplayManager.DefaultBackgroundColor
        );
        playerNameValueLabelPosition = new()
        {
            Vertical = playerNameKeyLabelPosition.Vertical,
            Horizontal = playerNameKeyLabelPosition.Horizontal + PlayerNameKeyLabel.Length
        };

        Coord scoreKeyLabelPosition = new()
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
        scoreValueLabelPosition = new()
        {
            Vertical = scoreKeyLabelPosition.Vertical,
            Horizontal = scoreKeyLabelPosition.Horizontal + ScoreKeyLabel.Length
        };

        Coord remainingUndosKeyLabelPosition = new()
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
        remainingUndosValueLabelPosition = new()
        {
            Vertical = remainingUndosKeyLabelPosition.Vertical,
            Horizontal = remainingUndosKeyLabelPosition.Horizontal + RemainingUndosKeyLabel.Length
        };

        Coord remainingLivesKeyLabelPosition = new()
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
        remainingLivesValueLabelPosition = new()
        {
            Vertical = remainingLivesKeyLabelPosition.Vertical,
            Horizontal = remainingLivesKeyLabelPosition.Horizontal + RemainingLivesKeyLabel.Length
        };

        Coord highestNumberKeyLabelPosition = new()
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
        highestNumberValueLabelPosition = new()
        {
            Vertical = highestNumberKeyLabelPosition.Vertical,
            Horizontal = highestNumberKeyLabelPosition.Horizontal + HighestNumberKeyLabel.Length
        };
    }

    /// <summary>
    /// Constructs the grid on the display and sets the positional value of the tiles.
    /// </summary>
    void ConstructGridFrame()
    {
        string rowfull = "";
        tilePositions = new Coord[gridHeight, gridWidth];
        for (int i = 0; i < gridHeight; i++)
        {
            rowfull = "" + GridCornerElement;
            string rowEmpty = "" + GridVerticalElement;
            for (int j = 0; j < gridWidth; j++)
            {
                rowfull += new string(GridHorizontalElement, highestNumberWidth) + GridCornerElement;
                rowEmpty += new string(GridEmptyElement, highestNumberWidth) + GridVerticalElement;
                tilePositions[i, j] = new Coord()
                {
                    Vertical = GridVerticalOffset + 1 + i * 2,
                    Horizontal = GridHorizontalOffset + 1 + j * (highestNumberWidth + 1)
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
                GridVerticalOffset + i * 2 + 1,
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
    /// <summary>
    /// Prints a number tile on the display.
    /// </summary>
    /// <param name="tileValue">The value of the number tile.</param>
    /// <param name="tilePosition">The position of the number tile.</param>
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

    /// <summary>
    /// Prints the name of the player on the display.
    /// </summary>
    /// <param name="oldName">The old name of the player.</param>
    /// <param name="newName">The new name of the player.</param>
    void PrintPlayerName(string oldName, string newName)
    {
        DisplayManager.PrintText(
            new string(' ', oldName.Length),
            playerNameValueLabelPosition.Vertical,
            playerNameValueLabelPosition.Horizontal,
            DisplayManager.DefaultBackgroundColor,
            DisplayManager.DefaultBackgroundColor
        );
        DisplayManager.PrintText(
            newName,
            playerNameValueLabelPosition.Vertical,
            playerNameValueLabelPosition.Horizontal,
            defaultTileForeground,
            defaultTileBackground
        );
    }

    /// <summary>
    /// Prints the socre of the player on the display.
    /// </summary>
    /// <param name="score">The score of the player.</param>
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

    /// <summary>
    /// Prints the number of remaining undos of the player on the display.
    /// </summary>
    /// <param name="remainingUndos">The number of remaining undos of the player.</param>
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
    /// <summary>
    /// Prints the number of remaining lives of the player on the display.
    /// </summary>
    /// <param name="remainingLives">The number of remaining lives of the player.</param>
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

    #region EventHandling
    public void MiscEventHappenedDispatcher(object? sender, MiscEventHappenedEventArgs args)
    {
        switch (args.Event)
        {
            case MiscEvent.GoalReached:
                {
                    new MessageOverlay("Congratulations! You have won the game! You can play more or exit.", MessageType.Success).PrintMessage();
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
        new MessageOverlay(args.ErrorMessage, MessageType.Error).PrintMessage();
    }

    #region Handle move event
    public void OnMoveHappened(object? sender, MoveHappenedEventArgs args)
    {
        switch (args.Direction)
        {
            case MoveDirection.Up:
                {
                    MoveUp(args.State.Grid);
                    break;
                }
            case MoveDirection.Down:
                {
                    MoveDown(args.State.Grid);
                    break;
                }
            case MoveDirection.Left:
                {
                    MoveLeft(args.State.Grid);
                    break;
                }
            case MoveDirection.Right:
                {
                    MoveRight(args.State.Grid);
                    break;
                }
            default:
                break;
        }
        PrintScore(args.State.Score);
    }

    /// <summary>
    /// Handles and prints movement action upwards.
    /// </summary>
    /// <param name="actualGrid">The grid to print.</param>
    void MoveUp(List<List<int>> actualGrid)
    {
        for (int j = 0; j < gridWidth; j++)
        {
            for (int i = 0; i < gridHeight; i++)
            {
                PrintTile(actualGrid[i][j], tilePositions[i, j]);
            }
        }
    }

    /// <summary>
    /// Handles and prints movement action downwards.
    /// </summary>
    /// <param name="actualGrid">The grid to print.</param>
    void MoveDown(List<List<int>> actualGrid)
    {
        for (int j = gridWidth - 1; j >= 0; j--)
        {
            for (int i = gridHeight - 1; i >= 0; i--)
            {
                PrintTile(actualGrid[i][j], tilePositions[i, j]);
            }
        }
    }

    /// <summary>
    /// Handles and prints movement action left.
    /// </summary>
    /// <param name="actualGrid">The grid to print.</param>
    void MoveLeft(List<List<int>> actualGrid)
    {
        for (int i = gridHeight - 1; i >= 0; i--)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                PrintTile(actualGrid[i][j], tilePositions[i, j]);
            }
        }
    }

    /// <summary>
    /// Handles and prints movement action right.
    /// </summary>
    /// <param name="actualGrid">The grid to print.</param>
    void MoveRight(List<List<int>> actualGrid)
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

        for (int i = 0; i < args.State.Grid.Count; i++)
        {
            for (int j = 0; j < args.State.Grid[i].Count; j++)
            {
                PrintTile(args.State.Grid[i][j], tilePositions[i, j]);
            }
        }

        PrintPlayerName("", args.PlayerName);
        PrintScore(args.State.Score);
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

    public void OnPlayerNameChanged(object? sender, PlayerNameChangedEventArgs args)
    {
        PrintPlayerName(args.OldName, args.NewName);
    }

    public void OnPlayEnded(object? sender, EventArgs args)
    {
        DisplayManager.RollBackOverLay(suppressPrintingPreviousOverlay);
    }

    public void SetPreviousOverlaySuppression(bool previousOverlaySuppression)
    {
        suppressPrintingPreviousOverlay = previousOverlaySuppression;
    }
    #endregion
}
