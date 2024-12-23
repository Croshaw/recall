namespace Call.Core.Utilities;

public enum ErrorType
{
    None,
    Unknown,
    Redefinition,
    Undeclared,
    Expected,
    OutOfRange,
    NotValidNumber
}

public static class Error
{
    public static string GetMessage(ErrorType errorType, string value, Position position)
    {
        return errorType switch
        {
            ErrorType.Unknown => "Unknown type name '" + value + "' at position " + position.ToShortString(),
            ErrorType.Expected => "At position " + position.ToShortString() + " is expected \"" + value + "\"",
            ErrorType.Undeclared => "Use of undeclared identifier '" + value + "' at position " +
                                    position.ToShortString(),
            ErrorType.Redefinition => "Redefinition of " + value + " at position " + position.ToShortString(),
            ErrorType.OutOfRange => $"The number {value} given is out of range " + position.ToShortString(),
            ErrorType.NotValidNumber => $"The number {value} given is not a valid number",
            _ => throw new ArgumentOutOfRangeException(nameof(errorType), errorType, null)
        };
    }
}