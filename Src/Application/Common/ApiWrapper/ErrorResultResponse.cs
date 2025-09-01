using Application.Common.ApiWrapper;
using Application.Common.Responses;
using Core.Bases;
using System.Text.Json.Serialization;

namespace Application.Common.ApiWrapper;

public class ErrorResultResponse<TData> : ApiBaseResponse<TData>
{
    public ErrorResultResponse(int statusCode, string message) : base(statusCode, message)
    {
    }

	public ErrorResultResponse(int statusCode, TData? data, string message, ICollection<ValidationErrorResponse>? errors = null) : base(statusCode, data, message)
	{
        Errors = errors;
    }

    public string? Exception { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ICollection<ValidationErrorResponse>? Errors { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Source { get; set; }
}
