namespace TrainingCenter.Api.Common.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }
    public List<string> Errors { get; }

    public AppException(string message, int statusCode = 400, List<string>? errors = null) : base(message)
    {
        StatusCode = statusCode;
        Errors = errors ?? new List<string>();
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404)
    {
    }
}

public class BadRequestException : AppException
{
    public BadRequestException(string message, List<string>? errors = null) : base(message, 400, errors)
    {
    }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409)
    {
    }
}

public class ValidationException : AppException
{
    public ValidationException(string message, List<string> errors) : base(message, 400, errors)
    {
    }
}
