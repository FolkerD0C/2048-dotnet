using Game2048.Classes;

namespace Game2048.Interfaces;

public interface IMenu
{
    string DisplayName { get; }

    MenuResult MenuAction();
}
