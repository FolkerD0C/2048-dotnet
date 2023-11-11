using Game2048.Classes;

namespace Game2048.Interfaces;

interface IGameDisplay
{
    void ScaleUp(object? o, int[,] grid);

    void RedrawGridInstance(object? o, (int[,] grid, int score) args);

    void PrintTile(object? o, (int vertical, int horizontal, int value) args);

    void PrintScore(object? o, int score);

    void PrintErrorMessage(string errorMessage);

    void PrintUndosCount(object? o, int undosCount);
    
    void PrintLivesCount(object? o, int livesCount);
}
