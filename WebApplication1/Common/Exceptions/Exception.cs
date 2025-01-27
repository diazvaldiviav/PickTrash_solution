namespace WebApplication1.Common.Exceptions
{
    // Common/Exceptions/BaseException.cs
    public abstract class BaseException : Exception
    {
        public string Title { get; private set; }
        public int StatusCode { get; private set; }

        protected BaseException(string message, string title, int statusCode)
            : base(message)
        {
            Title = title;
            StatusCode = statusCode;
        }
    }

    // Common/Exceptions/NotFoundException.cs
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message)
            : base(message, "Not Found", StatusCodes.Status404NotFound)
        {
        }
    }

    // Common/Exceptions/BadRequestException.cs
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message)
            : base(message, "Bad Request", StatusCodes.Status400BadRequest)
        {
        }
    }

    // Common/Exceptions/UnauthorizedException.cs
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message)
            : base(message, "Unauthorized", StatusCodes.Status401Unauthorized)
        {
        }
    }

    // Common/Exceptions/ForbiddenException.cs
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message)
            : base(message, "Forbidden", StatusCodes.Status403Forbidden)
        {
        }
    }

    // Common/Exceptions/ConflictException.cs
    public class ConflictException : BaseException
    {
        public ConflictException(string message)
            : base(message, "Conflict", StatusCodes.Status409Conflict)
        {
        }
    }

    // Common/Exceptions/ValidationException.cs
    public class ValidationException : BaseException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.", "Validation Error", StatusCodes.Status422UnprocessableEntity)
        {
            Errors = errors;
        }
    }
}
