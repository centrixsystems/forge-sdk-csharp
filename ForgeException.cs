namespace Forge.Sdk;

/// <summary>Base exception for the Forge SDK.</summary>
public class ForgeException : Exception
{
    public ForgeException(string message) : base(message) { }
    public ForgeException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>The server returned a 4xx/5xx response.</summary>
public class ForgeServerException : ForgeException
{
    public int StatusCode { get; }

    public ForgeServerException(int statusCode, string message)
        : base($"server error ({statusCode}): {message}")
    {
        StatusCode = statusCode;
    }
}

/// <summary>Failed to connect to the Forge server.</summary>
public class ForgeConnectionException : ForgeException
{
    public ForgeConnectionException(Exception cause)
        : base($"connection error: {cause.Message}", cause)
    {
    }
}
