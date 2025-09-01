using Application.Common.ApiWrapper;
using System.Net;

namespace Application.Common.ApiWrapper;

public class SuccessResultResponse<TData>(TData? data, string message) : ApiBaseResponse<TData>((int)HttpStatusCode.OK, data, message);
