using Myra.Graphics2D.UI;
using Myra.Graphics2D;
using System;
using Myra.Graphics2D.Brushes;
using Microsoft.Xna.Framework;

namespace MonoGameClient.App.ContentLoading;

internal static class MainMenu
{
    static bool loaded = false;

    static Button newGameButton;
    internal static Button NewGameButton => newGameButton;

    static Button loadGameButton;
    internal static Button LoadGameButton => loadGameButton;

    static Button gameDescriptionButton;
    internal static Button GameDescriptionButton => gameDescriptionButton;

    static Button optionsButton;
    internal static Button OptionsButton => optionsButton;

    static Button exitButton;
    internal static Button ExitButton => exitButton;

    internal static VerticalStackPanel Load()
    {
        if (loaded)
        {
            throw new InvalidOperationException("Main menu is already loaded.");
        }

        VerticalStackPanel mainMenu = new()
        {
            //ShowGridLines = true,
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Border = new SolidBrush(Color.Aquamarine),
            BorderThickness = new Thickness(4)
        };

        newGameButton = new()
        {
            Content = new Label()
            {
                Text = "New game",
                HorizontalAlignment= HorizontalAlignment.Center,
                Font = MyraContent.ComicMono32,
                Padding = new Thickness(10)
            },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        mainMenu.Widgets.Add(newGameButton);

        loadGameButton = new()
        {
            Content = new Label()
            {
                Text = "Load game",
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = MyraContent.ComicMono32,
                Padding = new Thickness(10)
            },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        mainMenu.Widgets.Add(loadGameButton);

        gameDescriptionButton = new()
        {
            Content = new Label()
            {
                Text = "Game description",
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = MyraContent.ComicMono32,
                Padding = new Thickness(10)
            },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        mainMenu.Widgets.Add(gameDescriptionButton);

        optionsButton = new()
        {
            Content = new Label()
            {
                Text = "Options",
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = MyraContent.ComicMono32,
                Padding = new Thickness(10)
            },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        mainMenu.Widgets.Add(optionsButton);

        exitButton = new()
        {
            Content = new Label()
            {
                Text = "Exit",
                HorizontalAlignment = HorizontalAlignment.Center,
                Font = MyraContent.ComicMono32,
                Padding = new Thickness(10)
            },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        mainMenu.Widgets.Add(exitButton);

        loaded = true;
        return mainMenu;
    }
}
