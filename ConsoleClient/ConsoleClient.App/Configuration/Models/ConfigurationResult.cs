using ConsoleClient.App.Configuration.Enums;

namespace ConsoleClient.App.Configuration.Models;

internal record ConfigurationResult
{
    internal ConfigurationResultType Result { get; set; }
    internal string? Message { get; set; }
}
