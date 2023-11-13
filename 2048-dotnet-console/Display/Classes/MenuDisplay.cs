using Game2048.ConsoleFrontend.Helpers;
using Game2048.ConsoleFrontend.Helpers.EventHandlers;
using Game2048.ConsoleFrontend.Models;
using System.Collections.Generic;

namespace Game2048.ConsoleFrontend.Display;

public class MenuDisplay : IMenuDisplay
{
    public IDisplayRow this[int index] { get { return drawPositions[index]; } set { drawPositions[index] = value; } }

    readonly IList<IDisplayRow> drawPositions;
    public IList<IDisplayRow> DrawPositions => drawPositions;

    public int RowCount => drawPositions.Count;

    readonly IList<IMenuItem> menuItems;
    public IList<IMenuItem> MenuItems => menuItems;

    readonly int selectedMenuItem;
    public int SelectedMenuItem => selectedMenuItem;

    readonly IList<string> displayText;
    public IList<string> DisplayText => displayText;

    public MenuDisplay(IList<IMenuItem> menuItems, int selectedMenuItem, IList<string> displayText)
    {
        this.menuItems = menuItems;
        this.selectedMenuItem = selectedMenuItem;
        this.displayText = displayText;
        drawPositions = new List<IDisplayRow>();
    }

    public void InitializeDisplayRows()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        displayText.Dispose();
        menuItems.Dispose();
        drawPositions.Dispose();
    }

    public void OnMenuNavigationEnded(object? sender, MenuNavigationEndedEventArgs args)
    {
        DisplayManager.RollBackOverLay();
        Dispose();
    }

    public void OnMenuSelectionChanged(object? sender, MenuSelectionChangedEventArgs args)
    {
        throw new System.NotImplementedException();
    }
}
