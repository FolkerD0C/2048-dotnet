using Game2048.Shared.Enums;

namespace Game2048.Shared.Models;

public struct SaveResult
{
    public SaveResultType ResultType { get; set; }

    public string Message { get; set; }
}
