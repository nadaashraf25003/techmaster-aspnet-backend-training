namespace TrainingCenter.Api.Common.Responses;

/// <summary>
/// Encapsulates the outcome of a domain or business workflow without throwing unnecessary exceptions.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Message { get; }
    public List<string> Errors { get; }
    public int StatusCode { get; }

    protected Result(bool isSuccess, string message, List<string>? errors = null, int statusCode = 200)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors ?? new List<string>();
        StatusCode = statusCode;
    }

    public static Result Success(string message = "Operation completed successfully.", int statusCode = 200)
    {
        return new Result(true, message, null, statusCode);
    }

    public static Result Failure(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new Result(false, message, errors, statusCode);
    }

    public static Result NotFound(string message = "Resource not found.")
    {
        return new Result(false, message, null, 404);
    }

    public static Result Forbidden(string message = "Access denied.")
    {
        return new Result(false, message, null, 403);
    }

    public static Result Unauthorized(string message = "Unauthorized access.")
    {
        return new Result(false, message, null, 401);
    }
}

/// <summary>
/// Encapsulates the outcome of a business operation with a strongly-typed value payload.
/// </summary>
/// <typeparam name="T">The payload type.</typeparam>
public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(bool isSuccess, T? value, string message, List<string>? errors = null, int statusCode = 200)
        : base(isSuccess, message, errors, statusCode)
    {
        Value = value;
    }

    public static Result<T> Success(T value, string message = "Operation completed successfully.", int statusCode = 200)
    {
        return new Result<T>(true, value, message, null, statusCode);
    }

    public static new Result<T> Failure(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new Result<T>(false, default, message, errors, statusCode);
    }

    public static new Result<T> NotFound(string message = "Resource not found.")
    {
        return new Result<T>(false, default, message, null, 404);
    }

    public static new Result<T> Forbidden(string message = "Access denied.")
    {
        return new Result<T>(false, default, message, null, 403);
    }

    public static new Result<T> Unauthorized(string message = "Unauthorized access.")
    {
        return new Result<T>(false, default, message, null, 401);
    }
}
