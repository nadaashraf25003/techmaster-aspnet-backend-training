namespace TrainingCenter.Api.Common.Exceptions;

public class AppException : Exception
{
    public AppException(string message) : base(message) { }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message) { }
}

public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message) : base(message) { }
}

public class ValidationException : AppException
{
    public ValidationException(string message) : base(message) { }
}
