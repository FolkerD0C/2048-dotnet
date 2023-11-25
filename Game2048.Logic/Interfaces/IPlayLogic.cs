using Game2048.Logic.Enums;
using Game2048.Shared.Enums;

namespace Game2048.Logic;

public interface IPlayLogic : IPlayInstance
{
    void Start();
    void End();
    void PreInput();
    InputResult HandleInput(GameInput input);
}