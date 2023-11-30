using ConsoleClient.AppUI.Enums;

namespace ConsoleClient.AppUI.Misc;

/// <summary>
/// A struct that represents the input for an <see cref="INameForm"/> object action.
/// </summary>
public struct NameFormInput
{
    /// <summary>
    /// The type of the input.
    /// </summary>
    public NameFormInputType InputType { get; set; }

    /// <summary>
    /// The input value in case of <see cref="NameFormInputType.Character"/>.
    /// </summary>
    public char InputValue { get; set; }
}
