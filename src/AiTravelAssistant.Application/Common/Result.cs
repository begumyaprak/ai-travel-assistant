namespace AiTravelAssistant.Application.Common;

/// <summary>
/// Represents the outcome of an application operation, carrying either a success value or an error.
/// </summary>
/// <typeparam name="T">The type of the payload returned on success.</typeparam>
public class Result<T>
{
    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the operation payload when <see cref="IsSuccess"/> is <see langword="true"/>; otherwise <see langword="null"/>.</summary>
    public T? Data { get; }

    /// <summary>Gets the machine-readable error code when <see cref="IsSuccess"/> is <see langword="false"/>; otherwise <see langword="null"/>.</summary>
    public string? ErrorCode { get; }

    /// <summary>Gets the human-readable error description when <see cref="IsSuccess"/> is <see langword="false"/>; otherwise <see langword="null"/>.</summary>
    public string? ErrorMessage { get; }

    private Result(T data)
    {
        IsSuccess = true;
        Data = data;
    }

    private Result(string errorCode, string errorMessage)
    {
        IsSuccess = false;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result carrying the specified data.
    /// </summary>
    /// <param name="data">The operation payload.</param>
    /// <returns>A successful <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Success(T data) => new(data);

    /// <summary>
    /// Creates a failed result with the specified error details.
    /// </summary>
    /// <param name="errorCode">The machine-readable error code.</param>
    /// <param name="errorMessage">A human-readable description of the failure.</param>
    /// <returns>A failed <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Failure(string errorCode, string errorMessage) => new(errorCode, errorMessage);
}
