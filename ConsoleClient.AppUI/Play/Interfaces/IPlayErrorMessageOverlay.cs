using System.Collections.Generic;

namespace Game2048.ConsoleFrontend.Display;

public interface IPlayErrorMessageOverlay : IOverLay
{
    IEnumerable<string> ErrorMessage { get; set; }
}