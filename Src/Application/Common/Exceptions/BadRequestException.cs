using Application.Common.Responses;
using System.Net;

namespace Application.Common.Exceptions;

public class BadRequestException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public string? ErrorCode { get; }

    public ICollection<ValidationErrorResponse>? Errors { get; }


    public BadRequestException(string message, string errorCode, HttpStatusCode statusCode = HttpStatusCode.BadRequest, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public BadRequestException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
