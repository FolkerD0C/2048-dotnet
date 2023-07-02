using Game2048.Interfaces;
using Game2048.Classes.Menus;
using Game2048.Static;

namespace Game2048.Classes;

class Play
{
    IGameDisplay display;

    IGameRepository repository;

    IFileHandler fileHandler;

    string savePath;

    bool inGame;

    public Play(object[] args)
    {
        var colorSet = InitColors();
        display = new GameDisplay(colorSet);
        fileHandler = (IFileHandler)args[0];
        if (args.Length > 1)
        {
            savePath = (string)args[1];
            var jsonRepository  = fileHandler.GetSavedObject(savePath);
            repository = fileHandler.Converter.DeserializeRepository(jsonRepository);
        }
        else
        {
            repository = new GameRepository();
        }
        repository.GridUpdated += display.PrintTile;
        repository.ScoreUpdated += display.PrintScore;
        repository.UndoHappened += display.RedrawGridInstance;
        repository.Reach2048 += GameWon;
        repository.Reach2048 += display.ScaleUp;
        repository.UndoCountChanged += display.PrintUndosCount;
        repository.LivesCountChanged += display.PrintLivesCount;
        repository.Initialize(args.Length > 1);
    }

    public static void Initialize(params object[] args)
    {
        var play = new Play(args);
        play.inGame = true;
        play.Run();
    }

    Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)> InitColors()
    {
        Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)> colorSet =
            new Dictionary<int, (ConsoleColor Fg, ConsoleColor Bg)>();
        ConsoleColor[] fgColors = new ConsoleColor[]
        {
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Black, ConsoleColor.Black, ConsoleColor.Black,
            ConsoleColor.White, ConsoleColor.White, ConsoleColor.White,
            ConsoleColor.White, ConsoleColor.White
        };
        ConsoleColor[] bgColors = new ConsoleColor[]
        {
            ConsoleColor.Gray, ConsoleColor.Gray, ConsoleColor.Gray,
            ConsoleColor.White, ConsoleColor.White, ConsoleColor.White,
            ConsoleColor.DarkGray, ConsoleColor.DarkGray, ConsoleColor.DarkGray,
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Red,
            ConsoleColor.Red, ConsoleColor.Red
        };
        colorSet.Add(0, (ConsoleColor.White, ConsoleColor.Black));
        for (int i = 0; i < fgColors.Length; i++)
        {
            colorSet.Add((int)Math.Pow(2, i + 1), (fgColors[i], bgColors[i]));
        }
        return colorSet;
    }

    void HandleInput()
    {
        MoveDirection? input = null;
        switch(Console.ReadKey(true).Key)
        {
            case ConsoleKey.W: case ConsoleKey.UpArrow:
                {
                    input = MoveDirection.Up;
                    break;
                }
            case ConsoleKey.S: case ConsoleKey.DownArrow:
                {
                    input = MoveDirection.Down;
                    break;
                }
            case ConsoleKey.A: case ConsoleKey.LeftArrow:
                {
                    input = MoveDirection.Left;
                    break;
                }
            case ConsoleKey.D: case ConsoleKey.RightArrow:
                {
                    input = MoveDirection.Right;
                    break;
                }
            case ConsoleKey.Backspace: case ConsoleKey.U:
                {
                    try
                    {
                        repository.Undo();
                    }
                    catch (UndoImpossibleException exc)
                    {
                        display.PrintErrorMessage(exc.Message);
                    }
                    break;
                }
            case ConsoleKey.Escape:
                {
                    ConstructIngameMenu();
                    break;
                }
        }
        if (input == null)
        {
            return;
        }
        try
        {
            repository.Move(input);
        }
        catch (CannotMoveException exc)
        {
            display.PrintErrorMessage(exc.Message);
        }
        catch (GridStuckException exc)
        {
            display.PrintErrorMessage(exc.Message);
        }
    }

    void GameOver()
    {
        var highScores = fileHandler.Converter.DeserializeHighScores(fileHandler.GetSavedObject(fileHandler.HighscoresPath));
        var playerScore = repository.UndoChain.First.Value.Score;
        if (highScores.Select(t => t.Score).ToList()[^1] < playerScore)
        {
            display.PrintErrorMessage("You have reached a new high score!");
            Thread.Sleep(1000);
            string playerName = NameForm.Form();
            var highScoresToSave = highScores.ToList();
            highScoresToSave.Add((playerName, playerScore));
            highScoresToSave = highScoresToSave.OrderByDescending(t => t.Score).Take(10).ToList();
            fileHandler.SaveObject(fileHandler.HighscoresPath, fileHandler.Converter.SerializeHighScores(highScoresToSave));
        }
    }

    void GameWon(object? o, int[,] grid)
    {
        display.PrintErrorMessage("Congratulations! You have reached the goal of the game.");
        Thread.Sleep(1000);
        ConstructIngameMenu();
    }

    void ConstructIngameMenu()
    {
        List<IMenu> subMenus = new List<IMenu>();
        subMenus.Add(new NamedReturnMenu("Resume game", MenuResult.Back));
        ObjectMenu saveSubMenu = new ObjectMenu("Save game", Save, new object[0]);
        saveSubMenu.AddArgs(new object[] { saveSubMenu });
        subMenus.Add(saveSubMenu);
        subMenus.Add(new PromptMenu("Exit to main menu", new string[] { "Are you sure to exit to main menu?" }, LeaveGame));
        subMenus.Add(new PromptMenu("Quit game", new string[] { "Are you sure to quit the game?" }, Resources.GracefulExit));

        NotifiableMenu ingameMenu = new NotifiableMenu("ingameMenu", subMenus,
                new Dictionary<IMenu, Dictionary<MenuResult, string[]>>()
                {
                    {
                        saveSubMenu, new Dictionary<MenuResult, string[]>()
                        {
                            {
                                MenuResult.Obj, new string[]
                                {
                                    "Game successfully saved."
                                }
                            }
                        }
                    }
                });
        ingameMenu.AddAcceptedResult(MenuResult.No);
        ingameMenu.AddAcceptedResult(MenuResult.Obj);
        ingameMenu.MenuAction();
    }

    void LeaveGame()
    {
        inGame = false;
    }

    public bool Run()
    {
        try
        {
            while (inGame)
            {
                HandleInput();
            }
        }
        catch (GameOverException exc)
        {
            display.PrintErrorMessage(exc.Message);
            GameOver();
        }
        return true;
    }

    void Save(object[] args)
    {
        if (savePath is not null)
        {
            fileHandler.SaveObject(savePath, fileHandler.Converter.SerializeRepository(repository));
        }
        else
        {
            try
            {
                string playerName = NameForm.Form();
                if (fileHandler.SaveExists(playerName))
                {
                    new PromptObjectMenu("overwriteSave", new string[] { "This savefile already exists,", "would you like to overwrite it?" }, OverwriteSave, playerName).MenuAction();
                }
                else
                {
                    savePath = fileHandler.GetFullSavePath(playerName);
                    fileHandler.SaveObject(savePath, fileHandler.Converter.SerializeRepository(repository));
                }
            }
            catch (FormCancelledException)
            {
                (args.First(arg => arg is ObjectMenu) as ObjectMenu).SetResult(MenuResult.No);
            }
        }
    }

    void OverwriteSave(object[] args)
    {
        savePath = fileHandler.GetFullSavePath((string)args[0]);
        fileHandler.SaveObject(savePath, fileHandler.Converter.SerializeRepository(repository));
    }
}
