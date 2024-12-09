namespace Call.Core.Utilities;

public enum ErrorType
{
    Unknown,
    Redefinition,
    Undeclared,
    Expected
}

public static class Error
{
    public static string GetMesg(ErrorType errorType, string value, Position position)
    {
        return errorType switch
        {
            ErrorType.Unknown => "Unknown type name '" + value + "' at position " + position.ToShortString(),
            ErrorType.Expected => "At position " + position.ToShortString() + " is expected \"" + value + "\"",
            ErrorType.Undeclared => "Use of undeclared identifier '" + value + "' at position " +
                                    position.ToShortString(),
            ErrorType.Redefinition => "Redefinition of " + value + " at position " + position.ToShortString(),
            _ => throw new ArgumentOutOfRangeException(nameof(errorType), errorType, null)
        };
    }
}